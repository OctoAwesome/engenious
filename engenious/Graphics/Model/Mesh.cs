namespace engenious.Graphics
{
    public class Mesh:GraphicsResource
    {
        public Mesh(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
        }

        public int PrimitiveCount{ get; set; }

        public VertexBuffer VB{ get; set; }

        public BoundingBox BoundingBox{get;internal set;}

        public void Draw()
        {
            GraphicsDevice.VertexBuffer = VB;
            GraphicsDevice.DrawPrimitives(PrimitiveType.Triangles, 0, PrimitiveCount);
        }
    }
}

