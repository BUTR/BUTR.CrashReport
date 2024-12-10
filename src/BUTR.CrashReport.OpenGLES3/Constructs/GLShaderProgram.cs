using BUTR.CrashReport.Memory.Utils;

using System.Numerics;
using System.Text;

namespace OpenGLES3;

using static GL;

public sealed unsafe class GLShaderProgram : IDisposable
{
    private readonly GL _gl;

    /// <summary>
    /// Specifies the OpenGL shader program ID.
    /// </summary>
    public readonly uint ProgramID;

    /// <summary>
    /// Specifies the vertex shader used in this program.
    /// </summary>
    public readonly GLShader VertexShader;

    /// <summary>
    /// Specifies the fragment shader used in this program.
    /// </summary>
    public readonly GLShader FragmentShader;

    /// <summary>
    /// Specifies whether this program will dispose of the child
    /// vertex/fragment programs when the IDisposable method is called.
    /// </summary>
    public readonly bool DisposeChildren;

    private readonly Dictionary<string, GLShaderProgramParam> shaderParams = new();

    /// <summary>
    /// Queries the shader parameter hashtable to find a matching attribute/uniform.
    /// </summary>
    /// <param name="name">Specifies the case-sensitive name of the shader attribute/uniform.</param>
    /// <returns>The requested attribute/uniform, or null on a failure.</returns>
    public GLShaderProgramParam? this[string name] => shaderParams.TryGetValue(name, out var param) ? param : null;

    public string ProgramLog => _gl.GetProgramInfoLogUtf16(ProgramID);

    /// <summary>
    /// Links a vertex and fragment shader together to create a shader program.
    /// </summary>
    /// <param name="vertexShader">Specifies the vertex shader.</param>
    /// <param name="fragmentShader">Specifies the fragment shader.</param>
    public GLShaderProgram(GL gl, GLShader vertexShader, GLShader fragmentShader)
    {
        _gl = gl;
        VertexShader = vertexShader;
        FragmentShader = fragmentShader;
        ProgramID = _gl.CreateProgram();
        DisposeChildren = false;

        _gl.AttachShader(ProgramID, vertexShader.ShaderID);
        _gl.CheckError();
        _gl.AttachShader(ProgramID, fragmentShader.ShaderID);
        _gl.CheckError();
        _gl.LinkProgram(ProgramID);
        _gl.CheckError();

        //Check whether the program linked successfully.
        //If not then throw an error with the linking error.
        if (!_gl.GetProgramLinkStatus(ProgramID))
            throw new Exception(ProgramLog);

        GetParams();
    }

    /// <summary>
    /// Creates two shaders and then links them together to create a shader program.
    /// </summary>
    /// <param name="vertexShaderSource">Specifies the source code of the vertex shader.</param>
    /// <param name="fragmentShaderSource">Specifies the source code of the fragment shader.</param>
    public GLShaderProgram(GL gl, ReadOnlySpan<byte> vertexShaderSource, ReadOnlySpan<byte> fragmentShaderSource)
        : this(gl, new GLShader(gl, vertexShaderSource, ShaderType.VertexShader), new GLShader(gl, fragmentShaderSource, ShaderType.FragmentShader))
    {
        DisposeChildren = true;
    }

    /// <summary>
    /// Parses all of the parameters (attributes/uniforms) from the two attached shaders
    /// and then loads their location by passing this shader program into the parameter object.
    /// </summary>
    private void GetParams()
    {
        shaderParams.Clear();

        var resources = stackalloc int[1];
        var actualLength = stackalloc uint[1];
        var arraySize = stackalloc int[1];
        var type1 = stackalloc ActiveAttribType[1];
        var type2 = stackalloc ActiveUniformType[1];
        var sb = stackalloc byte[256];

        _gl.GetProgramiv(ProgramID, ProgramParameter.ActiveAttributes, resources);
        _gl.CheckError();

        for (uint i = 0; i < resources[0]; i++)
        {
            _gl.GetActiveAttrib(ProgramID, i, 256, actualLength, arraySize, type1, sb);
            var nameUtf8 = new Span<byte>(sb, (int) actualLength[0]);
            var name = Utf8Utils.Utf8ToUtf16(nameUtf8);
            _gl.CheckError();

            if (!shaderParams.ContainsKey(name))
            {
                var param = new GLShaderProgramParam(_gl, TypeFromAttributeType(type1[0]), ParamType.Attribute, name);
                shaderParams.Add(param.Name, param);
                param.GetLocation(this);
            }
        }

        _gl.GetProgramiv(ProgramID, ProgramParameter.ActiveUniforms, resources);
        _gl.CheckError();

        for (uint i = 0; i < resources[0]; i++)
        {
            _gl.GetActiveUniform(ProgramID, i, 256, actualLength, arraySize, type2, sb);
            var nameSpan = new Span<byte>(sb, (int) actualLength[0]);
            var name = Utf8Utils.Utf8ToUtf16(nameSpan);
            _gl.CheckError();

            if (!shaderParams.ContainsKey(name))
            {
                var param = new GLShaderProgramParam(_gl, TypeFromUniformType(type2[0]), ParamType.Uniform, name);
                shaderParams.Add(param.Name, param);
                param.GetLocation(this);
            }
        }
    }

    Type TypeFromAttributeType(ActiveAttribType type)
    {
        switch (type)
        {
            case ActiveAttribType.Float: return typeof(float);
            case ActiveAttribType.FloatMat2: return typeof(float[]);
            case ActiveAttribType.FloatMat3: throw new Exception();
            case ActiveAttribType.FloatMat4: return typeof(Matrix4x4);
            case ActiveAttribType.FloatVec2: return typeof(Vector2);
            case ActiveAttribType.FloatVec3: return typeof(Vector3);
            case ActiveAttribType.FloatVec4: return typeof(Vector4);
            default: return typeof(object);
        }
    }

    Type TypeFromUniformType(ActiveUniformType type)
    {
        switch (type)
        {
            case ActiveUniformType.Int: return typeof(int);
            case ActiveUniformType.Float: return typeof(float);
            case ActiveUniformType.FloatVec2: return typeof(Vector2);
            case ActiveUniformType.FloatVec3: return typeof(Vector3);
            case ActiveUniformType.FloatVec4: return typeof(Vector4);
            case ActiveUniformType.IntVec2: return typeof(int[]);
            case ActiveUniformType.IntVec3: return typeof(int[]);
            case ActiveUniformType.IntVec4: return typeof(int[]);
            case ActiveUniformType.Bool: return typeof(bool);
            case ActiveUniformType.BoolVec2: return typeof(bool[]);
            case ActiveUniformType.BoolVec3: return typeof(bool[]);
            case ActiveUniformType.BoolVec4: return typeof(bool[]);
            case ActiveUniformType.FloatMat2: return typeof(float[]);
            case ActiveUniformType.FloatMat3: throw new Exception();
            case ActiveUniformType.FloatMat4: return typeof(Matrix4x4);
            case ActiveUniformType.Sampler1D:
            case ActiveUniformType.Sampler2D:
            case ActiveUniformType.Sampler3D:
            case ActiveUniformType.SamplerCube:
            case ActiveUniformType.Sampler1DShadow:
            case ActiveUniformType.Sampler2DShadow:
            case ActiveUniformType.Sampler2DRect:
            case ActiveUniformType.Sampler2DRectShadow: return typeof(int);
            case ActiveUniformType.FloatMat2x3:
            case ActiveUniformType.FloatMat2x4:
            case ActiveUniformType.FloatMat3x2:
            case ActiveUniformType.FloatMat3x4:
            case ActiveUniformType.FloatMat4x2:
            case ActiveUniformType.FloatMat4x3: return typeof(float[]);
            case ActiveUniformType.Sampler1DArray:
            case ActiveUniformType.Sampler2DArray:
            case ActiveUniformType.SamplerBuffer:
            case ActiveUniformType.Sampler1DArrayShadow:
            case ActiveUniformType.Sampler2DArrayShadow:
            case ActiveUniformType.SamplerCubeShadow: return typeof(int);
            case ActiveUniformType.UnsignedIntVec2: return typeof(uint[]);
            case ActiveUniformType.UnsignedIntVec3: return typeof(uint[]);
            case ActiveUniformType.UnsignedIntVec4: return typeof(uint[]);
            case ActiveUniformType.IntSampler1D:
            case ActiveUniformType.IntSampler2D:
            case ActiveUniformType.IntSampler3D:
            case ActiveUniformType.IntSamplerCube:
            case ActiveUniformType.IntSampler2DRect:
            case ActiveUniformType.IntSampler1DArray:
            case ActiveUniformType.IntSampler2DArray:
            case ActiveUniformType.IntSamplerBuffer: return typeof(int);
            case ActiveUniformType.UnsignedIntSampler1D:
            case ActiveUniformType.UnsignedIntSampler2D:
            case ActiveUniformType.UnsignedIntSampler3D:
            case ActiveUniformType.UnsignedIntSamplerCube:
            case ActiveUniformType.UnsignedIntSampler2DRect:
            case ActiveUniformType.UnsignedIntSampler1DArray:
            case ActiveUniformType.UnsignedIntSampler2DArray:
            case ActiveUniformType.UnsignedIntSamplerBuffer: return typeof(uint);
            case ActiveUniformType.Sampler2DMultisample: return typeof(int);
            case ActiveUniformType.IntSampler2DMultisample: return typeof(int);
            case ActiveUniformType.UnsignedIntSampler2DMultisample: return typeof(uint);
            case ActiveUniformType.Sampler2DMultisampleArray: return typeof(int);
            case ActiveUniformType.IntSampler2DMultisampleArray: return typeof(int);
            case ActiveUniformType.UnsignedIntSampler2DMultisampleArray: return typeof(uint);
            default: return typeof(object);
        }
    }

    public void Use() => _gl.UseProgram(ProgramID);

    public int GetUniformLocation(string name)
    {
        Use();
        var nameUtf8 = Encoding.UTF8.GetBytes(name);
        fixed (byte* p = nameUtf8)
            return _gl.GetUniformLocation(ProgramID, p);
    }

    public int GetAttributeLocation(string name)
    {
        Use();
        var nameUtf8 = Encoding.UTF8.GetBytes(name);
        fixed (byte* p = nameUtf8)
            return _gl.GetAttribLocation(ProgramID, p);
    }

    ~GLShaderProgram() => Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    void Dispose(bool disposing)
    {
        if (ProgramID != 0)
        {
            _gl.UseProgram(0);

            _gl.DetachShader(ProgramID, VertexShader.ShaderID);
            _gl.DetachShader(ProgramID, FragmentShader.ShaderID);
            _gl.DeleteProgram(ProgramID);

            if (DisposeChildren)
            {
                VertexShader.Dispose();
                FragmentShader.Dispose();
            }
        }
    }
}