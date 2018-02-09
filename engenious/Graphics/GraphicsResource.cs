using System;

namespace engenious.Graphics
{
    public abstract class GraphicsResource : IDisposable
    {
        protected GraphicsResource()
        {
        }

        protected GraphicsResource(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
        }

        public GraphicsDevice GraphicsDevice{ get; internal set; }

        public string Name { get; set; }

        public object Tag{ get; set; }

        public bool IsDisposed{ get; private set; }

        public virtual void Dispose()
        {
            if (IsDisposed)
                return;
			
            IsDisposed = true;
        }
    }
}

