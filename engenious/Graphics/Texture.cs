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
                _samplerState = value ?? SamplerState.LinearWrap;
                SetSampler(_samplerState);
            }
        }

        public PixelInternalFormat Format { get; protected set; }

        public int LevelCount { get; protected set; }

        internal abstract void SetSampler(SamplerState state);

        internal abstract void Bind();

        public abstract void BindComputation(int unit=0);
    }
}

