using System.IO;
using engenious.Graphics;

namespace engenious.Content.Serialization
{
    public sealed class ContentReader :BinaryReader
    {
        public ContentReader(Stream input)
            : base(input)
        {
        }

        public T Read<T>(ContentManager manager)
        {
            var name = ReadString();
            var typeReader = manager.GetReader(name);
            return typeReader == null ? default(T) : Read<T>(manager, typeReader);
        }

        public T Read<T>(ContentManager manager, IContentTypeReader typeReader)
        {
            return (T)typeReader.Read(manager, this);

        }

        public VertexPositionNormalTexture ReadVertexPositionNormalTexture()
        {
            return new VertexPositionNormalTexture(ReadVector3(), ReadVector3(), ReadVector2());
        }

        public VertexPositionColor ReadVertexPositionColor()
        {
            return new VertexPositionColor(ReadVector3(), ReadColor());
        }

        public VertexPositionColorTexture ReadVertexPositionColorTexture()
        {
            return new VertexPositionColorTexture(ReadVector3(), ReadColor(), ReadVector2());
        }

        public VertexPositionTexture ReadVertexPositionTexture()
        {
            return new VertexPositionTexture(ReadVector3(), ReadVector2());
        }

        public Matrix ReadMatrix()
        {
            return new Matrix(ReadVector4(), ReadVector4(), ReadVector4(), ReadVector4());
        }

        public Quaternion ReadQuaternion()
        {
            return new Quaternion(ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle());
        }

        public Vector2 ReadVector2()
        {
            return new Vector2(ReadSingle(), ReadSingle());
        }

        public Vector3 ReadVector3()
        {
            return new Vector3(ReadSingle(), ReadSingle(), ReadSingle());
        }

        public Vector4 ReadVector4()
        {
            return new Vector4(ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle());
        }

        public Color ReadColor()
        {
            return new Color(ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle());
        }
    }
}

