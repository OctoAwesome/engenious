using System.Runtime.InteropServices;

namespace engenious.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionColorTexture:IPositionVertex,IColorVertex,ITextureCoordinatesVertex
    {
        public static readonly VertexDeclaration VertexDeclaration;

        static VertexPositionColorTexture()
        {
            VertexElement[] elements = {
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector4, VertexElementUsage.Color, 0),
                new VertexElement(28, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
            };
            var declaration = new VertexDeclaration(elements);
            VertexDeclaration = declaration;
        }

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        public VertexPositionColorTexture(Vector3 position, Color color, Vector2 textureCoord)
        {
            TextureCoordinate = textureCoord;
            Position = position;
            Color = color;


        }

        public Vector3 Position { get; set; }
        public Color Color { get; set; }
        public Vector2 TextureCoordinate { get; set; }
        //public Vector3 Position{ get; private set; }

        //public Color Color{ get; private set; }

        //public Vector2 TextureCoordinate{ get; private set; }
    }
}

