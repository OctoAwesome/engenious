using System;
using System.Runtime.InteropServices;
using engenious.Helper;
using OpenTK.Graphics.OpenGL;

namespace engenious.Graphics
{
    public class Texture2DArray : Texture
    {
        private readonly int _texture;
        private readonly PixelInternalFormat _internalFormat;
        public Texture2DArray(GraphicsDevice graphicsDevice, int levels, int width, int height, int layers)
            : base(graphicsDevice)
        {
            using (Execute.OnUiContext)
            {
                _texture = GL.GenTexture();
                
                GL.BindTexture(TextureTarget.Texture2DArray, _texture);
                _internalFormat = PixelInternalFormat.Rgba8;//TODO dynamic format
                GL.TexStorage3D(TextureTarget3d.Texture2DArray, levels, SizedInternalFormat.Rgba8, width, height, Math.Max(layers,1));
            }
            Width = width;
            Height = height;
            LayerCount = layers;
        }
        public Texture2DArray(GraphicsDevice graphicsDevice,int levels,int width,int height,Texture2D[] textures)
            :this(graphicsDevice,levels,width,height,textures.Length)
        {
            using (Execute.OnUiContext)
            {
                var layer=0;

                var createMipMaps=false;
                foreach(var text in textures){
                    if (text.LevelCount < levels)
                        createMipMaps = true;
                    int mipWidth =text.Width,mipHeight=text.Height;
                    for (var i=0;i<1 && createMipMaps || !createMipMaps && i < levels;i++){
                        GL.CopyImageSubData(text.Texture,ImageTarget.Texture2D,i,0,0,0,_texture,ImageTarget.Texture2DArray,i,0,0,layer,mipWidth,mipHeight,1);
                        mipWidth/=2;
                        mipHeight /=2;
                    }
                    layer++;
                }
                if (createMipMaps){
                    GL.GenerateMipmap(GenerateMipmapTarget.Texture2DArray);
                }
                setDefaultTextureParameters();
            }
        }
        private void setDefaultTextureParameters()
        {
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2DArray,TextureParameterName.TextureMagFilter,(int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2DArray,TextureParameterName.TextureMinFilter,(int)All.LinearMipmapLinear);
        }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int LayerCount { get; private set; }

        internal override void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2DArray, _texture);
        }

        public override void BindComputation(int unit = 0)
        {
            GL.BindImageTexture(unit, _texture, 0, false, 0, TextureAccess.WriteOnly,
                (SizedInternalFormat) _internalFormat);
        }

        internal override void SetSampler(SamplerState state)
        {
            using (Execute.OnUiContext)
            {
                state = state ?? SamplerState.LinearClamp;
                GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int) state.AddressU);
                GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int) state.AddressV);
                GL.TexParameter(TextureTarget.Texture2DArray,TextureParameterName.TextureMagFilter,(int)state.TextureFilter);
                GL.TexParameter(TextureTarget.Texture2DArray,TextureParameterName.TextureMinFilter,(int)state.TextureFilter);
            }
        }
        public void SetData<T>(T[] data,int layer,int level=0)where T : struct
        {
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            using (Execute.OnUiContext)
            {
                Bind();
                var pxType = PixelType.UnsignedByte;
                if (typeof(T) == typeof(Color))
                    pxType = PixelType.Float;

                    GL.TexSubImage3D(TextureTarget.Texture2DArray, level, 0, 0,layer, Width, Height,1, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, pxType, handle.AddrOfPinnedObject());
            }
            handle.Free();
        }
    }
}
