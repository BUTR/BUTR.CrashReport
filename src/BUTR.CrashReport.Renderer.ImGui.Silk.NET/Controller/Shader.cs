// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using BUTR.CrashReport.Renderer.ImGui.Implementation.CImGui.Utils;

using Silk.NET.OpenGL;

using System.Runtime.CompilerServices;

namespace BUTR.CrashReport.Renderer.ImGui.Implementation.CImGui.Controller;

internal class Shader
{
    private readonly GL _gl;

    private bool _initialized;

    public uint Program { get; private set; }

    public Shader(GL gl, ReadOnlySpan<byte> vertexShaderUtf8, ReadOnlySpan<byte> fragmentShaderUtf8)
    {
        _gl = gl;
        Program = CreateProgram(vertexShaderUtf8, fragmentShaderUtf8);
    }
    public void UseShader()
    {
        _gl.UseProgram(Program);
        _gl.CheckGlError();
    }

    public void Dispose()
    {
        if (_initialized)
        {
            _gl.DeleteProgram(Program);
            _gl.CheckGlError();
            _initialized = false;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetUniformLocation(ReadOnlySpan<byte> uniformUtf8)
    {
        var result = _gl.GetUniformLocation(Program, uniformUtf8);
        _gl.CheckGlError();
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetAttribLocation(ReadOnlySpan<byte> attribUtf8)
    {
        var result = _gl.GetAttribLocation(Program, attribUtf8);
        _gl.CheckGlError();
        return result;
    }

    private uint CreateProgram(ReadOnlySpan<byte> vertexShaderUtf8, ReadOnlySpan<byte> fragmentShaderUtf8)
    {
        var program = _gl.CreateProgram();
        _gl.CheckGlError();

        var vertexShader = CompileShader(GLEnum.VertexShader, vertexShaderUtf8);
        var fragmentShader = CompileShader(GLEnum.FragmentShader, fragmentShaderUtf8);

        _gl.AttachShader(program, vertexShader);
        _gl.CheckGlError();
        _gl.AttachShader(program, fragmentShader);
        _gl.CheckGlError();
        _gl.LinkProgram(program);
        _gl.CheckGlError();

        CheckProgram(program);

        _gl.DetachShader(program, vertexShader);
        _gl.CheckGlError();
        _gl.DetachShader(program, fragmentShader);
        _gl.CheckGlError();
        _gl.DeleteShader(vertexShader);
        _gl.CheckGlError();
        _gl.DeleteShader(fragmentShader);
        _gl.CheckGlError();

        _initialized = true;

        return program;
    }

    private unsafe uint CompileShader(GLEnum type, ReadOnlySpan<byte> sourceUtf8)
    {
        var shader = _gl.CreateShader(type);
        _gl.CheckGlError();
        var shaderData = stackalloc Utf8ZPtr[]
        {
            new Utf8ZPtr(in sourceUtf8),
        };
        Span<int> shaderDataLength = stackalloc int[]
        {
            sourceUtf8.Length,
        };
        _gl.ShaderSource(shader, (byte**) shaderData, shaderDataLength);
        _gl.CheckGlError();
        _gl.CompileShader(shader);
        _gl.CheckGlError();
        CheckShader(shader);

        return shader;
    }

    private void CheckShader(uint handle)
    {
        Span<int> status = stackalloc int[1];
        _gl.GetShader(handle, GLEnum.CompileStatus, status);
        if (status[0] != (int) GLEnum.False) return;

        var info = _gl.GetShaderInfoLog(handle);
        //Debug.WriteLine($"GL.CompileShader for shader [{type}] had info log:\n{info}");
        throw new Exception($"OpenGL Shader: {info}");
    }

    private void CheckProgram(uint handle)
    {
        Span<int> status = stackalloc int[1];
        _gl.GetProgram(handle, GLEnum.LinkStatus, status);
        if (status[0] != (int) GLEnum.False) return;

        var info = _gl.GetProgramInfoLog(handle);
        throw new Exception($"OpenGL Program: {info}");
    }
}