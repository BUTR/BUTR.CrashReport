#if OPENGL

using ImGui.Structures;

using Silk.NET.OpenGL;

using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace BUTR.CrashReport.Renderer.ImGui.Implementation.CImGui.Controller;

internal partial class ImGuiController
{
    partial void RenderImDrawData(ref readonly ImDrawDataWrapper drawData)
    {
        if (drawData.CmdListsCount == 0) return;

        var framebufferWidth = (uint) (drawData.DisplaySize.X * drawData.FramebufferScale.X);
        var framebufferHeight = (uint) (drawData.DisplaySize.Y * drawData.FramebufferScale.Y);
        if (framebufferWidth <= 0 || framebufferHeight <= 0) return;

        // Backup GL state
        var lastActiveTexture = (TextureUnit) _gl.GetInteger(GLEnum.ActiveTexture);
        _gl.CheckGlError();
        _gl.ActiveTexture(GLEnum.Texture0);
        _gl.CheckGlError();

        var lastProgram = (uint) _gl.GetInteger(GLEnum.CurrentProgram);
        _gl.CheckGlError();
        var lastTexture = (uint) _gl.GetInteger(GLEnum.TextureBinding2D);
        _gl.CheckGlError();

        var lastSampler = (uint) _gl.GetInteger(GLEnum.SamplerBinding);
        _gl.CheckGlError();

        var lastVertexArrayObject = (uint) _gl.GetInteger(GLEnum.VertexArrayBinding);
        _gl.CheckGlError();
        var lastArrayBuffer = (uint) _gl.GetInteger(GLEnum.ArrayBufferBinding);
        _gl.CheckGlError();

        var lastPolygonMode = (PolygonMode) _gl.GetInteger(GLEnum.PolygonMode);
        _gl.CheckGlError();

        Span<int> lastScissorBox = stackalloc int[4];
        _gl.GetInteger(GLEnum.ScissorBox, lastScissorBox);
        _gl.CheckGlError();

        Span<int> lastViewport = stackalloc int[4];
        _gl.GetInteger(GLEnum.Viewport, lastViewport);
        _gl.CheckGlError();

        var lastBlendSrcRgb = (BlendingFactor) _gl.GetInteger(GLEnum.BlendSrcRgb);
        _gl.CheckGlError();
        var lastBlendDstRgb = (BlendingFactor) _gl.GetInteger(GLEnum.BlendDstRgb);
        _gl.CheckGlError();

        var lastBlendSrcAlpha = (BlendingFactor) _gl.GetInteger(GLEnum.BlendSrcAlpha);
        _gl.CheckGlError();
        var lastBlendDstAlpha = (BlendingFactor) _gl.GetInteger(GLEnum.BlendDstAlpha);
        _gl.CheckGlError();

        var lastBlendEquationRgb = (BlendEquationModeEXT) _gl.GetInteger(GLEnum.BlendEquationRgb);
        _gl.CheckGlError();
        var lastBlendEquationAlpha = (BlendEquationModeEXT) _gl.GetInteger(GLEnum.BlendEquationAlpha);
        _gl.CheckGlError();

        var lastEnableBlend = _gl.IsEnabled(GLEnum.Blend);
        _gl.CheckGlError();
        var lastEnableCullFace = _gl.IsEnabled(GLEnum.CullFace);
        _gl.CheckGlError();
        var lastEnableDepthTest = _gl.IsEnabled(GLEnum.DepthTest);
        _gl.CheckGlError();
        var lastEnableStencilTest = _gl.IsEnabled(GLEnum.StencilTest);
        _gl.CheckGlError();
        var lastEnableScissorTest = _gl.IsEnabled(GLEnum.ScissorTest);
        _gl.CheckGlError();

        var lastEnablePrimitiveRestart = _gl.IsEnabled(GLEnum.PrimitiveRestart);
        _gl.CheckGlError();

        SetupRenderState(in drawData, framebufferWidth, framebufferHeight);

        RenderCommandList(in drawData, framebufferWidth, framebufferHeight);

        // Destroy the temporary VAO
        _gl.DeleteVertexArray(_vertexArrayObject);
        _gl.CheckGlError();
        _vertexArrayObject = 0;

        // Restore modified GL state
        _gl.UseProgram(lastProgram);
        _gl.CheckGlError();
        _gl.BindTexture(GLEnum.Texture2D, lastTexture);
        _gl.CheckGlError();

        _gl.BindSampler(0, lastSampler);
        _gl.CheckGlError();

        _gl.ActiveTexture(lastActiveTexture);
        _gl.CheckGlError();

        _gl.BindVertexArray(lastVertexArrayObject);
        _gl.CheckGlError();

        _gl.BindBuffer(GLEnum.ArrayBuffer, lastArrayBuffer);
        _gl.CheckGlError();
        _gl.BlendEquationSeparate(lastBlendEquationRgb, lastBlendEquationAlpha);
        _gl.CheckGlError();
        _gl.BlendFuncSeparate(lastBlendSrcRgb, lastBlendDstRgb, lastBlendSrcAlpha, lastBlendDstAlpha);
        _gl.CheckGlError();

        if (lastEnableBlend) _gl.Enable(GLEnum.Blend);
        else _gl.Disable(GLEnum.Blend);
        _gl.CheckGlError();

        if (lastEnableCullFace) _gl.Enable(GLEnum.CullFace);
        else _gl.Disable(GLEnum.CullFace);
        _gl.CheckGlError();

        if (lastEnableDepthTest) _gl.Enable(GLEnum.DepthTest);
        else _gl.Disable(GLEnum.DepthTest);
        _gl.CheckGlError();
        if (lastEnableStencilTest) _gl.Enable(GLEnum.StencilTest);
        else _gl.Disable(GLEnum.StencilTest);
        _gl.CheckGlError();

        if (lastEnableScissorTest) _gl.Enable(GLEnum.ScissorTest);
        else _gl.Disable(GLEnum.ScissorTest);
        _gl.CheckGlError();

        if (lastEnablePrimitiveRestart) _gl.Enable(GLEnum.PrimitiveRestart);
        else _gl.Disable(GLEnum.PrimitiveRestart);
        _gl.CheckGlError();

        _gl.PolygonMode(GLEnum.FrontAndBack, lastPolygonMode);
        _gl.CheckGlError();

        _gl.Viewport(lastViewport[0], lastViewport[1], (uint) lastViewport[2], (uint) lastViewport[3]);
        _gl.CheckGlError();

        _gl.Scissor(lastScissorBox[0], lastScissorBox[1], (uint) lastScissorBox[2], (uint) lastScissorBox[3]);
        _gl.CheckGlError();
    }

    private void SetupRenderState(ref readonly ImDrawDataWrapper drawData, uint framebufferWidth, uint framebufferHeight)
    {
        _gl.Enable(GLEnum.Blend);
        _gl.CheckGlError();
        _gl.BlendEquation(GLEnum.FuncAdd);
        _gl.CheckGlError();
        _gl.BlendFuncSeparate(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha, GLEnum.One, GLEnum.OneMinusSrcAlpha);
        _gl.CheckGlError();
        _gl.Disable(GLEnum.CullFace);
        _gl.CheckGlError();
        _gl.Disable(GLEnum.DepthTest);
        _gl.CheckGlError();
        _gl.Disable(GLEnum.StencilTest);
        _gl.CheckGlError();
        _gl.Enable(GLEnum.ScissorTest);
        _gl.CheckGlError();
        _gl.Disable(GLEnum.PrimitiveRestart);
        _gl.CheckGlError();
        _gl.PolygonMode(GLEnum.FrontAndBack, GLEnum.Fill);

        _gl.Viewport(0, 0, framebufferWidth, framebufferHeight);
        _gl.CheckGlError();

        var orthographicProjection = Matrix4x4.CreateOrthographicOffCenter(
            left: drawData.DisplayPos.X,
            right: drawData.DisplayPos.X + drawData.DisplaySize.X,
            bottom: drawData.DisplayPos.Y + drawData.DisplaySize.Y,
            top: drawData.DisplayPos.Y,
            zNearPlane: -1,
            zFarPlane: 1);

        _shader.UseShader();
        _gl.Uniform1(_attribLocationTex, 0);
        _gl.CheckGlError();
        _gl.UniformMatrix4x4(_attribLocationProjMtx, ref orthographicProjection);
        _gl.CheckGlError();

        _gl.BindSampler(0, 0);
        _gl.CheckGlError();

        // Setup desired GL state
        // Recreate the VAO every time (this is to easily allow multiple GL contexts to be rendered to. VAO are not shared among GL contexts)
        // The renderer would actually work without any VAO bound, but then our VertexAttrib calls would overwrite the default one currently bound.
        _vertexArrayObject = _gl.GenVertexArray();
        _gl.CheckGlError();
        _gl.BindVertexArray(_vertexArrayObject);
        _gl.CheckGlError();

        _gl.BindBuffer(GLEnum.ArrayBuffer, _vboHandle);
        _gl.CheckGlError();
        _gl.BindBuffer(GLEnum.ElementArrayBuffer, _elementsHandle);
        _gl.CheckGlError();
        _gl.EnableVertexAttribArray(_attribLocationVtxPos);
        _gl.CheckGlError();
        _gl.EnableVertexAttribArray(_attribLocationVtxUv);
        _gl.CheckGlError();
        _gl.EnableVertexAttribArray(_attribLocationVtxColor);
        _gl.CheckGlError();
        _gl.VertexAttribPointer2(_attribLocationVtxPos, 2, GLEnum.Float, false, _sizeOfImDrawVert, _offsetOfImDrawVertPos);
        _gl.CheckGlError();
        _gl.VertexAttribPointer2(_attribLocationVtxUv, 2, GLEnum.Float, false, _sizeOfImDrawVert, _offsetOfImDrawVertUV);
        _gl.CheckGlError();
        _gl.VertexAttribPointer2(_attribLocationVtxColor, 4, GLEnum.UnsignedByte, true, _sizeOfImDrawVert, _offsetOfImDrawVertCol);
        _gl.CheckGlError();
    }

    private void RenderCommandList(ref readonly ImDrawDataWrapper drawData, uint framebufferWidth, uint framebufferHeight)
    {
        // Will project scissor/clipping rectangles into framebuffer space
        ref var clipOff = ref drawData.DisplayPos;         // (0,0) unless using multi-viewports
        ref var clipScale = ref drawData.FramebufferScale; // (1,1) unless using retina display which are often (2,2)

        // Render command lists
        //var cmdListSpan = drawData.CmdLists.AsSpan();
        drawData.GetCmdLists(out var cmdLists);
        for (var n = 0; n < cmdLists.Size; n++)
        {
            ref var cmdList = ref cmdLists[n];
            cmdList.GetVtxBuffer(out var vtxBuffer);
            cmdList.GetIdxBuffer(out var idxBuffer);
            cmdList.GetCmdBuffer(out var cmdBuffer);

            // Upload vertex/index buffers

            _gl.BufferData(GLEnum.ArrayBuffer, vtxBuffer.AsSpan(), GLEnum.StreamDraw);
            _gl.CheckGlError($"Data Vert {n}");
            _gl.BufferData(GLEnum.ElementArrayBuffer, idxBuffer.AsSpan(), GLEnum.StreamDraw);
            _gl.CheckGlError($"Data Idx {n}");

            for (var cmd_i = 0; cmd_i < cmdBuffer.Size; cmd_i++)
            {
                ref var cmd = ref cmdBuffer[cmd_i];

                if (cmd.UserCallback != IntPtr.Zero)
                    ThrowNotImplementedException();

                Vector4Ref clipRect;
                clipRect.X = (cmd.ClipRect.X - clipOff.X) * clipScale.X;
                clipRect.Y = (cmd.ClipRect.Y - clipOff.Y) * clipScale.Y;
                clipRect.Z = (cmd.ClipRect.Z - clipOff.X) * clipScale.X;
                clipRect.W = (cmd.ClipRect.W - clipOff.Y) * clipScale.Y;

                if (clipRect.X < framebufferWidth && clipRect.Y < framebufferHeight && clipRect is { Z: >= 0.0f, W: >= 0.0f })
                {
                    _gl.Scissor((int) clipRect.X, (int) (framebufferHeight - clipRect.W), (uint) (clipRect.Z - clipRect.X), (uint) (clipRect.W - clipRect.Y));
                    _gl.CheckGlError();

                    if (cmd.TextureId != IntPtr.Zero)
                    {
                        _gl.BindTexture(GLEnum.Texture2D, (uint) cmd.TextureId);
                        _gl.CheckGlError();
                    }

                    _gl.DrawElementsBaseVertex2(GLEnum.Triangles, cmd.ElemCount, GLEnum.UnsignedShort, (IntPtr) (cmd.IdxOffset * sizeof(ushort)), (int) cmd.VtxOffset);
                    _gl.CheckGlError();
                }
            }
        }
    }

    private static void ThrowNotImplementedException() => throw new NotImplementedException();
}
#endif