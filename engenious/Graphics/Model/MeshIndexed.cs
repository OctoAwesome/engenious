namespace engenious.Graphics
{
    public class MeshIndexed : BaseMesh
    {
        public MeshIndexed(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
        }
        public VertexBuffer VB{ get; set; }
        public IndexBuffer IB{ get; set; }
        public override void Draw()
        {
            GraphicsDevice.VertexBuffer = VB;
            GraphicsDevice.IndexBuffer = IB;
            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.Triangles, 0,0, VB.VertexCount,0,PrimitiveCount);
        }
    }
}