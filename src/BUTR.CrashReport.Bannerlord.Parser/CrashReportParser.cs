using BUTR.CrashReport.Extensions;
using BUTR.CrashReport.Models;

using HtmlAgilityPack;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;

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

    private static IReadOnlyList<EnhancedStacktraceFrameModel> GetEnhancedStacktrace(ReadOnlySpan<char> rawContent, int version, HtmlNode node)
    {
        const string enhancedStacktraceStartDelimiter1 = "<div id='enhanced-stacktrace' class='headers-container'>";
        const string enhancedStacktraceStartDelimiter2 = "<div id=\"enhanced-stacktrace\" class=\"headers-container\">";
        const string enhancedStacktraceEndDelimiter = "</div>";

        var idx = 0;
        if (rawContent.IndexOf(enhancedStacktraceStartDelimiter1.AsSpan()) is var enhancedStacktraceStartIdx1 and not -1) idx = enhancedStacktraceStartIdx1;
        if (rawContent.IndexOf(enhancedStacktraceStartDelimiter2.AsSpan()) is var enhancedStacktraceStartIdx2 and not -1) idx = enhancedStacktraceStartIdx2;

        if (version < 1000 && idx != -1)
        {
            var enhancedStacktraceEndIdx = rawContent.Slice(idx).IndexOf(enhancedStacktraceEndDelimiter.AsSpan()) - enhancedStacktraceEndDelimiter.Length;
            var enhancedStacktraceRaw = rawContent.Slice(idx, enhancedStacktraceEndIdx).ToString();
            var toEscape = GetAllOpenTags(enhancedStacktraceRaw.AsSpan(), span => !span.SequenceEqual(enhancedStacktraceStartDelimiter1.AsSpan()) && !span.SequenceEqual(enhancedStacktraceStartDelimiter2.AsSpan()) && span is not "<ul>" and not "<li>" and not "<br>");
            enhancedStacktraceRaw = toEscape.Aggregate(enhancedStacktraceRaw, (current, s) => current.Replace(s, s.Replace("<", "&lt;").Replace(">", "&gt;")));
            //var openTags = GetAllOpenTags(enhancedStacktraceRaw, span => !span.SequenceEqual("<ul>")  && !span.SequenceEqual("<li>") && !span.SequenceEqual("<br>")).ToArray();
            var enhancedStacktraceDoc = new HtmlDocument();
            enhancedStacktraceDoc.LoadHtml(enhancedStacktraceRaw);
            node = enhancedStacktraceDoc.DocumentNode;
        }

        return node.SelectSingleNode("descendant::div[@id=\"enhanced-stacktrace\"]/ul")?.ChildNodes.Where(cn => cn.Name == "li").Select(ParseEnhancedStacktrace).ToArray() ?? Array.Empty<EnhancedStacktraceFrameModel>();
    }

    public static string? ParseHtmlJson(string content)
    {
        var document = new HtmlDocument();
        document.LoadHtml(content.Replace("<filename unknown>", "NULL"));
        return document.DocumentNode.SelectSingleNode("descendant::div[@id=\"json-model-data\"]")?.InnerText;
    }

    public static bool TryParse(string content, out byte version, out CrashReportModel? crashReportModel, out string? crashReportJson)
    {
        try
        {
            var document = new HtmlDocument();
            document.LoadHtml(content.Replace("<filename unknown>", "NULL"));

            var versionStr = document.DocumentNode.SelectSingleNode("descendant::report")?.Attributes?["version"]?.Value;
            version = byte.TryParse(versionStr, out var v) ? v : (byte) 1;
            switch (version)
            {
                case >= 13:
                {
                    crashReportModel = null;
                    crashReportJson = document.DocumentNode.SelectSingleNode("descendant::div[@id=\"json-model-data\"]")?.InnerText;
                    return true;
                }
                default:
                {
                    crashReportModel = ParseLegacyHtml(version, document, content);
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

    public static IEnumerable<LogSource> ParseLegacyHtmlLogs(string content)
    {
        var html = new HtmlDocument();
        html.LoadHtml(content.Replace("<filename unknown>", "NULL"));
        var document = html.DocumentNode;

        var versionStr = document.SelectSingleNode("descendant::report")?.Attributes?["version"]?.Value;
        var version = byte.TryParse(versionStr, out var v) ? v : (byte) 1;
        switch (version)
        {
            case >= 13:
            {
                return Enumerable.Empty<LogSource>();
            }
            default:
            {
                var exceptionNode = document.SelectSingleNode("descendant::div[@id=\"log-files\"]");
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
                Level = line.Substring(idxLevelStart, idxLevelEnd - idxLevelStart),
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
                Logs = entries.OfType<LogEntry>().ToImmutableArray(),
                AdditionalMetadata = ImmutableArray<MetadataModel>.Empty,
            };
        }
    }

    public static CrashReportModel ParseLegacyHtml(byte version, HtmlDocument document, string content)
    {
        var node = document.DocumentNode;
        var id = node.SelectSingleNode("descendant::report")?.Attributes?["id"]?.Value ?? string.Empty;
        var gameVersion = node.SelectSingleNode("descendant::game")?.Attributes?["version"]?.Value ?? string.Empty;
        var exception = ParseExceptions(node.SelectSingleNode("descendant::div[@id=\"exception\"]"));
        var installedModules = node.SelectSingleNode("descendant::div[@id=\"installed-modules\"]/ul")?.ChildNodes.Where(cn => cn.Name == "li").Select(x => ParseModule(version, x)).DistinctBy(x => x.Id).ToArray() ?? Array.Empty<ModuleModel>();
        var involvedModules = node.SelectSingleNode("descendant::div[@id=\"involved-modules\"]/ul")?.ChildNodes.Where(cn => cn.Name == "li").SelectMany(ParseInvolvedModule).ToArray() ?? Array.Empty<InvolvedModuleModel>();
        var enhancedStacktrace = GetEnhancedStacktrace(content.AsSpan(), version, node);

        var assemblies = node.SelectSingleNode("descendant::div[@id=\"assemblies\"]/ul")?.ChildNodes.Where(cn => cn.Name == "li").Select(ParseAssembly).ToArray() ?? Array.Empty<AssemblyModel>();
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
            GameVersion = gameVersion,
            Exception = exception,
            Metadata = new()
            {
                LauncherType = launcherType,
                LauncherVersion = launcherVersion,
                Runtime = runtime,
                AdditionalMetadata = new List<MetadataModel>
                {
                    new() { Key = "BUTRLoaderVersion", Value = butrloaderVersion },
                    new() { Key = "BLSEVersion", Value = blseVersion },
                    new() { Key = "LauncherExVersion", Value = launcherexVersion },
                },
            },
            Modules = installedModules,
            InvolvedModules = involvedModules,
            EnhancedStacktrace = enhancedStacktrace,
            Assemblies = assemblies,
            HarmonyPatches = harmonyPatches,
            AdditionalMetadata = ImmutableArray<MetadataModel>.Empty,
        };
    }

    private static ExceptionModel ParseExceptions(HtmlNode node)
    {
        var exceptions = new List<ExceptionModel>();

        foreach (var exception in node.InnerText.Split("Inner Exception information"))
        {
            var exceptionLines = exception.Split('\n', StringSplitOptions.RemoveEmptyEntries).Where(x => x.Trim().Length != 0).ToList();
            var type = exceptionLines.First(x => x.StartsWith("Type: ")).Substring(6);
            var message = exceptionLines.First(x => x.StartsWith("Message: ")).Substring(9);
            //var source = exceptionLines.First(x => x.StartsWith("Source: ")).Substring(8);
            var callstackIdx = exceptionLines.FindIndex(x => x.StartsWith("CallStack:"));
            var callstack = string.Join(Environment.NewLine, exceptionLines.Skip(callstackIdx + 1));
            exceptions.Add(new ExceptionModel
            {
                Type = type,
                Message = message,
                CallStack = callstack,
                InnerException = null,
                AdditionalMetadata = ImmutableArray<MetadataModel>.Empty,
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
            .FirstOrDefault(l => l.StartsWith($"{field}:"))?.Split(new[] { $"{field}:" }, StringSplitOptions.None).Skip(1).FirstOrDefault()?.Trim() ?? string.Empty;

        static IReadOnlyList<string> GetRange(IEnumerable<string> lines, string bField, IEnumerable<string> eFields) => lines
            .SkipWhile(l => !l.StartsWith($"{bField}:")).Skip(1)
            .TakeWhile(l => eFields.All(f => !l.StartsWith($"{f}:")))
            .ToArray();

        static IReadOnlyList<ModuleDependencyMetadataModel> GetModuleDependencyMetadatas(IReadOnlyList<string> lines) => lines.Select(sml => new ModuleDependencyMetadataModel
        {
            Type = sml.StartsWith("Load Before") ? ModuleDependencyMetadataModelType.LoadBefore
                : sml.StartsWith("Load After") ? ModuleDependencyMetadataModelType.LoadAfter
                : sml.StartsWith("Incompatible") ? ModuleDependencyMetadataModelType.Incompatible
                : 0,
            ModuleId = sml.Replace("Load Before", "").Replace("Load After", "").Replace("Incompatible", "").Replace("(optional)", "").Trim(),
            IsOptional = sml.Contains("(optional)"),
            Version = null, // Was not available pre 13
            VersionRange = null, // Was not available pre 13
            AdditionalMetadata = ImmutableArray<MetadataModel>.Empty,
        }).ToArray();

        static IReadOnlyList<ModuleSubModuleModel> GetModuleSubModules(IReadOnlyList<string> lines) => lines
            .Select((item, index) => new { Item = item, Index = index })
            .Where(o => !o.Item.Contains(':') && !o.Item.Contains(".dll"))
            .Select(o => lines.Skip(o.Index + 1).TakeWhile(l => l.Contains(':') || l.Contains(".dll")).ToArray())
            .Select(sml => new ModuleSubModuleModel
            {
                Name = sml.FirstOrDefault(l => l.StartsWith("Name:"))?.Split("Name:").Skip(1).FirstOrDefault()?.Trim() ?? string.Empty,
                AssemblyName = sml.FirstOrDefault(l => l.StartsWith("DLLName:"))?.Split("DLLName:").Skip(1).FirstOrDefault()?.Trim() ?? string.Empty,
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

        var lines = node.InnerText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
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
            DependencyMetadatas = GetModuleDependencyMetadatas(GetRange(lines, version == 1 ? "Dependency Metadatas" : "Dependencies", new[] { "SubModules", "Additional Assemblies", "Url" })),
            SubModules = GetModuleSubModules(GetRange(lines, "SubModules", new[] { "Additional Assemblies" })),
            AdditionalMetadata = ImmutableArray.Create<MetadataModel>(new MetadataModel { Key = "METADATA:MANAGED_BY_VORTEX", Value = isVortex.ToString()}).AddRange(lines.SkipWhile(l => !l.StartsWith("Additional Assemblies:")).Skip(1).Select(l =>
            {
                return new MetadataModel { Key = "METADATA:AdditionalAssembly", Value = l, };
            })),
        };
        return moduleModel;
    }

    private static IEnumerable<InvolvedModuleModel> ParseInvolvedModule(HtmlNode node)
    {
        var id = node.ChildNodes.FirstOrDefault(x => x.Name == "a")?.InnerText.Trim() ?? string.Empty;
        return node.ChildNodes.FirstOrDefault(x => x.Name == "ul")?.ChildNodes.Select(x =>
        {
            var lines = x.InnerHtml.Split("<br>");
            var frame = lines.FirstOrDefault(y => y.StartsWith("Frame: "))?.Replace("::", ".").Substring(7) ?? string.Empty;
            return new InvolvedModuleModel
            {
                Id = id,
                EnhancedStacktraceFrameName = frame,
                AdditionalMetadata = ImmutableArray<MetadataModel>.Empty,
            };
        }) ?? Array.Empty<InvolvedModuleModel>();
    }

    private static EnhancedStacktraceFrameModel ParseEnhancedStacktrace(HtmlNode node)
    {
        var frameLine = node.ChildNodes.FirstOrDefault()?.InnerText.Trim().Replace("\r\n", string.Empty) ?? string.Empty;
        var name = frameLine.Replace("Frame: ", "");
        var ilOffsetIdx = name.IndexOf(" (IL Offset: ", StringComparison.Ordinal);
        name = ilOffsetIdx != -1 ? name.Substring(0, ilOffsetIdx) : name;
        var ilOffset = int.TryParse(frameLine.Split("(IL Offset: ").Skip(1).FirstOrDefault()?.Replace(")", string.Empty).Trim(), out var ilOffsetVal) ? ilOffsetVal : -1;

        var methods = new List<EnhancedStacktraceFrameMethod>();
        foreach (var childNode in node.ChildNodes.FirstOrDefault(x => x.Name == "ul")?.ChildNodes ?? Enumerable.Empty<HtmlNode>())
        {
            var lines = childNode.InnerHtml.Replace("&lt;", "<").Replace("&gt;", ">").Trim().Split("<br>", StringSplitOptions.RemoveEmptyEntries);
            var module = lines?.Length > 0 ? lines[0].Substring(8) : "UNKNOWN";
            var methodFullName = lines?.Length > 1 ? lines[1].Substring(8).Replace("::", ".") : string.Empty;
            var idx1 = methodFullName.IndexOf("(", StringComparison.Ordinal);
            var idx2 = idx1 != -1 ? methodFullName.Substring(0, idx1).LastIndexOf(" ", StringComparison.Ordinal) : -1;
            var method = idx2 != -1 ? methodFullName.Substring(idx2 + 1) : string.Empty;
            var methodSplit = method.Split("(");
            var parameters = methodSplit.Length > 1
                ? methodSplit[1].Trim(')').Split(" ", StringSplitOptions.RemoveEmptyEntries)
                    .Where((_, i) => i % 2 == 0)
                    .Select(x => x.Trim(','))
                    .ToImmutableArray()
                : ImmutableArray<string>.Empty;
            methods.Add(new EnhancedStacktraceFrameMethod
            {
                Module = module,
                MethodFullName = methodFullName,
                Method = methodSplit[0].Replace("::", "."),
                MethodParameters = parameters,
                NativeInstructions = ImmutableArray<string>.Empty,
                CilInstructions = ImmutableArray<string>.Empty,
                AdditionalMetadata = ImmutableArray<MetadataModel>.Empty,
            });
        }

        var originalMethod = methods.Last();
        return new EnhancedStacktraceFrameModel
        {
            Name = name,
            ILOffset = ilOffset,
            NativeOffset = null,
            FrameDescription = name,
            OriginalMethod = originalMethod,
            PatchMethods = methods.Count == 1 ? ImmutableArray<EnhancedStacktraceFrameMethod>.Empty : methods.Take(methods.Count - 1).ToImmutableArray(),
            MethodFromStackframeIssue = false,
            AdditionalMetadata = ImmutableArray<MetadataModel>.Empty,
        };
    }

    private static AssemblyModel ParseAssembly(HtmlNode node)
    {
        var splt = node.InnerText.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
        var outerHtml = node.OuterHtml;

        var isDynamic = splt[3].Equals("DYNAMIC");
        var isEmpty = splt[3].Equals("EMPTY");

        var assemblyModel = new AssemblyModel
        {
            Name = splt[0],
            Version = splt[1],
            Architecture = splt[2],
            Hash = isDynamic || isEmpty ? string.Empty : splt[3],
            Path = isDynamic ? "DYNAMIC" : isEmpty ? "EMPTY" : splt[4],

            FullName = $"{splt[0]}, Version={splt[1]}",

            Type = (outerHtml.Contains("dynamic_assembly") ? AssemblyModelType.Dynamic
                : outerHtml.Contains("gac_assembly") ? AssemblyModelType.GAC
                : outerHtml.Contains("tw_assembly") ? AssemblyModelType.GameCore
                : outerHtml.Contains("tw_module_assembly") ? AssemblyModelType.GameModule
                : outerHtml.Contains("module_assembly") ? AssemblyModelType.Module
                : AssemblyModelType.Unclassified) | (isDynamic ? AssemblyModelType.Dynamic : AssemblyModelType.Unclassified),

            AdditionalMetadata = ImmutableArray<MetadataModel>.Empty,
        };
        return assemblyModel;
    }

    private static HarmonyPatchesModel ParseHarmonyPatch(HtmlNode node)
    {
        static HarmonyPatchModel ParsePatch(HtmlNode node, HarmonyPatchModelType type)
        {
            var split = node.InnerText.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            return new HarmonyPatchModel
            {
                Type = type,
                Owner = split.FirstOrDefault(x => x.StartsWith("Owner: "))?.Split(':')[1] ?? string.Empty,
                Namespace = split.FirstOrDefault(x => x.StartsWith("Namespace: "))?.Split(':')[1] ?? string.Empty,
                Index = split.FirstOrDefault(x => x.StartsWith("Index: "))?.Split(':')[1] is { } strIndex && int.TryParse(strIndex, out var index) ? index : 0,
                Priority = split.FirstOrDefault(x => x.StartsWith("Priority: "))?.Split(':')[1] is { } strPriority && int.TryParse(strPriority, out var piority) ? piority : 400,
                Before = split.FirstOrDefault(x => x.StartsWith("Before: "))?.Split(':')[1].Split(',') ?? Array.Empty<string>(),
                After = split.FirstOrDefault(x => x.StartsWith("After: "))?.Split(':')[1].Split(',') ?? Array.Empty<string>(),
                AdditionalMetadata = ImmutableArray<MetadataModel>.Empty,
            };
        }

        var originalMethodFullName = node.ChildNodes.Skip(0).First().InnerText.Trim('\n');
        var prefixes = node.ChildNodes.FirstOrDefault(x => x.InnerText?.Contains("Prefixes") == true);
        var postfixes = node.ChildNodes.FirstOrDefault(x => x.InnerText?.Contains("Postfixes") == true);
        var transpilers = node.ChildNodes.FirstOrDefault(x => x.InnerText?.Contains("Transpilers") == true);
        var finalizers = node.ChildNodes.FirstOrDefault(x => x.InnerText?.Contains("Finalizers") == true);
        var harmonyPatchModel = new HarmonyPatchesModel
        {
            OriginalMethod = originalMethodFullName.Split('.').Last(),
            OriginalMethodFullName = originalMethodFullName,
            Prefixes = prefixes?.SelectSingleNode("descendant::ul/li")?.ChildNodes.Select(x => ParsePatch(x, HarmonyPatchModelType.Prefix)).ToArray() ?? Array.Empty<HarmonyPatchModel>(),
            Postfixes = postfixes?.SelectSingleNode("descendant::ul/li")?.ChildNodes.Select(x => ParsePatch(x, HarmonyPatchModelType.Postfix)).ToArray() ?? Array.Empty<HarmonyPatchModel>(),
            Transpilers = transpilers?.SelectSingleNode("descendant::ul/li")?.ChildNodes.Select(x => ParsePatch(x, HarmonyPatchModelType.Transpiler)).ToArray() ?? Array.Empty<HarmonyPatchModel>(),
            Finalizers = finalizers?.SelectSingleNode("descendant::ul/li")?.ChildNodes.Select(x => ParsePatch(x, HarmonyPatchModelType.Finalizer)).ToArray() ?? Array.Empty<HarmonyPatchModel>(),
            AdditionalMetadata = ImmutableArray<MetadataModel>.Empty,
        };
        return harmonyPatchModel;
    }
}