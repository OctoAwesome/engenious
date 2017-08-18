using engenious.Helper;
using OpenTK.Graphics.OpenGL4;

namespace engenious.Graphics
{
    public class TextureArray : Texture
    {
        protected static int MaxTextureArrays;
        private readonly PixelInternalFormat _internalFormat;
        static TextureArray()
        {
            MaxTextureArrays = GL.GetInteger(GetPName.MaxArrayTextureLayers);
        }

        internal int Texture;
        public TextureArray(GraphicsDevice graphicsDevice,int layerCount=1,int levelCount=1,PixelInternalFormat format=PixelInternalFormat.Rgba8)
            :base(graphicsDevice,levelCount,format)
        {
            _internalFormat = format;
            LayerCount = layerCount;
            Texture=GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2DArray,Texture);
            SetDefaultTextureParameters();
        }
        private static void SetDefaultTextureParameters()
        {
            
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)All.Linear);

            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        }
                public int LayerCount{get;private set;}
        #region implemented abstract members of Texture

        internal override void SetSampler(SamplerState state)
        {
            using (Execute.OnUiContext)
            {
                state = state == null ? SamplerState.LinearClamp : state;
                GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int) state.AddressU);
                GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int) state.AddressV);

                GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int) state.TextureFilter);
                GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int) state.TextureFilter);
            }
        }

        internal override void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2DArray,Texture);
        }
        public override void BindComputation(int unit = 0)
        {
            GL.BindImageTexture(unit, Texture, 0, false, 0, TextureAccess.WriteOnly,
                (SizedInternalFormat) _internalFormat);
        }
        #endregion
    }
}

