namespace OpenGLES3;

using static GL;

public sealed class GLShader : IDisposable
{
    private readonly GL _gl;

    // Specifies the OpenGL ShaderID.
    public uint ShaderID { get; private set; }

    // Specifies the type of shader.
    public ShaderType ShaderType { get; private set; }

    // Returns Gl.GetShaderInfoLog(ShaderID), which contains any compilation errors.
    public string ShaderLog => _gl.GetShaderInfoLogUtf16(ShaderID);

    ~GLShader() => Dispose(false);

    public GLShader(GL gl, ReadOnlySpan<byte> source, ShaderType type)
    {
        _gl = gl;
        ShaderType = type;
        ShaderID = _gl.CreateShader(type);

        gl.ShaderSourceUtf8(ShaderID, source);
        gl.CompileShader(ShaderID);

        if (!_gl.GetShaderCompileStatus(ShaderID))
        {
            throw new Exception(ShaderLog);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (ShaderID != 0)
        {
            _gl.DeleteShader(ShaderID);
            ShaderID = 0;
        }
    }
}