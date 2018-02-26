using OpenTK.Graphics.OpenGL;

namespace engenious.Graphics
{
    public abstract class Texture:GraphicsResource
    {
        protected Texture(GraphicsDevice graphicsDevice, int levelCount = 1, PixelInternalFormat format = PixelInternalFormat.Rgba8)
            : base(graphicsDevice)
        {
            LevelCount = levelCount;
            Format = format;
        }

        private SamplerState _samplerState;
        public SamplerState SamplerState{
            get{
                return _samplerState;
            }
            set
            {
                if (_samplerState == value)
                    return;
                _samplerState?.Unbind();
                var samplerState = value ?? SamplerState.LinearWrap;
                samplerState.Bind(this);
                _samplerState = samplerState;
            }
        }

        internal abstract TextureTarget Target { get; }

        public PixelInternalFormat Format { get; protected set; }

        public int LevelCount { get; protected set; }

        internal abstract void Bind();

        public abstract void BindComputation(int unit=0);
    }
}

