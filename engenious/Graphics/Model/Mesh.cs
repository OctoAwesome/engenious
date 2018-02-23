namespace engenious.Graphics
{
    public class Mesh:BaseMesh
    {
        public Mesh(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
        }
        public VertexBuffer VB{ get; set; }


        public override void Draw()
        {
            GraphicsDevice.VertexBuffer = VB;
            GraphicsDevice.DrawPrimitives(PrimitiveType.Triangles, 0, VB.VertexCount);
        }
    }
}

