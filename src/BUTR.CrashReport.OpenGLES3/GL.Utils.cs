using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace OpenGLES3;

/// <summary>
/// the methods here are just convenience wrappers for calling the raw gl* method
/// </summary>
unsafe partial class GL
{
    public uint GenBuffer()
    {
        var buffer = 0U;
        GenBuffers(1, &buffer);
        return buffer;
    }

    public void DeleteBuffer(uint buffer)
    {
        DeleteBuffers(1, &buffer);
    }

    public string GetShaderInfoLogUtf16(uint shader)
    {
        var infoLogLength = 0;
        GetShaderIV(shader, ShaderParameter.InfoLogLength, &infoLogLength);
        if (infoLogLength == 0)
            return string.Empty;

        var infoLogPtr = stackalloc byte[infoLogLength];
        var length = 0U;
        GetShaderInfoLog(shader, (uint) infoLogLength, &length, infoLogPtr);
        return Encoding.UTF8.GetString(infoLogPtr, (int) length);
    }

    public void ShaderSourceUtf8(uint shader, ReadOnlySpan<byte> source)
    {
        fixed (byte* sourcePtr = source)
        {
            var sources = stackalloc byte*[1] { sourcePtr };
            var length = stackalloc int[1] { source.Length };
            ShaderSource(shader, 1, (IntPtr) sources, length);
        }
    }

    public bool GetShaderCompileStatus(uint shader)
    {
        var compileStatus = 0;
        GetShaderIV(shader, ShaderParameter.CompileStatus, &compileStatus);
        return Unsafe.As<int, bool>(ref compileStatus);
    }

    public string GetProgramInfoLogUtf16(uint program)
    {
        var infoLogLength = 0;
        GetProgramiv(program, ProgramParameter.InfoLogLength, &infoLogLength);
        if (infoLogLength == 0)
            return string.Empty;

        var infoLogPtr = stackalloc byte[infoLogLength];
        var length = 0U;
        GetProgramInfoLog(program, (uint) infoLogLength, &length, infoLogPtr);
        return Encoding.UTF8.GetString(infoLogPtr, (int) length);
    }

    public bool GetProgramLinkStatus(uint program)
    {
        var linkStatus = 0;
        GetProgramiv(program, ProgramParameter.LinkStatus, &linkStatus);
        return Unsafe.As<int, bool>(ref linkStatus);
    }

    public void UniformMatrix4(int location, ref readonly Matrix4x4 value)
    {
        var data = stackalloc float[16]
        {
            value.M11, value.M12, value.M13, value.M14,
            value.M21, value.M22, value.M23, value.M24,
            value.M31, value.M32, value.M33, value.M34,
            value.M41, value.M42, value.M43, value.M44,
        };
        UniformMatrix4FV(location, 1, 0, data);
    }

    public void VertexAttribPointerBool(int index, int size, VertexAttribPointerType type, bool normalized, uint stride, void* pointer)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index));
        VertexAttribPointer((uint) index, size, type, Unsafe.As<bool, byte>(ref normalized), stride, pointer);
    }

    public uint GenVertexArray()
    {
        var array = 0U;
        GenVertexArrays(1, &array);
        return array;
    }

    public void DeleteVertexArray(uint vao)
    {
        DeleteVertexArrays(1, &vao);
    }

    public uint GenTexture()
    {
        var texture = 0U;
        GenTextures(1, &texture);
        return texture;
    }

    public void DeleteTexture(uint texture)
    {
        DeleteTextures(1, &texture);
    }

    [Conditional("DEBUG")]
    public void CheckError(string title = "", [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
    {
        var error = GetError();
        while (error != GL_NO_ERROR)
        {
            Debug.Print($"{filePath} {memberName}:{lineNumber - 1} - [{title}: {error}]");
            error = GetError();
        }
    }
}