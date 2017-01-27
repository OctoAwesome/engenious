using System;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace engenious.Graphics
{
    public class Texture2D : Texture
    {
        internal int texture;
        private PixelInternalFormat _internalFormat;

        public Texture2D(GraphicsDevice graphicsDevice, int width, int height, int mipMaps = 1,
            PixelInternalFormat internalFormat = PixelInternalFormat.Rgba8)
            : base(graphicsDevice, 1, internalFormat)
        {
            _internalFormat = internalFormat;
            Width = width;
            Height = height;
            Bounds = new Rectangle(0, 0, width, height);
            using (Execute.OnUiContext)
            {
                texture = GL.GenTexture();

                Bind();
                setDefaultTextureParameters();
                bool isDepthTarget = ((int) internalFormat >= (int) PixelInternalFormat.DepthComponent16 &&
                                      (int) internalFormat <= (int) PixelInternalFormat.DepthComponent32Sgix);
                if (isDepthTarget)
                    GL.TexImage2D(
                        TextureTarget.Texture2D,
                        0,
                        (OpenTK.Graphics.OpenGL4.PixelInternalFormat) internalFormat,
                        width,
                        height,
                        0,
                        OpenTK.Graphics.OpenGL4.PixelFormat.DepthComponent,
                        PixelType.Float,
                        IntPtr.Zero);
                else
                    GL.TexStorage2D(
                        TextureTarget2d.Texture2D,
                        mipMaps,
                        (OpenTK.Graphics.OpenGL4.SizedInternalFormat) internalFormat,
                        width,
                        height);

                //GL.TexImage2D(TextureTarget.Texture2D, 0, (OpenTK.Graphics.OpenGL4.PixelInternalFormat)internalFormat, width, height, 0, (OpenTK.Graphics.OpenGL4.PixelFormat)Format, PixelType.UnsignedByte, IntPtr.Zero);
            }
        }

        private void setDefaultTextureParameters()
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) All.Linear);
            //if (GL.SupportsExtension ("Version12")) {
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);


            /*} else {
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.Clamp);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.Clamp);
			}*/
        }

        internal override void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, texture);
        }

        public override void BindComputation(int unit = 0)
        {
            GL.BindImageTexture(unit, texture, 0, false, 0, TextureAccess.WriteOnly,
                (OpenTK.Graphics.OpenGL4.SizedInternalFormat) _internalFormat);
        }

        internal override void SetSampler(SamplerState state)
        {
            using (Execute.OnUiContext)
            {
                state = state ?? SamplerState.LinearClamp;
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) state.AddressU);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) state.AddressV);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) state.TextureFilter);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) state.TextureFilter);
            }
        }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public Rectangle Bounds { get; private set; }

        private static PixelType getPixelType(Type type)
        {
            if (type == typeof(float) || type == typeof(double))
            {
                return PixelType.Float;
            }
            else if (type == typeof(int))
            {
                return PixelType.Byte;
            }
            else if (type == typeof(uint))
            {
                return PixelType.UnsignedByte;
            }
            return PixelType.UnsignedByte;
        }

        public void SetData<T>(T[] data, int level = 0) where T : struct //ValueType
        {
            SetData(data, level, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra);
        }

        internal void SetData<T>(T[] data, int level,
            OpenTK.Graphics.OpenGL4.PixelFormat format = OpenTK.Graphics.OpenGL4.PixelFormat.Bgra)
            where T : struct //ValueType
        {
            bool hwCompressed =
                format == (OpenTK.Graphics.OpenGL4.PixelFormat) engenious.Content.TextureContentFormat.DXT1 ||
                format == (OpenTK.Graphics.OpenGL4.PixelFormat) engenious.Content.TextureContentFormat.DXT3 || format ==
                (OpenTK.Graphics.OpenGL4.PixelFormat) engenious.Content.TextureContentFormat.DXT5;
            if (!hwCompressed && Marshal.SizeOf(typeof(T)) * data.Length < Width * Height)
                throw new ArgumentException("Not enough pixel data");

            unsafe
            {
                int width = Width, height = Height;
                for (int i = 0; i < level; i++)
                {
                    width /= 2;
                    height /= 2;
                    if (width == 0 || height == 0)
                        return;
                }
                GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
                using (Execute.OnUiContext)
                {
                    Bind();
                    PixelType pxType = PixelType.UnsignedByte;
                    if (typeof(T) == typeof(Color))
                        pxType = PixelType.Float;
                    if (hwCompressed)
                    {
                        int blockSize = (format ==
                                         (OpenTK.Graphics.OpenGL4.PixelFormat) engenious.Content.TextureContentFormat
                                             .DXT1)
                            ? 8
                            : 16;
                        int mipSize = ((width + 3) / 4) * ((height + 3) / 4) * blockSize;
                        GL.CompressedTexSubImage2D(TextureTarget.Texture2D, level, 0, 0, width, height,
                            (OpenTK.Graphics.OpenGL4.PixelFormat) format, mipSize, handle.AddrOfPinnedObject());
                    }
                    else
                        GL.TexSubImage2D(TextureTarget.Texture2D, level, 0, 0, width, height, format, pxType,
                            handle.AddrOfPinnedObject());
                }
                handle.Free();
            }
            //GL.TexSubImage2D<T> (TextureTarget.Texture2D, 0, 0, 0, Width, Height, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, getPixelType (typeof(T)), data);
        }

        public void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct
        {
            throw new NotImplementedException("Need to implement offset");
            //TODO:GL.TexSubImage2D<T> (TextureTarget.Texture2D, 0, 0, 0, Width, Height, OpenTK.Graphics.OpenGL4.PixelFormat.Rgba, PixelType.UnsignedByte, data);
        }

        public void SetData<T>(int level, Nullable<Rectangle> rect, T[] data, int startIndex, int elementCount)
            where T : struct
        {
            throw new NotImplementedException("Need to implement offset"); //TODO:
            /*if (rect.HasValue)
				GL.TexSubImage2D<T> (TextureTarget.Texture2D, level, rect.Value.X, rect.Value.Y, rect.Value.Width, rect.Value.Height, OpenTK.Graphics.OpenGL4.PixelFormat.Rgba, PixelType.UnsignedByte, data);
			else
				GL.TexSubImage2D<T> (TextureTarget.Texture2D, level, 0, 0, Width, Height, OpenTK.Graphics.OpenGL4.PixelFormat.Rgba, PixelType.UnsignedByte, data);*/
        }

        public void GetData<T>(T[] data) where T : struct //ValueType
        {
            using (Execute.OnUiContext)
            {
                Bind();

                /*if (glFormat == All.CompressedTextureFormats) {
            throw new NotImplementedException ();
        } else {*/
                int level = 0;
                Rectangle? rect = new Rectangle?(); //new Rectangle? (new Rectangle (0, 0, Width, Height));
                if (rect.HasValue)
                {
                    var temp = new T[this.Width * this.Height];
                    GL.GetTexImage(TextureTarget.Texture2D, level, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra,
                        PixelType.UnsignedByte, temp);
                    int z = 0, w = 0;

                    for (int y = rect.Value.Y; y < rect.Value.Y + rect.Value.Height; ++y)
                    {
                        for (int x = rect.Value.X; x < rect.Value.X + rect.Value.Width; ++x)
                        {
                            data[z * rect.Value.Width + w] = temp[(y * Width) + x];
                            ++w;
                        }
                        ++z;
                        w = 0;
                    }
                }
                else
                {
                    GL.GetTexImage(TextureTarget.Texture2D, level, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra,
                        PixelType.UnsignedByte, data);
                }
            }
            //}
        }

        public void GetData<T>(int level, Nullable<Rectangle> rect, T[] data, int startIndex, int elementCount)
            where T : struct
        {
            using (Execute.OnUiContext)
            {
                Bind();
                if (rect.HasValue)
                {
                    //TODO: use ? GL.CopyImageSubData(
                    var temp = new T[this.Width * this.Height];
                    GL.GetTexImage(TextureTarget.Texture2D, level, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra,
                        PixelType.UnsignedByte, temp);
                    int z = 0, w = 0;

                    for (int y = rect.Value.Y; y < rect.Value.Y + rect.Value.Height; ++y)
                    {
                        for (int x = rect.Value.X; x < rect.Value.X + rect.Value.Width; ++x)
                        {
                            data[z * rect.Value.Width + w] = temp[(y * Width) + x];
                            ++w;
                        }
                        ++z;
                        w = 0;
                    }
                }
                else
                {
                    GL.GetTexImage(TextureTarget.Texture2D, level, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra,
                        PixelType.UnsignedByte, data);
                }
            };
        }

        public static Texture2D FromStream(GraphicsDevice graphicsDevice, Stream stream)
        {
            using (Bitmap bmp = new Bitmap(stream))
            {
                return FromBitmap(graphicsDevice, bmp);
            }
        }

        public static Texture2D FromBitmap(GraphicsDevice graphicsDevice, Bitmap bmp, int mipMaps = 1)
        {
            Texture2D text = null;
            text = new Texture2D(graphicsDevice, bmp.Width, bmp.Height, mipMaps);
            BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, text.Width, text.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Execute.OnUiContext)
            {
                text.Bind();
                GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, text.Width, text.Height,
                    OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);
            }
            bmp.UnlockBits(bmpData);


            return text;
        }

        public static Bitmap ToBitmap(Texture2D text)
        {
            Bitmap bmp = new Bitmap(text.Width, text.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, text.Width, text.Height),
                ImageLockMode.WriteOnly, bmp.PixelFormat);
            using (Execute.OnUiContext)
            {
                text.Bind();
                GL.GetTexImage(TextureTarget.Texture2D, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra,
                    PixelType.UnsignedByte, bmpData.Scan0);
            };
            //System.Runtime.InteropServices.Marshal.Copy (bmpData.Scan0, data, 0, data.Length);//TODO: performance
            //TODO: convert pixel formats


            bmp.UnlockBits(bmpData);

            return bmp;
        }

        public static Texture2D FromStream(GraphicsDevice graphicsDevice, Stream stream, int width, int height,
            bool zoom)
        {
            //TODO:implement correct
            using (Bitmap bmp = new Bitmap(stream))
            {
                using (Bitmap scaled = new Bitmap(bmp, width, height))
                {
                    //TODO: zoom?
                    return FromBitmap(graphicsDevice, scaled);
                }
            }
        }

        public override void Dispose()
        {
            using (Execute.OnUiContext)
                GL.DeleteTexture(texture);

            base.Dispose();
        }
    }
}