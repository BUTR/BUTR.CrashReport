// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Silk.NET.OpenGL;

using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BUTR.CrashReport.Renderer.ImGui.Implementation.CImGui.Controller;

internal static class Util
{
    [Pure]
    public static float Clamp(float value, float min, float max) => value < min ? min : value > max ? max : value;

    [Conditional("DEBUG")]
    public static void CheckGlError(this GL gl, string title = "", [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
    {
        var error = gl.GetError();
        while (error != GLEnum.NoError)
        {
            Debug.Print($"{filePath} {memberName}:{lineNumber - 1} - [{title}: {error}]");
            error = gl.GetError();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void UniformMatrix4x4(this GL gl, int uniformLocation, ref readonly Matrix4x4 value)
    {
        var data = stackalloc float[16]
        {
            value.M11, value.M12, value.M13, value.M14,
            value.M21, value.M22, value.M23, value.M24,
            value.M31, value.M32, value.M33, value.M34,
            value.M41, value.M42, value.M43, value.M44,
        };
        gl.UniformMatrix4(uniformLocation, 1, false, data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void VertexAttribPointer2(this GL gl, uint index, int size, GLEnum type, bool normalized, uint stride, IntPtr pointer) =>
        gl.VertexAttribPointer(index, size, type, normalized, stride, pointer.ToPointer());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void BufferData<T>(this GL gl, GLEnum target, Span<T> span, GLEnum usage) where T : unmanaged
    {
        fixed (T* ptr = span)
            gl.BufferData(target, (nuint) (span.Length * Unsafe.SizeOf<T>()), ptr, usage);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void DrawElementsBaseVertex2(this GL gl, GLEnum mode, uint count, GLEnum type, IntPtr indices, int basevertex) =>
        gl.DrawElementsBaseVertex(mode, count, type, indices.ToPointer(), basevertex);
}