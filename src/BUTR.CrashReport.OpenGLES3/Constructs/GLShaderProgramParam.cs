using System.Numerics;

namespace OpenGLES3;

using static GL;

public enum ParamType
{
    Uniform,
    Attribute,
}

public sealed class GLShaderProgramParam
{
    private readonly GL _gl;

    /// <summary>
    /// Specifies the OpenGL program ID.
    /// </summary>
    public uint ProgramId;

    /// <summary>
    /// Specifies the location of the parameter in the OpenGL program.
    /// </summary>
    public int Location;

    /// <summary>
    /// Specifies the C# equivalent of the GLSL data type.
    /// </summary>
    public readonly Type Type;

    /// <summary>
    /// Specifies the parameter type (either attribute or uniform).
    /// </summary>
    public readonly ParamType ParamType;

    /// <summary>
    /// Specifies the case-sensitive name of the parameter.
    /// </summary>
    public readonly string Name;

    public GLShaderProgramParam(GL gl, Type type, ParamType paramType, string name)
    {
        _gl = gl;
        Type = type;
        ParamType = paramType;
        Name = name;
    }

    public GLShaderProgramParam(GL gl, Type type, ParamType paramType, string name, uint program, int location) : this(gl, type, paramType, name)
    {
        ProgramId = program;
        Location = location;
    }

    /// <summary>
    /// Gets the location of the parameter in a compiled OpenGL program.
    /// </summary>
    /// <param name="program">Specifies the shader program that contains this parameter.</param>
    public void GetLocation(GLShaderProgram program)
    {
        program.Use();
        if (ProgramId == 0)
        {
            ProgramId = program.ProgramID;
            Location = ParamType == ParamType.Uniform ? program.GetUniformLocation(Name) : program.GetAttributeLocation(Name);
        }
    }

    public void SetValue(bool param)
    {
        _gl.Uniform1I(Location, param ? 1 : 0);
    }

    public void SetValue(int param)
    {
        _gl.Uniform1I(Location, param);
    }

    public void SetValue(float param)
    {
        _gl.Uniform1F(Location, param);
    }

    public void SetValue(ref readonly Vector2 param)
    {
        _gl.Uniform2F(Location, param.X, param.Y);
    }

    public void SetValue(ref readonly Vector3 param)
    {
        _gl.Uniform3F(Location, param.X, param.Y, param.Z);
    }

    public void SetValue(ref readonly Vector4 param)
    {
        _gl.Uniform4F(Location, param.X, param.Y, param.Z, param.W);
    }

    public void SetValue(ref readonly Matrix4x4 param)
    {
        _gl.UniformMatrix4(Location, in param);
    }
}