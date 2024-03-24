/*
// TODO: Investigate whether we can read the source mappings if they are available remotely?
// At least save the mapping and then use it online?
using BUTR.CrashReport.ILSpy;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading;

namespace BUTR.CrashReport.Decompilers.Utils;

partial class MethodDecompiler
{
    public static string[] DecompileILWithCSharpCode3(MethodBase? method)
    {
        if (method is null) return Array.Empty<string>();

        var assembly = method.Module.Assembly;
        if (assembly.IsDynamic) return Array.Empty<string>();

        using var stream = File.OpenRead(assembly.Location);
        using var peReader = new PEReader(stream);

        try
        {
            var embeddedEntries = peReader.ReadDebugDirectory().Where(x => x.Type == DebugDirectoryEntryType.EmbeddedPortablePdb);
            using var provider = peReader.ReadEmbeddedPortablePdbDebugDirectoryData(embeddedEntries.Single());
            var pdbReader = provider.GetMetadataReader();


            var definition = MetadataTokens.MethodDefinitionHandle(method.MetadataToken);
            foreach (var methodDebugInfoHandle in pdbReader.MethodDebugInformation)
            {
                if (methodDebugInfoHandle.ToDefinitionHandle() != definition) continue;

                var debugInfo = pdbReader.GetMethodDebugInformation(methodDebugInfoHandle);
                foreach (var sequencePoint in debugInfo.GetSequencePoints())
                {
                    var document = pdbReader.GetDocument(sequencePoint.Document);
                    var fileName = GetDocumentName(pdbReader, document.Name);
                }
            }
        }
        catch (Exception e)
        {
            return new[] { e.ToString() };
        }

        return Array.Empty<string>();
    }

    private static string GetDocumentName(MetadataReader reader, DocumentNameBlobHandle handle)
    {
        var blobReader = reader.GetBlobReader(handle);
        var separator = (char) blobReader.ReadByte();
        string name = "";
        while (blobReader.Offset < blobReader.Length)
        {
            var partHandle = blobReader.ReadBlobHandle();
            if (name.Length != 0)
                name += separator;
            name += Encoding.ASCII.GetString(reader.GetBlobBytes(partHandle));
        }
        return name;
    }
}
*/