// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Silk.NET.OpenGL;

using System.Runtime.CompilerServices;

namespace BUTR.CrashReport.Renderer.ImGui.Implementation.CImGui.Controller;

internal enum TextureCoordinate
{
    S = TextureParameterName.TextureWrapS,
    T = TextureParameterName.TextureWrapT,
    R = TextureParameterName.TextureWrapR,
}

internal class Texture : IDisposable
{
    public static float? MaxAniso;
    public readonly uint GlTexture;
    public readonly uint Width, Height;
    public readonly uint MipmapLevels;
    public readonly SizedInternalFormat InternalFormat;

    private readonly GL _gl;

    public unsafe Texture(GL gl, uint width, uint height, IntPtr data, bool generateMipmaps = false, bool srgb = false)
    {
        _gl = gl;
        if (MaxAniso is null)
        {
            MaxAniso = gl.GetFloat(GLEnum.MaxTextureMaxAnisotropy);
            _gl.CheckGlError();
        }
        Width = width;
        Height = height;
        InternalFormat = srgb ? SizedInternalFormat.Srgb8Alpha8 : SizedInternalFormat.Rgba8;
        MipmapLevels = generateMipmaps == false ? 1U : (uint) Math.Floor(Math.Log(Math.Max(Width, Height), 2));

        GlTexture = _gl.GenTexture();
        _gl.CheckGlError();
        Bind();

        var pxFormat = PixelFormat.Bgra;

        _gl.TexImage2D(GLEnum.Texture2D, 0, (int) InternalFormat, width, height, 0, pxFormat, GLEnum.UnsignedByte, data.ToPointer());
        _gl.CheckGlError();
        //_gl.TexStorage2D(GLEnum.Texture2D, MipmapLevels, InternalFormat, Width, Height);
        //_gl.TexSubImage2D(GLEnum.Texture2D, 0, 0, 0, Width, Height, pxFormat, PixelType.UnsignedByte, (void*) data);

        if (generateMipmaps)
        {
            _gl.GenerateTextureMipmap(GlTexture);
            _gl.CheckGlError();
        }

        SetWrap(TextureCoordinate.S, TextureWrapMode.Repeat);
        SetWrap(TextureCoordinate.T, TextureWrapMode.Repeat);

        var level = MipmapLevels - 1;
        _gl.TexParameterI(GLEnum.Texture2D, TextureParameterName.TextureMaxLevel, in level);
        _gl.CheckGlError();
    }

    public void Bind()
    {
        _gl.BindTexture(GLEnum.Texture2D, GlTexture);
        _gl.CheckGlError();
    }

    public void SetMinFilter(GLEnum filter)
    {
        _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMinFilter, in Unsafe.As<GLEnum, int>(ref filter));
        _gl.CheckGlError();
    }

    public void SetMagFilter(GLEnum filter)
    {
        _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMagFilter, in Unsafe.As<GLEnum, int>(ref filter));
        _gl.CheckGlError();
    }

    public void SetAnisotropy(float level)
    {
        _gl.TexParameter(GLEnum.Texture2D, TextureParameterName.TextureMaxAnisotropy, Util.Clamp(level, 1, MaxAniso.GetValueOrDefault()));
        _gl.CheckGlError();
    }

    public void SetLod(int @base, int min, int max)
    {
        _gl.TexParameterI(GLEnum.Texture2D, TextureParameterName.TextureLodBias, in @base);
        _gl.CheckGlError();
        _gl.TexParameterI(GLEnum.Texture2D, TextureParameterName.TextureMinLod, in min);
        _gl.CheckGlError();
        _gl.TexParameterI(GLEnum.Texture2D, TextureParameterName.TextureMaxLod, in max);
        _gl.CheckGlError();
    }

    public void SetWrap(TextureCoordinate coord, TextureWrapMode mode)
    {
        _gl.TexParameterI(GLEnum.Texture2D, (TextureParameterName) coord, in Unsafe.As<TextureWrapMode, int>(ref mode));
        _gl.CheckGlError();
    }

    public void Dispose()
    {
        _gl.DeleteTexture(GlTexture);
        _gl.CheckGlError();
    }
}