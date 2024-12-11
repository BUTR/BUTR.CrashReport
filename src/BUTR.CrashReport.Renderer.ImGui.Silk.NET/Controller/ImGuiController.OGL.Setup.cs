#if OPENGL
using BUTR.CrashReport.Memory;
using BUTR.CrashReport.Renderer.ImGui.Implementation.CImGui.Utils;
using BUTR.CrashReport.Renderer.ImGui.Silk.NET.Extensions;

using ImGui;

using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace BUTR.CrashReport.Renderer.ImGui.Implementation.CImGui.Controller;

internal partial class ImGuiController
{
    private static readonly LiteralSpan<byte> VertexShaderUtf8 = "#version 330 core\n"u8 +
                                                                 """
                                                                 layout (location = 0) in vec2 Position;
                                                                 layout (location = 1) in vec2 UV;
                                                                 layout (location = 2) in vec4 Color;
                                                                 uniform mat4 ProjMtx;
                                                                 out vec2 Frag_UV;
                                                                 out vec4 Frag_Color;
                                                                 void main()
                                                                 {
                                                                     Frag_UV = UV;
                                                                     Frag_Color = Color;
                                                                     gl_Position = ProjMtx * vec4(Position.xy,0,1);
                                                                 } 
                                                                 """u8 +
                                                                 "\0"u8;

    private static readonly LiteralSpan<byte> FragmentShaderUtf8 = "#version 330 core\n"u8 +
                                                                   """
                                                                   in vec2 Frag_UV;
                                                                   in vec4 Frag_Color;
                                                                   uniform sampler2D Texture;
                                                                   layout (location = 0) out vec4 Out_Color;
                                                                   void main()
                                                                   {
                                                                       Out_Color = Frag_Color * texture(Texture, Frag_UV.st);
                                                                   }
                                                                   """u8 +
                                                                   "\0"u8;

    private readonly GL _gl;

    private Shader _shader = default!;
    private Texture _fontTexture = default!;
    private int _attribLocationTex, _attribLocationProjMtx;
    private uint _attribLocationVtxPos, _attribLocationVtxUv, _attribLocationVtxColor;
    private uint _vboHandle, _elementsHandle, _vertexArrayObject;

    public ImGuiController(CmGui imgui, GL gl, IView view, IInputContext input)
    {
        _imgui = imgui;
        _gl = gl;
        _view = view;
        _input = input;
    }

    partial void CreateDeviceObjects()
    {
        var lastTexture = _gl.GetInteger(GLEnum.TextureBinding2D);
        _gl.CheckGlError();
        var lastArrayBuffer = _gl.GetInteger(GLEnum.ArrayBufferBinding);
        _gl.CheckGlError();
        var lastVertexArray = _gl.GetInteger(GLEnum.VertexArrayBinding);
        _gl.CheckGlError();

        _shader = new Shader(_gl, VertexShaderUtf8, FragmentShaderUtf8);

        _attribLocationTex = _shader.GetUniformLocation("Texture\0"u8);
        _attribLocationProjMtx = _shader.GetUniformLocation("ProjMtx\0"u8);
        _attribLocationVtxPos = (uint) _shader.GetAttribLocation("Position\0"u8);
        _attribLocationVtxUv = (uint) _shader.GetAttribLocation("UV\0"u8);
        _attribLocationVtxColor = (uint) _shader.GetAttribLocation("Color\0"u8);

        _vboHandle = _gl.GenBuffer();
        _gl.CheckGlError();
        _elementsHandle = _gl.GenBuffer();
        _gl.CheckGlError();

        CreateFontsTexture();

        _gl.BindTexture(GLEnum.Texture2D, (uint) lastTexture);
        _gl.CheckGlError();
        _gl.BindBuffer(GLEnum.ArrayBuffer, (uint) lastArrayBuffer);
        _gl.CheckGlError();
        _gl.BindVertexArray((uint) lastVertexArray);
        _gl.CheckGlError();
    }

    private void CreateFontsTexture()
    {
        var scale = 1f;
        if (_view.Native?.Sdl != null)
            scale = SdlUtils.GetScale();
        if (_view.Native?.Glfw != null)
            scale = GlfwUtils.GetScale();

        _imgui.GetIO(out var io);
        _imgui.ImFontConfig(out var config);
        io.GetFonts(out var fonts);

        config.RasterizerDensity = 2f; // Set your max scale

        var fontSize = Math.Round(13f * scale);

        var fontData = typeof(ImGuiController).Assembly.GetManifestResourceStreamAsSpan("CascadiaCode.ttf.compressed");
        var fontDataCopy = _imgui.MemAlloc<byte>((uint) fontData.Length);
        fontData.CopyTo(fontDataCopy);

        fonts.AddFontFromMemoryCompressedTTF(fontDataCopy, (float) fontSize, config, out _);

        // Build texture atlas
        // Load as RGBA 32-bit (75% of the memory is wasted, but default font is so small) because it is more likely to be compatible with user's existing shaders.
        // If your ImTextureId represent a higher-level concept than just a GL texture id, consider calling GetTexDataAsAlpha8() instead to save on GPU memory
        fonts.GetTexDataAsRGBA32(out var pixels, out var width, out var height, out _);

        // Upload texture to graphics system
        var lastTexture = _gl.GetInteger(GLEnum.TextureBinding2D);
        _gl.CheckGlError();

        _fontTexture = new Texture(_gl, (uint) width, (uint) height, pixels);
        _fontTexture.Bind();
        _fontTexture.SetMagFilter(GLEnum.Linear);
        _fontTexture.SetMinFilter(GLEnum.Linear);
        _gl.PixelStore(GLEnum.UnpackRowLength, 0);
        _gl.CheckGlError();

        fonts.SetTexID((IntPtr) _fontTexture.GlTexture);

        _gl.BindTexture(GLEnum.Texture2D, (uint) lastTexture);
        _gl.CheckGlError();

        _imgui.GetStyle(out var style);
        style.ScaleAllSizes(scale);
    }

    private void DestroyDeviceObjects()
    {
        if (_vboHandle != 0)
        {
            _gl.DeleteBuffer(_vboHandle);
            _gl.CheckGlError();
            _vboHandle = 0;
        }

        if (_elementsHandle != 0)
        {
            _gl.DeleteBuffer(_elementsHandle);
            _gl.CheckGlError();
            _elementsHandle = 0;
        }
    }

    private void DestroyShader()
    {
        _shader.Dispose();
    }

    private void DestroyFontsTexture()
    {
        _imgui.GetIO(out var io);
        io.GetFonts(out var fonts);
        fonts.SetTexID(IntPtr.Zero);

        _fontTexture.Dispose();
    }

    public void Reset()
    {
        DestroyDeviceObjects();
        DestroyShader();
        DestroyFontsTexture();

        _imgui.DestroyContext(_context);
    }
}
#endif