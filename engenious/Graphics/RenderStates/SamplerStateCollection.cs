using engenious.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace engenious
{
    public class SamplerStateCollection : GraphicsResource
    {
        private readonly SamplerState[] _samplerStates;

        internal SamplerStateCollection(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            var maxTextures = GL.GetInteger(GetPName.MaxTextureImageUnits);
            _samplerStates = new SamplerState[maxTextures];
        }

        public SamplerState this [int index]
        {
            get { return _samplerStates[index]; }
            set
            {
                if (_samplerStates[index] != value)
                {
                    _samplerStates[index] = value ?? SamplerState.LinearClamp;
                    GL.ActiveTexture(TextureUnit.Texture0 + index);
                    if (GraphicsDevice.Textures[index] != null)
                        GraphicsDevice.Textures[index].SetSampler(value);
                    
                }
            }
        }
    }
}

