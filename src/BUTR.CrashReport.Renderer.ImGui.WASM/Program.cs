using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.Models;
using BUTR.CrashReport.Native;
using BUTR.CrashReport.Renderer.ImGui.Renderer;
using BUTR.CrashReport.Renderer.ImGui.WASM.Controller;

using Emscripten;

using ImGui;
using ImGui.Structures;

using OpenGLES3;

using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;

[assembly: PInvokeDelegateLoader(typeof(CmGui), "cimgui")]

[assembly: PInvokeDelegateLoader(typeof(Emscripten.Emscripten), "*")]

namespace BUTR.CrashReport.Renderer.ImGui.WASM;

using ImGuiRenderer = ImGuiRenderer<ImGuiIOWrapper, ImGuiViewportWrapper, ImDrawListWrapper, ImGuiStyleWrapper, RangeAccessorRef<System.Numerics.Vector4, ImGuiCol>, ImGuiListClipperWrapper>;

public static partial class Program
{
    [JSImport("finishedLoading", "interop")]
    private static partial void FinishedLoading();

    [JSExport]
    private static void SetDarkMode(bool isDarkMode)
    {
        _renderer.SetDarkMode(isDarkMode);
    }

    // https://localhost:7211/?arg=http%3A%2F%2Flocalhost%3A65530%2Fcrashreport.json
    internal static async Task Main(string[] args)
    {
        var url = args.Length > 0 ? args[0] : throw new ArgumentException("URL to Crash Report JSON is required");
        var cr = await FetchAsync(url);

        _emscripten = CreateEmscripten();

        _imgui = CreateCmGui();
        _renderer = CreateImGuiRenderer(cr, [], _imgui);

        _window = CreateWindow("BUTR Crash Report Renderer"u8, 800, 600);
        _gl = CreateGL(_window);
        _controller = CreateImGuiGLRenderer(_window, _gl, _imgui);

        FinishedLoading();

        SetMainLoop();
    }

    //private static readonly HttpRequestOptionsKey<IDictionary<string, object>> FetchRequestOptionsKey = new("WebAssemblyFetchOptions");
    private static async Task<CrashReportModel> FetchAsync(string url)
    {
        using var client = new HttpClient();

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        /*
        if (!request.Options.TryGetValue(FetchRequestOptionsKey, out var fetchOptions))
        {
            fetchOptions = new Dictionary<string, object>(StringComparer.Ordinal);
            request.Options.Set(FetchRequestOptionsKey, fetchOptions);
        }
        
        fetchOptions["mode"] = "no-cors";
        */

        using var response = await client.SendAsync(request);

        return JsonSerializer.Deserialize<CrashReportModel>(await response.Content.ReadAsStreamAsync(), CustomJsonSerializerContext.Default.CrashReportModel)!;
    }

    private static unsafe void SetMainLoop()
    {
        _emscripten.emscripten_set_main_loop(&MainLoop, 0, 0);
    }

    private static Emscripten.Emscripten _emscripten = default!;
    private static CmGui _imgui = default!;
    private static ImGuiController _controller = default!;
    private static ImGuiRenderer _renderer = default!;
    private static IntPtr _window = default!;
    private static GL _gl = default!;

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void MainLoop()
    {
#if SDL2
        while (SDL_PollEvent(out var e) != 0)
#elif SDL3
        while (SDL_PollEvent(out var e))
#endif
        {
            _controller.ProcessEvent(e);
            /*
            switch (e.type)
            {
                case SDL_EventType.SDL_QUIT:
                {
                    break;
                }
                case SDL_EventType.SDL_KEYDOWN:
                {
                    switch (e.key.keysym.sym)
                    {
                        case SDL_Keycode.SDLK_ESCAPE:
                        case SDL_Keycode.SDLK_q:
                            break;
                    }

                    break;
                }
            }
            */
        }


        _gl.ClearColor(0.1f, 0.2f, 0.3f, 1.0f);
        _gl.Clear(ClearBufferMask.ColorBufferBit);

        _controller.NewFrame();
        _renderer.Render();
        _controller.Render();

        SDL_GL_SwapWindow(_window);
    }

    private static CmGui CreateCmGui()
    {
        var cmgui = new CmGui();
        cmgui.LoadFromPInvoke();
        return cmgui;
    }

    private static ImGuiRenderer CreateImGuiRenderer(CrashReportModel cr, LogSourceModel[] logs, CmGui imgui)
    {
        var renderer = new ImGuiRenderer(imgui, imgui, imgui, imgui, imgui, imgui, cr, logs, new CrashReportRendererUtilities(cr, logs), () => { });
        return renderer;
    }

    private static IntPtr CreateWindow(ReadOnlySpan<byte> title, int width, int height)
    {
#if SDL2
        SDL_Init(SDL_INIT_VIDEO);
        SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_ES);
        SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
        SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 0);

        var flags = SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI;

        //SDL_SetHint(SDL_HINT_MOUSE_RELATIVE_SCALING, "1");
        //SDL_SetHint(SDL_HINT_WINDOWS_DPI_AWARENESS, "1");
        //SDL_SetHint(SDL_HINT_WINDOWS_DPI_SCALING, "1");

        var window = SDL_CreateWindow(title, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, width, height, flags);
#elif SDL3
        SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO);
        //SDL_GL_SetAttribute(SDL_GLAttr.SDL_GL_CONTEXT_PROFILE_MASK, SDL_GL_CONTEXT_PROFILE_ES);
        SDL_GL_SetAttribute(SDL_GLAttr.SDL_GL_CONTEXT_PROFILE_MASK, 0x0004);
        SDL_GL_SetAttribute(SDL_GLAttr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
        SDL_GL_SetAttribute(SDL_GLAttr.SDL_GL_CONTEXT_MINOR_VERSION, 0);

        var flags = SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDL_WindowFlags.SDL_WINDOW_HIGH_PIXEL_DENSITY;

        var window = SDL_CreateWindow(title, width, height, flags);
#endif

        return window;
    }

    private static unsafe GL CreateGL(IntPtr window)
    {
        var glContext = SDL_GL_CreateContext(window);
        if (glContext == IntPtr.Zero)
            throw new Exception("CouldNotCreateContext");

        var gl = new GL(glContext, pointer => SDL_GL_GetProcAddress((byte*) pointer));

        SDL_GL_MakeCurrent(window, glContext);
        SDL_GL_SetSwapInterval(1);

        return gl;
    }

    private static ImGuiController CreateImGuiGLRenderer(IntPtr window, GL gl, CmGui imgui)
    {
        return new ImGuiController(window, gl, _emscripten, imgui);
    }

    private static Emscripten.Emscripten CreateEmscripten()
    {
        var emscripten = new Emscripten.Emscripten();
        emscripten.LoadFromPInvoke();
        return emscripten;
    }
}