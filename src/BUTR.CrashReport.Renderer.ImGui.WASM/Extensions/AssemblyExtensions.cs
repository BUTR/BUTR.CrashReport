using System.Reflection;

namespace BUTR.CrashReport.Renderer.ImGui.WASM.Extensions;

internal static class AssemblyExtensions
{
    public static unsafe ReadOnlySpan<byte> GetManifestResourceStreamAsSpan(this Assembly a, string name)
    {
        var stream = (UnmanagedMemoryStream) a.GetManifestResourceStream(name)!;
        return new ReadOnlySpan<byte>(stream.PositionPointer, checked((int) stream.Length));
    }
}