namespace engenious.Graphics
{
    public abstract class BaseMesh : GraphicsResource,IMesh
    {
        protected BaseMesh(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            
        }
        public BoundingBox BoundingBox { get; protected internal set; }
        public int PrimitiveCount { get;protected internal set; }
        public abstract void Draw();
    }
}