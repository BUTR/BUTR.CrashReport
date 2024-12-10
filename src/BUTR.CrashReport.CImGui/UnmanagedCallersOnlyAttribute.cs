#if NETSTANDARD2_0
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable once CheckNamespace
namespace System.Runtime.InteropServices;

[ExcludeFromCodeCoverage]
[DebuggerNonUserCode]
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
internal sealed class UnmanagedCallersOnlyAttribute : Attribute
{
    /// <summary>
    /// Optional. If omitted, the runtime will use the default platform calling convention.
    /// </summary>
    /// <remarks>
    /// Supplied types must be from the official "System.Runtime.CompilerServices" namespace and
    /// be of the form "CallConvXXX".
    /// </remarks>
    public Type[]? CallConvs;

    /// <summary>
    /// Optional. If omitted, no named export is emitted during compilation.
    /// </summary>
    public string? EntryPoint;
}
#endif