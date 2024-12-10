#if OPENGLES3
global using static OpenGLES3.GL;
#endif
#if SDL2
global using static SDL2.SDL;
#endif
#if SDL3
global using static SDL3.SDL;
#endif
#if EMSCRIPTEN
global using static Emscripten.Emscripten;
#endif

global using static ImGui.CmGui;