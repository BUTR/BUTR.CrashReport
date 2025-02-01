using BUTR.CrashReport.Memory;
using BUTR.CrashReport.Renderer.ImGui.WASM.Extensions;

using ImGui.Structures;

using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BUTR.CrashReport.Renderer.ImGui.WASM.Controller;

internal partial class ImGuiController
{
    private static readonly LiteralSpan<byte> VertexShader = "#version 300 es\n"u8 +
                                                             """
                                                             precision highp float;
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
                                                             
                                                                 gl_Position = ProjMtx * vec4(Position.xy, 0, 1);
                                                             }
                                                             """u8 + "\0"u8;

    private static readonly LiteralSpan<byte> FragmentShader = "#version 300 es\n"u8 +
                                                               """
                                                               precision mediump float;
                                                               uniform sampler2D Texture;
                                                               in vec2 Frag_UV;
                                                               in vec4 Frag_Color;
                                                               layout (location = 0) out vec4 Out_Color;
                                                               void main()
                                                               {
                                                                   Out_Color = Frag_Color * texture(Texture, Frag_UV.st);
                                                               }
                                                               """u8 + "\0"u8;


    private static readonly uint _sizeOfImDrawVert = (uint) Unsafe.SizeOf<ImGuiNET.ImDrawVert>();
    private static readonly unsafe void* _offsetOfImDrawVertPos = (void*) Marshal.OffsetOf<ImGuiNET.ImDrawVert>(nameof(ImGuiNET.ImDrawVert.pos));
    private static readonly unsafe void* _offsetOfImDrawVertUV = (void*) Marshal.OffsetOf<ImGuiNET.ImDrawVert>(nameof(ImGuiNET.ImDrawVert.uv));
    private static readonly unsafe void* _offsetOfImDrawVertCol = (void*) Marshal.OffsetOf<ImGuiNET.ImDrawVert>(nameof(ImGuiNET.ImDrawVert.col));

    private uint LoadTexture(IntPtr pixelData, uint width, uint height)
    {
        var textureId = _gl.GenTexture();
        _gl.CheckError();
        _gl.BindTexture(TextureTarget.Texture2D, textureId);
        _gl.CheckError();
        _gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.Linear);
        _gl.CheckError();
        _gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.Linear);
        _gl.CheckError();
        _gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
        _gl.CheckError();
        _gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
        _gl.CheckError();
        _gl.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixelData);
        _gl.CheckError();
        _gl.BindTexture(TextureTarget.Texture2D, 0);
        _gl.CheckError();
        return textureId;
    }

    private void RebuildFontAtlas()
    {
        var pixelRatio = GetWindowDevicePixelRatio();
        var maxPixelRatio = MathF.Max(pixelRatio.X, pixelRatio.Y);

        _imgui.GetIO(out var io);
        _imgui.ImFontConfig(out var config);
        io.GetFonts(out var fonts);

        config.RasterizerDensity = 2f;

        var fontSize = Math.Round(13f * maxPixelRatio);

        var fontData = typeof(ImGuiController).Assembly.GetManifestResourceStreamAsSpan("CascadiaCode.ttf.compressed");
        var fontDataCopy = _imgui.MemAlloc<byte>((uint) fontData.Length);
        fontData.CopyTo(fontDataCopy);

        fonts.AddFontFromMemoryCompressedTTF(fontDataCopy, (float) fontSize, config, out _);

        fonts.GetTexDataAsRGBA32(out var pixelData, out var width, out var height, out _);

        _fontTextureId = LoadTexture(pixelData, (uint) width, (uint) height);

        fonts.TexID = (IntPtr) _fontTextureId;
        fonts.ClearTexData();

        _imgui.GetStyle(out var style);
        style.ScaleAllSizes(maxPixelRatio);
    }

    public void Render()
    {
        SDL_GL_MakeCurrent(_window, _gl.Context);

        _imgui.Render();

        _imgui.GetDrawData(out var drawData);
        RenderDrawData(in drawData);
    }

    private unsafe void SetupRenderState(ref readonly ImDrawDataWrapper drawData, uint framebufferWidth, uint framebufferHeight)
    {
        _gl.Enable(EnableCap.Blend);
        _gl.CheckError();
        _gl.BlendEquation(BlendEquationMode.FuncAdd);
        _gl.CheckError();
        _gl.BlendFuncSeparate(BlendingFactorDest.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha, BlendingFactorDest.One, BlendingFactorDest.OneMinusSrcAlpha);
        _gl.CheckError();
        _gl.Disable(EnableCap.CullFace);
        _gl.CheckError();
        _gl.Disable(EnableCap.DepthTest);
        _gl.CheckError();
        _gl.Disable(EnableCap.StencilTest);
        _gl.CheckError();
        _gl.Enable(EnableCap.ScissorTest);
        _gl.CheckError();

        _gl.Viewport(0, 0, framebufferWidth, framebufferHeight);
        _gl.CheckError();

        _gl.UseProgram(_shader.ProgramID);
        _gl.CheckError();

        var orthographicProjection = Matrix4x4.CreateOrthographicOffCenter(
            left: drawData.DisplayPos.X,
            right: drawData.DisplayPos.X + drawData.DisplaySize.X,
            top: drawData.DisplayPos.Y,
            bottom: drawData.DisplayPos.Y + drawData.DisplaySize.Y,
            zNearPlane: -1,
            zFarPlane: 1);

        _shader["Texture"]!.SetValue(0);
        _shader["ProjMtx"]!.SetValue(in orthographicProjection);

        _gl.BindSampler(0, 0);
        _gl.CheckError();

        // Setup desired _gl. state
        // Recreate the VAO every time (this is to easily allow multiple _gl. contexts to be rendered to. VAO are not shared among _gl. contexts)
        // The renderer would actually work without any VAO bound, but then our VertexAttrib calls would overwrite the default one currently bound.
        _vertexArrayObject = _gl.GenVertexArray();
        _gl.CheckError();
        _gl.BindVertexArray(_vertexArrayObject);
        _gl.CheckError();

        _gl.BindBuffer(BufferTarget.ArrayBuffer, _vboHandle);
        _gl.CheckError();
        _gl.BindBuffer(BufferTarget.ElementArrayBuffer, _elementsHandle);
        _gl.CheckError();

        _gl.EnableVertexAttribArray((uint) _shader["Position"]!.Location);
        _gl.CheckError();
        _gl.EnableVertexAttribArray((uint) _shader["UV"]!.Location);
        _gl.CheckError();
        _gl.EnableVertexAttribArray((uint) _shader["Color"]!.Location);
        _gl.CheckError();

        _gl.VertexAttribPointerBool(_shader["Position"]!.Location, 2, VertexAttribPointerType.Float, false, _sizeOfImDrawVert, _offsetOfImDrawVertPos);
        _gl.CheckError();
        _gl.VertexAttribPointerBool(_shader["UV"]!.Location, 2, VertexAttribPointerType.Float, false, _sizeOfImDrawVert, _offsetOfImDrawVertUV);
        _gl.CheckError();
        _gl.VertexAttribPointerBool(_shader["Color"]!.Location, 4, VertexAttribPointerType.UnsignedByte, true, _sizeOfImDrawVert, _offsetOfImDrawVertCol);
        _gl.CheckError();
    }

    private unsafe void RenderCommandList(ref readonly ImDrawDataWrapper drawData, uint framebufferWidth, uint framebufferHeight)
    {
        _imgui.GetIO(out var io);

        ref var clipOffset = ref drawData.DisplayPos;
        ref var clipScale = ref drawData.FramebufferScale;

        drawData.ScaleClipRects(io.DisplayFramebufferScale);

        drawData.GetCmdLists(out var cmdLists);
        for (var n = 0; n < drawData.CmdListsCount; n++)
        {
            ref var cmdList = ref cmdLists[n];
            cmdList.GetVtxBuffer(out var vtxBuffer);
            cmdList.GetIdxBuffer(out var idxBuffer);
            cmdList.GetCmdBuffer(out var cmdBuffer);

            _gl.BufferData(BufferTarget.ArrayBuffer, vtxBuffer.SizeInBytes, vtxBuffer.Data, BufferUsageHint.StreamDraw);
            _gl.CheckError();
            _gl.BufferData(BufferTarget.ElementArrayBuffer, idxBuffer.SizeInBytes, idxBuffer.Data, BufferUsageHint.StreamDraw);
            _gl.CheckError();

            for (var cmd_i = 0; cmd_i < cmdBuffer.Size; cmd_i++)
            {
                ref var pcmd = ref cmdBuffer[cmd_i];

                switch (pcmd.UserCallback)
                {
                    case 0:
                        break;
                    case -1 or -8:
                        SetupRenderState(in drawData, framebufferWidth, framebufferHeight);
                        break;
                    default:
                        ThrowNotImplementedException();
                        break;
                }

                if (pcmd.ElemCount == 0)
                    continue;

                var clip_min = new Vector2Ref((pcmd.ClipRect.X - clipOffset.X) * clipScale.X, (pcmd.ClipRect.Y - clipOffset.Y) * clipScale.Y);
                var clip_max = new Vector2Ref((pcmd.ClipRect.Z - clipOffset.X) * clipScale.X, (pcmd.ClipRect.W - clipOffset.Y) * clipScale.Y);
                if (clip_max.X <= clip_min.X || clip_max.Y <= clip_min.Y)
                    continue;

                _gl.Scissor((int) clip_min.X, (int) (framebufferHeight - clip_max.Y), (uint) (clip_max.X - clip_min.X), (uint) (clip_max.Y - clip_min.Y));
                _gl.CheckError();

                _gl.BindTexture(TextureTarget.Texture2D, (uint) pcmd.TextureId);
                _gl.CheckError();

                _gl.DrawElements(PrimitiveType.Triangles, pcmd.ElemCount, DrawElementsType.UnsignedShort, (void*) (pcmd.IdxOffset * sizeof(ushort)));
                _gl.CheckError();
            }
        }
    }

    private void RenderDrawData(ref readonly ImDrawDataWrapper drawData)
    {
        if (drawData.CmdListsCount == 0) return;

        var framebufferWidth = (uint) (drawData.DisplaySize.X * drawData.FramebufferScale.X);
        var framebufferHeight = (uint) (drawData.DisplaySize.Y * drawData.FramebufferScale.Y);
        if (framebufferWidth <= 0 || framebufferHeight <= 0) return;

        _gl.ActiveTexture(TextureUnit.Texture0);
        _gl.CheckError();

        SetupRenderState(in drawData, framebufferWidth, framebufferHeight);

        RenderCommandList(in drawData, framebufferWidth, framebufferHeight);

        // Destroy the temporary VAO
        _gl.DeleteVertexArray(_vertexArrayObject);
        _gl.CheckError();
        _vertexArrayObject = 0;
    }

    [DoesNotReturn]
    private static void ThrowNotImplementedException() => throw new NotImplementedException();

}