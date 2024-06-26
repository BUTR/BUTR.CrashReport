﻿using ImGuiNET;

using Silk.NET.OpenGL;
using Silk.NET.Windowing;

using System;

namespace BUTR.CrashReport.Renderer.ImGui.Controller;

partial class ImGuiController
{
    private static readonly byte[] VertexShaderUtf8 = """
                                                      #version 330 core
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
                                                      } 
                                                      """u8.ToArray();

    private static readonly byte[] FragmentShaderUtf8 = """
                                                        #version 330 core
                                                        in vec2 Frag_UV;
                                                        in vec4 Frag_Color;
                                                        uniform sampler2D Texture;
                                                        layout (location = 0) out vec4 Out_Color;
                                                        void main()
                                                        {
                                                            Out_Color = Frag_Color * texture(Texture, Frag_UV.st);
                                                        } 
                                                        """u8.ToArray();

    public void Init()
    {
        _io.BackendFlags |= ImGuiBackendFlags.HasMouseCursors;
        _io.BackendFlags |= ImGuiBackendFlags.HasSetMousePos;
        _io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

        _windowsWidth = (uint) _view.Size.X;
        _windowsHeight = (uint) _view.Size.Y;

        CreateDeviceObjects();

        SetPerFrameImGuiData(1f / 60f);

        _view.Resize += WindowResized;

        Keyboard.KeyChar += OnKeyChar;
        Keyboard.KeyDown += KeyboardOnKeyDown;
        Keyboard.KeyUp += KeyboardOnKeyUp;

        Mouse.Scroll += MouseOnScroll;
        Mouse.MouseMove += MouseOnMouseMove;
        Mouse.MouseDown += MouseOnMouseDown;
        Mouse.MouseUp += MouseOnMouseUp;
    }

    private void CreateDeviceObjects()
    {
        var lastTexture = _gl.GetInteger(GLEnum.TextureBinding2D);
        var lastArrayBuffer = _gl.GetInteger(GLEnum.ArrayBufferBinding);
        var lastVertexArray = _gl.GetInteger(GLEnum.VertexArrayBinding);

        _shader = new Shader(_gl, VertexShaderUtf8, FragmentShaderUtf8);

        _attribLocationTex = _shader.GetUniformLocation("Texture\0"u8);
        _attribLocationProjMtx = _shader.GetUniformLocation("ProjMtx"u8);
        _attribLocationVtxPos = (uint) _shader.GetAttribLocation("Position\0"u8);
        _attribLocationVtxUv = (uint) _shader.GetAttribLocation("UV\0"u8);
        _attribLocationVtxColor = (uint) _shader.GetAttribLocation("Color\0"u8);

        _vboHandle = _gl.GenBuffer();
        _elementsHandle = _gl.GenBuffer();

        CreateFontsTexture();

        _gl.BindTexture(GLEnum.TextureBinding2D, (uint) lastTexture);
        _gl.BindBuffer(GLEnum.ArrayBuffer, (uint) lastArrayBuffer);
        _gl.BindVertexArray((uint) lastVertexArray);

        _gl.CheckGlError("End of ImGui setup");
    }

    private void CreateFontsTexture()
    {
        // Build texture atlas
        // Load as RGBA 32-bit (75% of the memory is wasted, but default font is so small) because it is more likely to be compatible with user's existing shaders.
        // If your ImTextureId represent a higher-level concept than just a GL texture id, consider calling GetTexDataAsAlpha8() instead to save on GPU memory
        _io.Fonts.GetTexDataAsRGBA32(out var pixels, out var width, out var height);

        // Upload texture to graphics system
        var lastTexture = _gl.GetInteger(GLEnum.TextureBinding2D);

        _fontTexture = new Texture(_gl, (uint) width, (uint) height, pixels);
        _fontTexture.Bind();
        _fontTexture.SetMagFilter(GLEnum.Linear);
        _fontTexture.SetMinFilter(GLEnum.Linear);
        _gl.PixelStore(GLEnum.UnpackRowLength, 0);

        _io.Fonts.SetTexID((IntPtr) _fontTexture.GlTexture);

        _gl.BindTexture(GLEnum.Texture2D, (uint) lastTexture);
    }

    private void DestroyDeviceObjects()
    {
        if (_vboHandle != 0)
        {
            _gl.DeleteBuffer(_vboHandle);
            _vboHandle = 0;
        }

        if (_elementsHandle != 0)
        {
            _gl.DeleteBuffer(_elementsHandle);
            _elementsHandle = 0;
        }
    }

    private void DestroyShader()
    {
        _shader.Dispose();
    }

    private void DestroyFontsTexture()
    {
        _io.Fonts.SetTexID(IntPtr.Zero);
        _fontTexture.Dispose();
    }

    public void Dispose()
    {
        DestroyDeviceObjects();
        DestroyShader();
        DestroyFontsTexture();
        _imgui.DestroyContext(_context);

        _input.Dispose();
        _gl.Dispose();
        _imgui.Dispose();
    }
}