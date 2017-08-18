using System.Runtime.InteropServices;

namespace engenious.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPosition:IVertexType
    {
        public static readonly VertexDeclaration VertexDeclaration;

        static VertexPosition()
        {
            VertexElement[] elements = { new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0) };
            var declaration = new VertexDeclaration(elements);
            VertexDeclaration = declaration;
        }

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        public VertexPosition(Vector3 position)
        {
            Position = position;
        }

        public Vector3 Position;
    }
}

