using System.Runtime.InteropServices;

namespace engenious.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct VertexPositionNormalColorTexture:IPositionVertex,INormalVertex,IColorVertex,ITextureCoordinatesVertex
    {
        public static readonly VertexDeclaration VertexDeclaration;

        static VertexPositionNormalColorTexture()
        {
            VertexElement[] elements = { new VertexElement (0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),new VertexElement (12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),new VertexElement (24, VertexElementFormat.Color, VertexElementUsage.Color, 0), new VertexElement (40, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0) };
            var declaration = new VertexDeclaration (elements);
            VertexDeclaration = declaration;
        }
        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        public VertexPositionNormalColorTexture (Vector3 position,Vector3 normal,Color color,Vector2 textureCoord)
        {
            Position = position;
            Normal = normal;
            Color = color;
            TextureCoordinate = textureCoord;
        }
			
        public Vector3 Position{ get; set;}
        public Vector3 Normal{ get; set;}
        public Color Color{ get; set;}
        public Vector2 TextureCoordinate{ get; set;}
    }
}

