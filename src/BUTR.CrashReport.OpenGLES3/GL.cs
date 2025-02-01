using BUTR.CrashReport.Memory.Utils;
using BUTR.CrashReport.Native;

using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

[assembly: DelegateLoader(typeof(OpenGLES3.GL), true)]

namespace OpenGLES3;

using IntPtrActiveAttribType = Pointer<GL.ActiveAttribType>;
using IntPtrActiveUniformType = Pointer<GL.ActiveUniformType>;
using IntPtrGLchar = Pointer<GLchar>;
using IntPtrGLfloat = Pointer<GLfloat>;
using IntPtrGLint = Pointer<GLint>;
using IntPtrGLuint = Pointer<GLuint>;
using IntPtrIntPtrGLchar = Pointer<Pointer<GLchar>>;

public partial class GL
{
    public readonly IntPtr Context;
    private readonly Func<IntPtrByte, IntPtrVoid> _getFunctionPointer;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glGenBuffers(GLsizei n, IntPtrGLuint buffers);
    public glGenBuffers GenBuffers = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glDeleteBuffers(GLsizei n, IntPtrGLuint buffers);
    public glDeleteBuffers DeleteBuffers = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glViewport(GLint x, GLint y, GLsizei width, GLsizei height);
    public glViewport Viewport = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glClearColor(GLfloat red, GLfloat green, GLfloat blue, GLfloat alpha);
    public glClearColor ClearColor = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glClear(ClearBufferMask mask);
    public glClear Clear = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glEnable(EnableCap cap);
    public glEnable Enable = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glDisable(EnableCap cap);
    public glDisable Disable = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glBlendEquation(BlendEquationMode mode);
    public glBlendEquation BlendEquation = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glUseProgram(GLuint program);
    public glUseProgram UseProgram = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glGetShaderiv(GLuint shader, ShaderParameter pname, IntPtrGLint param);
    public glGetShaderiv GetShaderIV = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glGetShaderInfoLog(GLuint shader, GLsizei bufSize, IntPtrGLuint length, IntPtrGLchar infoLog);
    public glGetShaderInfoLog GetShaderInfoLog = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate GLuint glCreateShader(ShaderType type);
    public glCreateShader CreateShader = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glShaderSource(GLuint shader, GLsizei count, IntPtrIntPtrGLchar sources, IntPtrGLint length);
    public glShaderSource ShaderSource = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glCompileShader(GLuint shader);
    public glCompileShader CompileShader = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glDeleteShader(GLuint shader);
    public glDeleteShader DeleteShader = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glGetProgramiv(GLuint program, ProgramParameter pname, IntPtrGLint param);
    public glGetProgramiv GetProgramiv = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glGetProgramInfoLog(GLuint program, GLsizei bufSize, IntPtrGLuint length, IntPtrGLchar infoLog);
    public glGetProgramInfoLog GetProgramInfoLog = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate GLuint glCreateProgram();
    public glCreateProgram CreateProgram = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glAttachShader(GLuint program, GLuint shader);
    public glAttachShader AttachShader = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glLinkProgram(GLuint program);
    public glLinkProgram LinkProgram = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate GLint glGetUniformLocation(GLuint program, IntPtrGLchar name);
    public glGetUniformLocation GetUniformLocation = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate GLint glGetAttribLocation(GLuint program, IntPtrGLchar name);
    public glGetAttribLocation GetAttribLocation = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glDetachShader(GLuint program, GLuint shader);
    public glDetachShader DetachShader = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glDeleteProgram(GLuint program);
    public glDeleteProgram DeleteProgram = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glGetActiveAttrib(GLuint program, GLuint index, GLsizei bufSize, IntPtrGLuint length, IntPtrGLint size, IntPtrActiveAttribType type, IntPtrGLchar name);
    public glGetActiveAttrib GetActiveAttrib = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glGetActiveUniform(GLuint program, GLuint index, GLsizei bufSize, IntPtrGLuint length, IntPtrGLint size, IntPtrActiveUniformType type, IntPtrGLchar name);
    public glGetActiveUniform GetActiveUniform = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glUniform1f(GLint location, GLfloat v0);
    public glUniform1f Uniform1F = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glUniform2f(GLint location, GLfloat v0, GLfloat v1);
    public glUniform2f Uniform2F = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glUniform3f(GLint location, GLfloat v0, GLfloat v1, GLfloat v2);
    public glUniform3f Uniform3F = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glUniform4f(GLint location, GLfloat v0, GLfloat v1, GLfloat v2, GLfloat v3);
    public glUniform4f Uniform4F = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glUniform1i(GLint location, GLint v0);
    public glUniform1i Uniform1I = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glUniformMatrix4fv(GLint location, GLsizei count, GLboolean transpose, IntPtrGLfloat value);
    public glUniformMatrix4fv UniformMatrix4FV = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glBindVertexArray(GLuint array);
    public glBindVertexArray BindVertexArray = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glBindBuffer(BufferTarget target, GLuint buffer);
    public glBindBuffer BindBuffer = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glEnableVertexAttribArray(GLuint index);
    public glEnableVertexAttribArray EnableVertexAttribArray = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glVertexAttribPointer(GLuint index, GLint size, VertexAttribPointerType type, GLboolean normalized, GLsizei stride, IntPtrVoid pointer);
    public glVertexAttribPointer VertexAttribPointer = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glBindTexture(TextureTarget target, GLuint texture);
    public glBindTexture BindTexture = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glBufferData(BufferTarget target, GLsizeiptr size, IntPtrVoid data, BufferUsageHint usage);
    public glBufferData BufferData = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glScissor(GLint x, GLint y, GLsizei width, GLsizei height);
    public glScissor Scissor = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glDeleteVertexArrays(GLsizei n, IntPtrGLuint arrays);
    public glDeleteVertexArrays DeleteVertexArrays = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glGenVertexArrays(GLsizei n, IntPtrGLuint arrays);
    public glGenVertexArrays GenVertexArrays = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glGenTextures(GLsizei n, IntPtrGLuint textures);
    public glGenTextures GenTextures = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glPixelStorei(PixelStoreParameter pname, GLint param);
    public glPixelStorei PixelStoreI = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glTexParameteri(TextureTarget target, TextureParameterName pname, TextureParameter param);
    public glTexParameteri TexParameterI = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glDeleteTextures(GLsizei n, IntPtrGLuint textures);
    public glDeleteTextures DeleteTextures = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glActiveTexture(TextureUnit texture);
    public glActiveTexture ActiveTexture = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glDrawElements(PrimitiveType mode, GLsizei count, DrawElementsType type, IntPtrVoid indices);
    public glDrawElements DrawElements = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glBindSampler(GLuint unit, GLuint sampler);
    public glBindSampler BindSampler = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glBlendFuncSeparate(BlendingFactorDest sfactorRGB, BlendingFactorDest dfactorRGB, BlendingFactorDest sfactorAlpha, BlendingFactorDest dfactorAlpha);
    public glBlendFuncSeparate BlendFuncSeparate = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate GLenum glGetError();
    public glGetError GetError = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void glTexImage2D(TextureTarget target, GLint level, PixelInternalFormat internalformat, GLsizei width, GLsizei height, GLint border, PixelFormat format, PixelType type, IntPtrVoid data);
    public glTexImage2D TexImage2D = null!;

    public GL(IntPtr context, Func<IntPtrByte, IntPtrVoid> getFunctionPointer)
    {
        Context = context;
        _getFunctionPointer = getFunctionPointer;

        this.LoadFrom(LoadFunctionPointer);
    }

    private unsafe IntPtr LoadFunctionPointer(string utf16FunctionName)
    {
        var utf8ByteCount = Encoding.UTF8.GetMaxByteCount(utf16FunctionName.Length) + 1;
        var tempMemory = utf8ByteCount > 2048 ? MemoryPool<byte>.Shared.Rent(utf8ByteCount) : null;
        var utf8 = utf8ByteCount <= 2048 ? stackalloc byte[utf8ByteCount] : tempMemory!.Memory.Span;
        var length = Utf8Utils.Utf16ToUtf8(utf16FunctionName, utf8);
        utf8[length] = 0;

        fixed (byte* utf8Ptr = utf8)
        {
            var ptr = (IntPtr) _getFunctionPointer((IntPtr) utf8Ptr);

            tempMemory?.Dispose();

            if (ptr == IntPtr.Zero)
                throw new InvalidOperationException($"Failed to load function pointer for {utf16FunctionName}");

            return ptr;
        }
    }
}