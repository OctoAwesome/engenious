using System.Runtime.InteropServices;

namespace engenious.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionColor:IVertexType
    {
        public static readonly VertexDeclaration VertexDeclaration;

        static VertexPositionColor()
        {
            VertexElement[] elements = { new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0) };
            VertexDeclaration declaration = new VertexDeclaration(elements);
            VertexDeclaration = declaration;
        }

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        public VertexPositionColor(Vector3 position, Color color)
        {
            Color = color;
            Position = position;
        }

        public Vector3 Position;
        public Color Color;
        //public Vector3 Position{ get; private set;}
        //public Color Color{ get; private set;}
    }
}

