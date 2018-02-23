namespace engenious.Graphics
{
    public class MeshContent
    {
        public int PrimitiveCount{ get; set; }

        public ConditionalVertexArray Vertices { get; set; }


        public bool HasIndices => Indicies != null;
        public int[] Indicies { get; set; }
    }
}

