namespace engenious.Graphics
{
    public interface IMesh
    {
        BoundingBox BoundingBox{get;}
        int PrimitiveCount{ get; }

        void Draw();
    }
}