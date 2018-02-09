using System.Runtime.InteropServices;

namespace engenious.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionNormal:IVertexType
    {
        public static readonly VertexDeclaration VertexDeclaration;

        static VertexPositionNormal()
        {
            VertexElement[] elements = { new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0) };
            var declaration = new VertexDeclaration(elements);
            VertexDeclaration = declaration;
        }

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        public VertexPositionNormal(Vector3 position, Vector3 normal)
        {
            Normal = normal;
            Position = position;
        }

        public Vector3 Position;
        public Vector3 Normal;
        //public Vector3 Position{ get; private set;}
        //public Color Color{ get; private set;}
    }
}

