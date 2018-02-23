using System.Runtime.InteropServices;

namespace engenious.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionNormalColor:IPositionVertex,INormalVertex,IColorVertex
    {
        public static readonly VertexDeclaration VertexDeclaration;

        static VertexPositionNormalColor()
        {
            VertexElement[] elements = { new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0), new VertexElement(24, VertexElementFormat.Vector3, VertexElementUsage.Color, 0) };
            var declaration = new VertexDeclaration(elements);
            VertexDeclaration = declaration;
        }

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        public VertexPositionNormalColor(Vector3 position, Vector3 normal,Color color)
        {
            Normal = normal;
            Position = position;
            Color = color;
        }

        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }
        public Color Color { get; set; }
        //public Vector3 Position{ get; private set;}
        //public Color Color{ get; private set;}
    }
}