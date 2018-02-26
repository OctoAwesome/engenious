using engenious.Helper;
using OpenTK.Graphics.OpenGL;

namespace engenious.Graphics
{
    public abstract class TextureArray : Texture
    {
        protected static int MaxTextureArrays;
        private readonly PixelInternalFormat _internalFormat;
        static TextureArray()
        {
            MaxTextureArrays = GL.GetInteger(GetPName.MaxArrayTextureLayers);
        }

        internal int Texture;

        protected TextureArray(GraphicsDevice graphicsDevice,int layerCount=1,int levelCount=1,PixelInternalFormat format=PixelInternalFormat.Rgba8)
            :base(graphicsDevice,levelCount,format)
        {
            _internalFormat = format;
            LayerCount = layerCount;
            Texture=GL.GenTexture();
            GL.BindTexture(Target,Texture);
            SetDefaultTextureParameters();
        }
        private void SetDefaultTextureParameters()
        {
            GL.TexParameter(Target, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(Target, TextureParameterName.TextureMagFilter, (int)All.Linear);

            GL.TexParameter(Target, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(Target, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        }
        public int LayerCount{get;private set;}
        #region implemented abstract members of Texture

        internal override void Bind()
        {
            GL.BindTexture(Target,Texture);
        }
        public override void BindComputation(int unit = 0)
        {
            GL.BindImageTexture(unit, Texture, 0, false, 0, TextureAccess.WriteOnly,
                (SizedInternalFormat) _internalFormat);
        }
        #endregion
    }
}

