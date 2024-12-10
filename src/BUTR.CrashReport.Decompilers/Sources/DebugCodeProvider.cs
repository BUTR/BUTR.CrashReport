using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BUTR.CrashReport.Decompilers.Sources;

internal static class DebugCodeProvider
{
    private static readonly Guid EmbeddedSource = new("0E8A571B-6926-466E-B4AD-8AB04611F5FE");
    private static readonly Guid SourceLink = new("CC110556-A091-4D38-9FEC-25AB9A351A6A");

    private static readonly Encoding DefaultEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: false);

    private static string Decode(MemoryStream stream, Encoding? encoding)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        encoding ??= DefaultEncoding;
        stream.Position = 0;

        using var reader = new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks: true);
        var text = reader.ReadToEnd();
        return text;
    }

    private static IEnumerable<SourceLocation> GetSourceLocation(MetadataReader metadataReader, uint methodToken)
    {
        var methodDebugInformationHandle = MetadataTokens.MethodDebugInformationHandle((int) methodToken);
        var methodDebugInformation = metadataReader.GetMethodDebugInformation(methodDebugInformationHandle);
        var documentHandle = methodDebugInformation.Document;

        if (documentHandle.IsNil)
            yield break;

        var document = metadataReader.GetDocument(documentHandle);
        var documentName = metadataReader.GetString(document.Name);

        var sequencePoints = new List<SourceSequencePoint>();
        var previousSequencePoint = default(SequencePoint);
        foreach (var sequencePoint in methodDebugInformation.GetSequencePoints().Reverse())
        {
            if (previousSequencePoint.Equals(default))
                previousSequencePoint = sequencePoint;

            sequencePoints.Add(new SourceSequencePoint(sequencePoint.Offset, previousSequencePoint.Offset, sequencePoint.StartLine, sequencePoint.StartColumn, sequencePoint.EndLine, sequencePoint.EndColumn, sequencePoint.IsHidden));
            previousSequencePoint = sequencePoint;
        }
        sequencePoints.Reverse();

        foreach (var customDebugInformationHandle in metadataReader.CustomDebugInformation)
        {
            var customDebugInformation = metadataReader.GetCustomDebugInformation(customDebugInformationHandle);
            var guid = metadataReader.GetGuid(customDebugInformation.Kind);

            if (guid == EmbeddedSource && customDebugInformation.Parent == documentHandle)
            {
                if (GetEmbeddedSource(metadataReader, customDebugInformation) is { } sourceCodeString)
                    yield return new SourceLocation(new SourceFileEmbedded(documentName, sourceCodeString), sequencePoints);
            }

            if (guid == SourceLink)
            {
                var sourceLink = JsonSerializer.Deserialize<SourceLink>(metadataReader.GetBlobBytes(customDebugInformation.Value));
                if (sourceLink?.GetSourceUrl(documentName) is { } sourceLinkUrl)
                    yield return new SourceLocation(new SourceFileSourceLink(documentName, sourceLinkUrl), sequencePoints);
            }
        }
    }

    private static string GetEmbeddedSource(MetadataReader reader, CustomDebugInformation customDebugInformation)
    {
        var bytes = reader.GetBlobBytes(customDebugInformation.Value);

        var uncompressedSize = BitConverter.ToInt32(bytes, 0);
        var stream = new MemoryStream(bytes, sizeof(int), bytes.Length - sizeof(int));

        if (uncompressedSize != 0)
        {
            var decompressed = new MemoryStream(uncompressedSize);

            using (var deflater = new DeflateStream(stream, CompressionMode.Decompress))
            {
                deflater.CopyTo(decompressed);
            }

            if (decompressed.Length != uncompressedSize)
            {
                throw new InvalidDataException();
            }

            stream = decompressed;
        }

        using (stream)
        {
            return Decode(stream, DefaultEncoding);
        }
    }

    public static List<SourceLocation> GetCSharpSourceFromPdb(MethodBase? method, Stream pdbStream, bool disposeStream)
    {
        if (method is null) return [];

        var pdbMeta = MetadataReaderProvider.FromPortablePdbStream(pdbStream, disposeStream ? MetadataStreamOptions.Default : MetadataStreamOptions.LeaveOpen);
        var pdbReader = pdbMeta.GetMetadataReader();
        return GetSourceLocation(pdbReader, (uint) method.MetadataToken).ToList();
    }

    public static List<SourceLocation> GetCSharpSourcesFromAssembly(MethodBase? method, Stream assemblyStream, bool disposeStream)
    {
        if (method is null) return [];

        using var peReader = new PEReader(assemblyStream, disposeStream ? PEStreamOptions.Default : PEStreamOptions.LeaveOpen);
        var embeddedEntries = peReader.ReadDebugDirectory().Where(x => x.Type == DebugDirectoryEntryType.EmbeddedPortablePdb).ToList();
        if (embeddedEntries.Count != 1) return [];

        using var provider = peReader.ReadEmbeddedPortablePdbDebugDirectoryData(embeddedEntries.Single());
        var pdbReader = provider.GetMetadataReader();
        return GetSourceLocation(pdbReader, (uint) method.MetadataToken).ToList();
    }
}

file class SourceLink
{
    public static Guid EmbeddedSourceId { get; } = new("0E8A571B-6926-466E-B4AD-8AB04611F5FE");
    public static Guid SourceLinkId { get; } = new("CC110556-A091-4D38-9FEC-25AB9A351A6A");

    [JsonPropertyName("documents")]
    public Dictionary<string, string>? Documents { get; set; }

    public string? GetSourceUrl(string documentName)
    {
        foreach (var (docPath, urlTemplate) in Documents ?? [])
        {
            // Check if the document path has a wildcard '*'
            if (docPath.Contains("*"))
            {
                // Ensure that the wildcard is the final character
                if (docPath.IndexOf("*", StringComparison.Ordinal) != docPath.Length - 1)
                    return null;
                // throw new ArgumentException("Wildcard '*' must be the final character in the file path.");

                // Ensure that the URL template also contains a '*'
                if (!urlTemplate.Contains("*"))
                    return null;
                // throw new ArgumentException("If the file path contains '*', the URL must also contain '*'.");

                // Remove the wildcard from the document path to get the base path
                var basePath = docPath.TrimEnd('*');

                // Check if the file path starts with the base path
                if (documentName.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
                {
                    // Get the relative path part after the base path
                    var relativePath = documentName.Substring(basePath.Length);

                    // Replace '*' in the URL template with the relative path
                    return urlTemplate.Replace("*", relativePath);
                }
            }
            else
            {
                // No wildcard in the document path; check for an exact match
                if (string.Equals(docPath, documentName, StringComparison.OrdinalIgnoreCase))
                {
                    // URL template should not contain a '*'
                    if (urlTemplate.Contains("*"))
                        return null;
                    // throw new ArgumentException("If the file path does not contain '*', the URL cannot contain '*'.");

                    return urlTemplate;
                }
            }
        }

        // If no match is found, return null or throw an exception
        return null;
    }
}