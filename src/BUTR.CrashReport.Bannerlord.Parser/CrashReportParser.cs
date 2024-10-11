using BUTR.CrashReport.Bannerlord.Parser.Extensions;
using BUTR.CrashReport.Models;
using BUTR.CrashReport.Utils;

using HtmlAgilityPack;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace BUTR.CrashReport.Bannerlord.Parser;

/// <summary>
/// Parses a rendered Crash Report
/// </summary>
public static class CrashReportParser
{
    private delegate bool MatchSpan(ReadOnlySpan<char> span);
    private static IReadOnlyList<string> GetAllOpenTags(ReadOnlySpan<char> content, MatchSpan matcher)
    {
        var list = new List<string>();
        var span = content;
        while (span.IndexOf('<') is var idxOpen and not -1 && span.Slice(idxOpen).IndexOf('>') is var idxClose and not -1)
        {
            var tag = span.Slice(idxOpen, idxClose + 1);
            span = span.Slice(idxOpen + idxClose + 1);
            if (tag.Length < 2 || tag[1] == '/' || tag[^2] == '/') continue;
            if (matcher(tag)) list.Add(tag.ToString());
        }
        return list;
    }

    private static IList<EnhancedStacktraceFrameModel> GetEnhancedStacktrace(ReadOnlySpan<char> rawContent, int version, HtmlNode node)
    {
        const string enhancedStacktraceStartDelimiter1 = "<div id='enhanced-stacktrace' class='headers-container'>";
        const string enhancedStacktraceStartDelimiter2 = "<div id=\"enhanced-stacktrace\" class=\"headers-container\">";
        const string enhancedStacktraceEndDelimiter = "</div>";

        var idx = 0;
        if (rawContent.IndexOf(enhancedStacktraceStartDelimiter1.AsSpan(), StringComparison.Ordinal) is var enhancedStacktraceStartIdx1 and not -1) idx = enhancedStacktraceStartIdx1;
        if (rawContent.IndexOf(enhancedStacktraceStartDelimiter2.AsSpan(), StringComparison.Ordinal) is var enhancedStacktraceStartIdx2 and not -1) idx = enhancedStacktraceStartIdx2;

        if (version < 1000 && idx != -1)
        {
            var enhancedStacktraceEndIdx = rawContent.Slice(idx).IndexOf(enhancedStacktraceEndDelimiter.AsSpan(), StringComparison.Ordinal) - enhancedStacktraceEndDelimiter.Length;
            var enhancedStacktraceRaw = rawContent.Slice(idx, enhancedStacktraceEndIdx).ToString();
            while (GetAllOpenTags(enhancedStacktraceRaw.AsSpan(), span => !span.SequenceEqual(enhancedStacktraceStartDelimiter1.AsSpan()) && !span.SequenceEqual(enhancedStacktraceStartDelimiter2.AsSpan()) && span is not "<ul>" and not "<li>" and not "<br>" and not "<pre>") is { Count: > 0 } toEscape)
            {
                enhancedStacktraceRaw = toEscape.Aggregate(enhancedStacktraceRaw, (current, s) => current.Replace(s, s.Replace("<", "&lt;").Replace(">", "&gt;")));
            }
            //var openTags = GetAllOpenTags(enhancedStacktraceRaw, span => !span.SequenceEqual("<ul>")  && !span.SequenceEqual("<li>") && !span.SequenceEqual("<br>")).ToArray();
            var enhancedStacktraceDoc = new HtmlDocument();
            enhancedStacktraceDoc.LoadHtml(enhancedStacktraceRaw);
            node = enhancedStacktraceDoc.DocumentNode;
        }

        return node.SelectSingleNode("descendant::div[@id=\"enhanced-stacktrace\"]/ul")?.ChildNodes.Where(cn => cn.Name == "li").Select(ParseEnhancedStacktrace).ToArray() ?? [];
    }

    private static HtmlDocument Create(ref string content)
    {
        content = content.Replace("<filename unknown>", "NULL");
        var document = new HtmlDocument();
        document.LoadHtml(content);
        return document;
    }

    /// <summary>
    /// Parses a HTML string that contains the json data
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static string? ParseHtmlJson(string content)
    {
        var document = Create(ref content);
        var gzipBase64Json = document.DocumentNode.SelectSingleNode("descendant::div[@id=\"json-model-data\"]")?.InnerText;
        if (string.IsNullOrEmpty(gzipBase64Json))
            return null;

        return DecompressJson(gzipBase64Json!);
    }

    private static string DecompressJson(string gzipBase64Json)
    {
        var compressedStream = new MemoryStream(Convert.FromBase64String(gzipBase64Json));
        using var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress);
        using var decompressedStream = new MemoryStream();
        decompressorStream.CopyTo(decompressedStream);
        return Encoding.UTF8.GetString(decompressedStream.ToArray());
    }

    /// <summary>
    /// Attempts to parse HTML content and will use the Json model if present or use the HTML code
    /// </summary>
    public static bool TryParse(string content, out byte version, out CrashReportModel? crashReportModel, out string? crashReportJson)
    {
        try
        {
            var document = Create(ref content);

            var versionStr = document.DocumentNode.SelectSingleNode("descendant::report")?.Attributes?["version"]?.Value;
            version = byte.TryParse(versionStr, out var v) ? v : (byte) 1;
            switch (version)
            {
                case >= 13:
                {
                    crashReportModel = null;
                    var gzipBase64Json = document.DocumentNode.SelectSingleNode("descendant::div[@id=\"json-model-data\"]")?.InnerText;
                    crashReportJson = DecompressJson(gzipBase64Json!);
                    return true;
                }
                default:
                {
                    crashReportModel = ParseLegacyHtmlInternal(version, document, content);
                    crashReportJson = null;
                    return true;
                }
            }
        }
        catch (Exception)
        {
            version = 0;
            crashReportModel = null;
            crashReportJson = null;
            return false;
        }
    }

    /// <summary>
    /// Parses the log files from the old HTML report
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static IEnumerable<LogSource> ParseLegacyHtmlLogs(string content)
    {
        var document = Create(ref content);

        var versionStr = document.DocumentNode.SelectSingleNode("descendant::report")?.Attributes?["version"]?.Value;
        var version = byte.TryParse(versionStr, out var v) ? v : (byte) 1;
        switch (version)
        {
            case >= 13:
            {
                return [];
            }
            default:
            {
                var exceptionNode = document.DocumentNode.SelectSingleNode("descendant::div[@id=\"log-files\"]");
                return ParseLogsInternal(exceptionNode);
            }
        }
    }

    private static IEnumerable<LogSource> ParseLogsInternal(HtmlNode node)
    {
        static LogEntry? ParseLogEntry(HtmlNode node)
        {
            var line = node.InnerText;
            var idxDateStart = line.IndexOf('[') + 1;
            var idxDateEnd = line.IndexOf(']');
            if (idxDateStart == -1 || idxDateEnd == -1) return null;

            if (!DateTimeOffset.TryParse(line.Substring(idxDateStart, idxDateEnd - idxDateStart), DateTimeFormatInfo.InvariantInfo, DateTimeStyles.RoundtripKind, out var date))
                return null;

            var idxTypeStart = line.IndexOf('[', idxDateEnd + 1) + 1;
            var idxTypeEnd = line.IndexOf(']', idxDateEnd + 1);
            if (idxTypeStart == -1 || idxTypeEnd == -1) return null;

            var idxLevelStart = line.IndexOf('[', idxTypeEnd + 1) + 1;
            var idxLevelEnd = line.IndexOf(']', idxTypeEnd + 1);
            if (idxLevelStart == -1 || idxLevelEnd == -1) return null;

            return new()
            {
                Date = date,
                Type = line.Substring(idxTypeStart, idxTypeEnd - idxTypeStart),
                Level = line.Substring(idxLevelStart, idxLevelEnd - idxLevelStart) switch
                {
                    "VRB" => LogLevel.Verbose,
                    "DBG" => LogLevel.Debug,
                    "INF" => LogLevel.Information,
                    "WRN" => LogLevel.Warning,
                    "ERR" => LogLevel.Error,
                    "FTL" => LogLevel.Fatal,
                    _ => LogLevel.Information,
                },
                Message = line.Substring(idxLevelEnd + 3),
            };
        }

        foreach (var source in node.SelectNodes("ul/li"))
        {
            var name = source.ChildNodes.First().InnerText;
            var entries = source.SelectNodes("ul/ul/li")?.Select(ParseLogEntry) ?? Array.Empty<LogEntry>();
            yield return new()
            {
                Name = name,
                Logs = entries.OfType<LogEntry>().ToList(),
                AdditionalMetadata = Array.Empty<MetadataModel>(),
            };
        }
    }

    /// <summary>
    /// Parses the HTML file with a specific version provided
    /// </summary>
    public static CrashReportModel ParseLegacyHtml(byte version, string content)
    {
        var document = Create(ref content);
        return ParseLegacyHtmlInternal(version, document, content);
    }

    private static CrashReportModel ParseLegacyHtmlInternal(byte version, HtmlDocument document, string content)
    {
        var node = document.DocumentNode;
        var id = node.SelectSingleNode("descendant::report")?.Attributes?["id"]?.Value ?? string.Empty;
        var gameVersion = node.SelectSingleNode("descendant::game")?.Attributes?["version"]?.Value ?? string.Empty;
        var installedModules = node.SelectSingleNode("descendant::div[@id=\"installed-modules\"]/ul")?.ChildNodes.Where(cn => cn.Name == "li").Select(x => ParseModule(version, x)).DistinctBy(x => x.Id).ToArray() ?? [];
        var exception = ParseExceptions(node.SelectSingleNode("descendant::div[@id=\"exception\"]"), installedModules);
        var involvedModules = node.SelectSingleNode("descendant::div[@id=\"involved-modules\"]/ul")?.ChildNodes.Where(cn => cn.Name == "li").SelectMany(ParseInvolvedModule).ToArray() ?? [];
        var enhancedStacktrace = GetEnhancedStacktrace(content.AsSpan(), version, node);

        var assemblies = node.SelectSingleNode("descendant::div[@id=\"assemblies\"]/ul")?.ChildNodes.Where(cn => cn.Name == "li").Select(x => ParseAssembly(x, installedModules)).ToArray() ?? [];
        var harmonyPatches = node.SelectSingleNode("descendant::div[@id=\"harmony-patches\"]/ul").ChildNodes.Where(cn => cn.Name == "li").Select(ParseHarmonyPatch).ToArray();
        var launcherType = node.SelectSingleNode("descendant::launcher")?.Attributes?["type"]?.Value ?? string.Empty;
        var launcherVersion = node.SelectSingleNode("descendant::launcher")?.Attributes?["version"]?.Value ?? string.Empty;
        var runtime = node.SelectSingleNode("descendant::runtime")?.Attributes?["value"]?.Value ?? string.Empty;
        var butrloaderVersion = node.SelectSingleNode("descendant::butrloader")?.Attributes?["version"]?.Value ?? string.Empty;
        var blseVersion = node.SelectSingleNode("descendant::blse")?.Attributes?["version"]?.Value ?? string.Empty;
        var launcherexVersion = node.SelectSingleNode("descendant::launcherex")?.Attributes?["version"]?.Value ?? string.Empty;

        return new CrashReportModel
        {
            Id = Guid.TryParse(id, out var val) ? val : Guid.Empty,
            Version = version,
            Exception = exception,
            Metadata = new()
            {
                GameName = "Bannerlord",
                GameVersion = gameVersion,
                LoaderPluginProviderName = !string.IsNullOrEmpty(butrloaderVersion) ? "BUTRLoader" : string.IsNullOrEmpty(blseVersion) ? "BLSE" : null,
                LoaderPluginProviderVersion = !string.IsNullOrEmpty(butrloaderVersion) ? butrloaderVersion : string.IsNullOrEmpty(blseVersion) ? blseVersion : null,
                LauncherType = launcherType,
                LauncherVersion = launcherVersion,
                Runtime = runtime,
                OperatingSystemType = OperatingSystemType.Unknown,
                OperatingSystemVersion = null,
                AdditionalMetadata = new List<MetadataModel>
                {
                    new() { Key = "LauncherExVersion", Value = launcherexVersion },
                },
            },
            Modules = installedModules,
            InvolvedModules = involvedModules,
            EnhancedStacktrace = enhancedStacktrace,
            Assemblies = assemblies,
            NativeModules = Array.Empty<NativeAssemblyModel>(),
            HarmonyPatches = harmonyPatches,
            //MonoModDetours = Array.Empty<MonoModDetoursModel>(),
            LoaderPlugins = Array.Empty<LoaderPluginModel>(),
            InvolvedLoaderPlugins = Array.Empty<InvolvedModuleOrPluginModel>(),
            AdditionalMetadata = Array.Empty<MetadataModel>(),
        };
    }

    private static ExceptionModel ParseExceptions(HtmlNode node, ModuleModel[] modules)
    {
        var exceptions = new List<ExceptionModel>();

        foreach (var exception in node.InnerHtml.Split("Inner Exception information"))
        {
            var exceptionLines = exception.Split(["<br>", "</br>"], StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim().Trim('\n')).Where(x => x.Length != 0).ToList();
            var type = exceptionLines.First(x => x.StartsWith("Type: ")).Substring(6);
            var message = exceptionLines.First(x => x.StartsWith("Message: ")).Substring(9);
            var source = exceptionLines.First(x => x.StartsWith("Source: ")).Substring(8);
            var callstackIdx = exceptionLines.FindIndex(x => x.StartsWith("CallStack:"));
            var callstack = string.Join(Environment.NewLine, exceptionLines.Skip(callstackIdx + 1)).Replace("<ol>\n", "").Replace("<li>", "").Replace("</li>\n", Environment.NewLine).Replace("</ol>", "");
            exceptions.Add(new ExceptionModel
            {
                SourceAssemblyId = null,
                SourceModuleId = modules.Any(x => x.Id == source) ? source : null,
                SourceLoaderPluginId = null,
                Type = type,
                Message = message,
                CallStack = callstack,
                InnerException = null,
                AdditionalMetadata = Array.Empty<MetadataModel>(),
            });
        }

        var currentException = default(ExceptionModel);
        foreach (var exception in exceptions.AsEnumerable().Reverse())
        {
            exception.InnerException = currentException;
            currentException = exception;
        }

        return currentException!;
    }

    private static ModuleModel ParseModule(byte version, HtmlNode node)
    {
        static string GetField(IEnumerable<string> lines, string field) => lines
            .FirstOrDefault(l => l.StartsWith($"{field}:"))?.Split([$"{field}:"], StringSplitOptions.None).Skip(1).FirstOrDefault()?.Trim() ?? string.Empty;

        static IReadOnlyList<string> GetRange(IEnumerable<string> lines, string bField, IEnumerable<string> eFields) => lines
            .SkipWhile(l => !l.StartsWith($"{bField}:")).Skip(1)
            .TakeWhile(l => eFields.All(f => !l.StartsWith($"{f}:")))
            .ToArray();

        static IList<DependencyMetadataModel> GetModuleDependencyMetadatas(IReadOnlyList<string> lines) => lines.Select(sml => new DependencyMetadataModel
        {
            Type = sml.StartsWith("Load Before") ? DependencyMetadataModelType.LoadBefore
                : sml.StartsWith("Load After") ? DependencyMetadataModelType.LoadAfter
                : sml.StartsWith("Incompatible") ? DependencyMetadataModelType.Incompatible
                : 0,
            ModuleOrPluginId = sml.Replace("Load Before", "").Replace("Load After", "").Replace("Incompatible", "").Replace("(optional)", "").Trim(),
            IsOptional = sml.Contains("(optional)"),
            Version = null, // Was not available pre 13
            VersionRange = null, // Was not available pre 13
            AdditionalMetadata = Array.Empty<MetadataModel>(),
        }).ToArray();

        static IList<ModuleSubModuleModel> GetModuleSubModules(IReadOnlyList<string> lines) => lines
            .Select((item, index) => new { Item = item, Index = index })
            .Where(o => !o.Item.Contains(':') && !o.Item.Contains(".dll"))
            .Select(o => lines.Skip(o.Index + 1).TakeWhile(l => l.Contains(':') || l.Contains(".dll")).ToArray())
            .Select(sml => new ModuleSubModuleModel
            {
                Name = sml.FirstOrDefault(l => l.StartsWith("Name:"))?.Split("Name:").Skip(1).FirstOrDefault()?.Trim() ?? string.Empty,
                AssemblyId = new()
                {
                    Name = sml.FirstOrDefault(l => l.StartsWith("DLLName:"))?.Split("DLLName:").Skip(1).FirstOrDefault()?.Trim() ?? string.Empty,
                    Version = null,
                    PublicKeyToken = null
                },
                Entrypoint = sml.FirstOrDefault(l => l.StartsWith("SubModuleClassType:"))?.Split("SubModuleClassType:").Skip(1).FirstOrDefault()?.Trim() ?? string.Empty,
                AdditionalMetadata = sml.SkipWhile(l => !l.StartsWith("Tags:")).Skip(1).TakeWhile(l => !l.StartsWith("Assemblies:")).Select(l =>
                {
                    var split = l.Split(':', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                    return new MetadataModel { Key = split[0], Value = split[1], };
                }).Concat(sml.SkipWhile(l => !l.StartsWith("Assemblies:")).Skip(1).Select(l =>
                {
                    return new MetadataModel { Key = "METADATA:Assembly", Value = l, };
                })).ToArray(),
            })
            .ToArray();

        var lines = node.InnerText.Split(["\r\n", "\r", "\n"], StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
        var isVortex = GetField(lines, "Vortex").Equals("true", StringComparison.OrdinalIgnoreCase);
        var moduleModel = new ModuleModel
        {
            Id = GetField(lines, "Id"),
            Name = GetField(lines, "Name"),
            Version = GetField(lines, "Version"),
            IsExternal = GetField(lines, "External").Equals("true", StringComparison.OrdinalIgnoreCase),
            IsOfficial = GetField(lines, "Official").Equals("true", StringComparison.OrdinalIgnoreCase),
            IsSingleplayer = GetField(lines, "Singleplayer").Equals("true", StringComparison.OrdinalIgnoreCase),
            IsMultiplayer = GetField(lines, "Multiplayer").Equals("true", StringComparison.OrdinalIgnoreCase),
            Url = GetField(lines, "Url"),
            UpdateInfo = null,
            DependencyMetadatas = GetModuleDependencyMetadatas(GetRange(lines, version == 1 ? "Dependency Metadatas" : "Dependencies", ["SubModules", "Additional Assemblies", "Url"
            ])),
            SubModules = GetModuleSubModules(GetRange(lines, "SubModules", ["Additional Assemblies"])),
            Capabilities = Array.Empty<CapabilityModuleOrPluginModel>(),
            AdditionalMetadata = new List<MetadataModel> { new() { Key = "METADATA:MANAGED_BY_VORTEX", Value = isVortex.ToString() } }.Concat(lines.SkipWhile(l => !l.StartsWith("Additional Assemblies:")).Skip(1).Select(l =>
            {
                return new MetadataModel { Key = "METADATA:AdditionalAssembly", Value = l, };
            })).ToList(),
        };
        return moduleModel;
    }

    private static IEnumerable<InvolvedModuleOrPluginModel> ParseInvolvedModule(HtmlNode node)
    {
        var id = node.ChildNodes.FirstOrDefault(x => x.Name == "a")?.InnerText.Trim() ?? string.Empty;
        return node.ChildNodes.FirstOrDefault(x => x.Name == "ul")?.ChildNodes.Select(x =>
        {
            var lines = x.InnerHtml.Split("<br>");
            var frame = lines.FirstOrDefault(y => y.StartsWith("Frame: "))?.Replace("::", ".").Substring(7) ?? string.Empty;
            return new InvolvedModuleOrPluginModel
            {
                ModuleOrLoaderPluginId = id,
                EnhancedStacktraceFrameName = frame,
                AdditionalMetadata = Array.Empty<MetadataModel>(),
            };
        }) ?? Array.Empty<InvolvedModuleOrPluginModel>();
    }

    private static EnhancedStacktraceFrameModel ParseEnhancedStacktrace(HtmlNode node)
    {
        var frameLine = node.ChildNodes.FirstOrDefault()?.InnerText.Trim().Replace("\r\n", string.Empty) ?? string.Empty;
        var name = frameLine.Replace("Frame: ", "");
        var ilOffsetIdx = name.IndexOf(" (IL Offset: ", StringComparison.Ordinal);
        name = ilOffsetIdx != -1 ? name.Substring(0, ilOffsetIdx) : name;
        var ilOffset = int.TryParse(frameLine.Split("(IL Offset: ").Skip(1).FirstOrDefault()?.Replace(")", string.Empty).Trim(), out var ilOffsetVal) ? ilOffsetVal : -1;

        var methods = new List<MethodSimple>();
        foreach (var childNode in node.ChildNodes.FirstOrDefault(x => x.Name == "ul")?.ChildNodes ?? Enumerable.Empty<HtmlNode>())
        {
            var lines = childNode.InnerHtml.Replace("&lt;", "<").Replace("&gt;", ">").Trim().Split("<br>", StringSplitOptions.RemoveEmptyEntries);
            var module = lines.Length > 0 ? lines[0].Substring(8) : null;
            var methodFullDescription = lines.Length > 1 ? lines[1].Substring(8).Replace("::", ".") : string.Empty;
            var idx1 = methodFullDescription.IndexOf("(", StringComparison.Ordinal);
            var idx2 = idx1 != -1 ? methodFullDescription.Substring(0, idx1).LastIndexOf(" ", StringComparison.Ordinal) : -1;
            var method = idx2 != -1 ? methodFullDescription.Substring(idx2 + 1) : string.Empty;
            var methodSplit = method.Split("(");
            var parameters = methodSplit.Length > 1
                ? methodSplit[1].Trim(')').Split(" ", StringSplitOptions.RemoveEmptyEntries)
                    .Where((_, i) => i % 2 == 0)
                    .Select(x => x.Trim(','))
                    .ToList()
                : [];
            var methodFullName = methodSplit[0].Replace("::", ".");
            var split = methodFullName.Split('.');
            methods.Add(new()
            {
                AssemblyId = null,
                ModuleId = module,
                LoaderPluginId = null,
                MethodDeclaredTypeName = split.Length == 1 ? null : string.Join(".", split.Take(split.Length - 1)),
                MethodName = split.Last(),
                MethodFullDescription = methodFullDescription,
                MethodParameters = parameters,
                ILInstructions = Array.Empty<string>(),
                CSharpILMixedInstructions = Array.Empty<string>(),
                CSharpInstructions = Array.Empty<string>(),
                AdditionalMetadata = Array.Empty<MetadataModel>(),
            });
        }

        var executingMethod = methods.Last();
        return new()
        {
            ILOffset = ilOffset,
            NativeOffset = null,
            FrameDescription = name,
            ExecutingMethod = new()
            {
                AssemblyId = null,
                ModuleId = executingMethod.ModuleId,
                LoaderPluginId = null,
                MethodDeclaredTypeName = executingMethod.MethodDeclaredTypeName,
                MethodName = executingMethod.MethodName,
                MethodFullDescription = executingMethod.MethodFullDescription,
                MethodParameters = executingMethod.MethodParameters,
                NativeInstructions = Array.Empty<string>(),
                ILInstructions = executingMethod.ILInstructions,
                CSharpILMixedInstructions = Array.Empty<string>(),
                CSharpInstructions = Array.Empty<string>(),
                AdditionalMetadata = executingMethod.AdditionalMetadata,
            },
            OriginalMethod = null,
            PatchMethods = methods.Count == 1 ? [] : methods.Take(methods.Count - 1).ToArray(),
            MethodFromStackframeIssue = false,
            AdditionalMetadata = Array.Empty<MetadataModel>(),
        };
    }

    private static AssemblyModel ParseAssembly(HtmlNode node, ModuleModel[] modules)
    {
        var splt = node.InnerText.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
        var outerHtml = node.OuterHtml;

        var isDynamic = splt[3].Equals("DYNAMIC");
        var isEmpty = splt[3].Equals("EMPTY");

        var module = modules.FirstOrDefault(x => x.AdditionalMetadata.Where(y => y.Key == "METADATA:AdditionalAssembly").Any(kv =>
        {
            var splt2 = kv.Value.Split(" (");
            var fullName = splt2[1].TrimEnd(')');
            return fullName.StartsWith(splt[0]);
        }));
        var assemblyModel = new AssemblyModel
        {
            Id = new()
            {
                Name = splt[0],
                Version = splt[1],
                PublicKeyToken = null,
            },
            ModuleId = module?.Id,
            LoaderPluginId = null,
            CultureName = null,
            Architecture = Enum.TryParse<AssemblyArchitectureType>(splt[2], true, out var arch) ? arch : AssemblyArchitectureType.Unknown,
            Hash = isDynamic || isEmpty ? string.Empty : splt[3],
            AnonymizedPath = isDynamic ? "DYNAMIC" : isEmpty ? "EMPTY" : Anonymizer.AnonymizePath(splt[4]),

            Type = (outerHtml.Contains("dynamic_assembly") ? AssemblyModelType.Dynamic
                : outerHtml.Contains("gac_assembly") ? AssemblyModelType.GAC
                : outerHtml.Contains("tw_assembly") ? AssemblyModelType.GameCore
                : outerHtml.Contains("tw_module_assembly") ? AssemblyModelType.GameModule
                : outerHtml.Contains("module_assembly") ? AssemblyModelType.Module
                : AssemblyModelType.Unclassified) | (isDynamic ? AssemblyModelType.Dynamic : AssemblyModelType.Unclassified),

            ImportedTypeReferences = Array.Empty<AssemblyImportedTypeReferenceModel>(),
            ImportedAssemblyReferences = Array.Empty<AssemblyImportedReferenceModel>(),

            AdditionalMetadata = Array.Empty<MetadataModel>(),
        };
        return assemblyModel;
    }

    private static HarmonyPatchesModel ParseHarmonyPatch(HtmlNode node)
    {
        static HarmonyPatchModel ParsePatch(HtmlNode node, HarmonyPatchType type)
        {
            var split = node.InnerText.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            return new HarmonyPatchModel
            {
                Type = type,
                AssemblyId = null,
                ModuleId = null,
                LoaderPluginId = null,
                Owner = split.FirstOrDefault(x => x.StartsWith("Owner: "))?.Split(':')[1] ?? string.Empty,
                Namespace = split.FirstOrDefault(x => x.StartsWith("Namespace: "))?.Split(':')[1] ?? string.Empty,
                Index = split.FirstOrDefault(x => x.StartsWith("Index: "))?.Split(':')[1] is { } strIndex && int.TryParse(strIndex, out var index) ? index : 0,
                Priority = split.FirstOrDefault(x => x.StartsWith("Priority: "))?.Split(':')[1] is { } strPriority && int.TryParse(strPriority, out var piority) ? piority : 400,
                Before = split.FirstOrDefault(x => x.StartsWith("Before: "))?.Split(':')[1].Split(',') ?? [],
                After = split.FirstOrDefault(x => x.StartsWith("After: "))?.Split(':')[1].Split(',') ?? [],
                AdditionalMetadata = Array.Empty<MetadataModel>(),
            };
        }

        var originalMethodFullName = node.ChildNodes.Skip(0).First().InnerText.Trim('\n');
        var prefixes = node.ChildNodes.FirstOrDefault(x => x.InnerText?.Contains("Prefixes") == true)?.SelectSingleNode("descendant::ul/li")?.ChildNodes.Select(x => ParsePatch(x, HarmonyPatchType.Prefix)).ToArray() ?? [];
        var postfixes = node.ChildNodes.FirstOrDefault(x => x.InnerText?.Contains("Postfixes") == true)?.SelectSingleNode("descendant::ul/li")?.ChildNodes.Select(x => ParsePatch(x, HarmonyPatchType.Postfix)).ToArray() ?? [];
        var transpilers = node.ChildNodes.FirstOrDefault(x => x.InnerText?.Contains("Transpilers") == true)?.SelectSingleNode("descendant::ul/li")?.ChildNodes.Select(x => ParsePatch(x, HarmonyPatchType.Transpiler)).ToArray() ?? [];
        var finalizers = node.ChildNodes.FirstOrDefault(x => x.InnerText?.Contains("Finalizers") == true)?.SelectSingleNode("descendant::ul/li")?.ChildNodes.Select(x => ParsePatch(x, HarmonyPatchType.Finalizer)).ToArray() ?? [];
        var harmonyPatchModel = new HarmonyPatchesModel
        {
            OriginalMethodName = originalMethodFullName.Split('.').Last(),
            OriginalMethodDeclaredTypeName = originalMethodFullName,
            Patches = prefixes.Concat(postfixes).Concat(transpilers).Concat(finalizers).ToArray(),
            AdditionalMetadata = Array.Empty<MetadataModel>(),
        };
        return harmonyPatchModel;
    }
}