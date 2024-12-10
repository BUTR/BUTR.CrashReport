using BUTR.CrashReport.Native;

using ImGui;

using OpenGLES3;

namespace BUTR.CrashReport.Renderer.ImGui.WASM.Controller;

internal partial class ImGuiController : IDisposable
{
    private static readonly Dictionary<IntPtr, WeakReference<ImGuiController>> _instances = new();

    private readonly Allocator _allocator = new();
    private readonly IntPtr _window;
    private readonly GL _gl;
    private readonly Emscripten.Emscripten _emscripten;
    private readonly CmGui _imgui;
    private readonly GLShaderProgram _shader;
    private readonly uint _vboHandle;
    private readonly uint _elementsHandle;
    private uint _vertexArrayObject, _fontTextureId;

    public ImGuiController(IntPtr window, GL gl, Emscripten.Emscripten emscripten, CmGui imgui)
    {
        _instances[window] = new WeakReference<ImGuiController>(this);

        _window = window;
        _gl = gl;
        _emscripten = emscripten;
        _imgui = imgui;
        _shader = new GLShaderProgram(_gl, VertexShader, FragmentShader);

        _vboHandle = _gl.GenBuffer();
        _gl.CheckError();
        _elementsHandle = _gl.GenBuffer();
        _gl.CheckError();

        var context = _imgui.CreateContext();
        _imgui.SetCurrentContext(context);

        RebuildFontAtlas();

        SetupEmscripten(_window);
    }

    public void Dispose()
    {
        _allocator.Dispose();

        _shader.Dispose();

        _gl.DeleteBuffer(_vboHandle);
        _gl.CheckError();
        _gl.DeleteBuffer(_elementsHandle);
        _gl.CheckError();
        _gl.DeleteVertexArray(_vertexArrayObject);
        _gl.CheckError();
        _gl.DeleteTexture(_fontTextureId);
        _gl.CheckError();
    }
}