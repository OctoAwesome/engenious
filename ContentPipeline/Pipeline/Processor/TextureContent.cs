using System;
using engenious.Graphics;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;


namespace engenious.Content.Pipeline
{
    public class TextureContent
    {
        private GraphicsDevice graphicsDevice;
        private int texture;

        public TextureContent(GraphicsDevice graphicsDevice,bool generateMipMaps, int mipMapCount, byte[] inputData, int width, int height, TextureContentFormat inputFormat, TextureContentFormat outputFormat)
        {
            this.graphicsDevice = graphicsDevice;
            System.Runtime.InteropServices.GCHandle handle = System.Runtime.InteropServices.GCHandle.Alloc(inputData, System.Runtime.InteropServices.GCHandleType.Pinned);
            createTexture(generateMipMaps, mipMapCount, handle.AddrOfPinnedObject(), width, height, inputFormat, outputFormat);
            handle.Free();
        }

        public TextureContent(GraphicsDevice graphicsDevice,bool generateMipMaps, int mipMapCount, IntPtr inputData, int width, int height, TextureContentFormat inputFormat, TextureContentFormat outputFormat)
        {
            this.graphicsDevice = graphicsDevice;
            createTexture(generateMipMaps, mipMapCount, inputData, width, height, inputFormat, outputFormat);
        }

        private void createTexture(bool generateMipMaps, int mipMapCount, IntPtr inputData, int width, int height, TextureContentFormat inputFormat, TextureContentFormat outputFormat)
        {
            Width = width;
            Height = height;
            Format = outputFormat;
            MipMaps = new List<TextureContentMipMap>();
            bool hwCompressedInput = inputFormat == TextureContentFormat.DXT1 || inputFormat == TextureContentFormat.DXT3 || inputFormat == TextureContentFormat.DXT5;
            bool hwCompressedOutput = outputFormat == TextureContentFormat.DXT1 || outputFormat == TextureContentFormat.DXT3 || outputFormat == TextureContentFormat.DXT5;
            using(Execute.OnUiContext)
            {
                texture = GL.GenTexture();

                GL.BindTexture(TextureTarget.Texture2D, texture);
                bool doGenerate = generateMipMaps && mipMapCount > 1;

                setDefaultTextureParameters();
                //GL.TexStorage2D(TextureTarget2d.Texture2D,(GenerateMipMaps ? 1 : MipMapCount),SizedInternalFormat.Rgba8,width,height);
                //GL.TexSubImage2D(TextureTarget.Texture2D,0,0,0,width,height,
                if (doGenerate)
                {
                    if (graphicsDevice.majorVersion < 3 &&
                        ((graphicsDevice.majorVersion == 1 && graphicsDevice.minorVersion >= 4) ||
                         graphicsDevice.majorVersion > 1))
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, 1);
                    else if (graphicsDevice.majorVersion < 3)
                        throw new NotSupportedException("Can't generate MipMaps on this Hardware");
                }
                GL.TexImage2D(TextureTarget.Texture2D, 0, (hwCompressedOutput ? (OpenTK.Graphics.OpenGL4.PixelInternalFormat)outputFormat : OpenTK.Graphics.OpenGL4.PixelInternalFormat.Rgba), width, height, 0, (hwCompressedInput ? (OpenTK.Graphics.OpenGL4.PixelFormat)inputFormat : OpenTK.Graphics.OpenGL4.PixelFormat.Bgra), PixelType.UnsignedByte, inputData);
                if (doGenerate)
                {
                    //TOODO non power of 2 Textures?
                    GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureMaxLevel,mipMapCount);
                    GL.Hint(HintTarget.GenerateMipmapHint,HintMode.Nicest);
                    if (graphicsDevice.majorVersion >= 3)
                        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                }
            }

            PreprocessMipMaps();

            using(Execute.OnUiContext)
            {
                GL.DeleteTexture(texture);
            }
        }

        private void setDefaultTextureParameters()
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        }

        private void PreprocessMipMaps()
        {
            bool hwCompressed = Format == TextureContentFormat.DXT1 || Format == TextureContentFormat.DXT3 || Format == TextureContentFormat.DXT5;
            int width=Width, height=Height;
            int realCount=0;
            for (int i = 0; i < (GenerateMipMaps ? 1 : MipMapCount); i++)
            {
                if (hwCompressed)
                {
                    int dataSize=0;
                    byte[] data=null;
                    using(Execute.OnUiContext)
                    {
                        GL.BindTexture(TextureTarget.Texture2D,texture);
                        GL.GetTexLevelParameter(TextureTarget.Texture2D,i,GetTextureParameter.TextureCompressedImageSize,out dataSize);
                        data = new byte[dataSize];
                        GL.GetCompressedTexImage(TextureTarget.Texture2D,i,data);
                    }
                    MipMaps.Add(new TextureContentMipMap(width, height, Format, data));
                }
                else
                {
                    var bmp = new System.Drawing.Bitmap(width,height);

                    var bmpData = bmp.LockBits(new System.Drawing.Rectangle(0,0,width,height),System.Drawing.Imaging.ImageLockMode.WriteOnly,System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    using(Execute.OnUiContext)
                    {
                        GL.BindTexture(TextureTarget.Texture2D,texture);
                        GL.GetTexImage(TextureTarget.Texture2D,i,OpenTK.Graphics.OpenGL4.PixelFormat.Bgra,PixelType.UnsignedByte,bmpData.Scan0);
                    }

                    bmp.UnlockBits(bmpData);

                    MipMaps.Add(new TextureContentMipMap(width, height, Format, bmp));

                }
                width/=2;
                height/=2;
                realCount++;
                if (width == 0 || height == 0)
                    break;
            }
            if (!GenerateMipMaps)
                MipMapCount = realCount;
        }
        public int Width{get;private set;}
        public int Height{get;private set;}
        public TextureContentFormat Format{ get; private set; }

        public bool GenerateMipMaps{ get; private set; }=false;

        public int MipMapCount{ get; private set; }=1;

        public List<TextureContentMipMap> MipMaps{ get; private set; }
    }

    public class TextureContentMipMap
    {
        private System.Drawing.Bitmap bitmap = null;
        private byte[] data = null;

        public TextureContentMipMap(int width, int height, TextureContentFormat format, byte[] data)
            : this(width, height, format)
        {
            this.data = data;
        }

        public TextureContentMipMap(int width, int height, TextureContentFormat format, System.Drawing.Bitmap data)
            : this(width, height, format)
        {
            
            this.bitmap = data;
        }

        protected TextureContentMipMap(int width, int height, TextureContentFormat format)
        {
            Width = width;
            Height = height;
            Format = format;
        }

        public int Width{ get; private set; }

        public int Height{ get; private set; }

        public TextureContentFormat Format{ get; private set; }

        public void Save(engenious.Content.Serialization.ContentWriter writer)
        {
            writer.Write(Width);
            writer.Write(Height);
            writer.Write((int)Format);
            if (bitmap != null)
            {
                using (System.IO.MemoryStream str = new System.IO.MemoryStream())
                {
                    switch (Format)
                    {
                        case TextureContentFormat.Png:
                            bitmap.Save(str, System.Drawing.Imaging.ImageFormat.Png);
                            break;
                        case TextureContentFormat.Jpg:
                            bitmap.Save(str, System.Drawing.Imaging.ImageFormat.Jpeg);
                            break;
                    }

                    writer.Write((int)str.Position);
                    str.Position = 0;
                    writer.Write(str);
                }
            }
            else if(data != null)
            {
                writer.Write(data.Length);
                writer.Write(data);
            }
            else
                throw new InvalidOperationException("Should never happen");
        }
    }
}

