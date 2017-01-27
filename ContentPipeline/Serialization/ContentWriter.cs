using System;
using System.IO;

namespace engenious.Content.Serialization
{
    public sealed class ContentWriter : BinaryWriter
    {

        public ContentWriter(Stream output)
            : base(output)
        {
        }

        public void WriteObject(object value)
        {
            IContentTypeWriter typeWriter = SerializationManager.Instance.GetWriter(value.GetType());
            Write(typeWriter.RuntimeReaderName);
            typeWriter.Write(this, value);
        }

        public void WriteObject<T>(T value)
        {
            WriteObject<T>(value, SerializationManager.Instance.GetWriter(typeof(T)));
        }

        public void WriteObject<T>(T value, IContentTypeWriter typeWriter)
        {
            Write(typeWriter.RuntimeReaderName);
            typeWriter.Write(this, value);
        }

        public void Write(Stream stream, int length = -1)
        {
            System.IO.Stream buffered = new BufferedStream(stream);
            byte[] buffer = new byte[1024*1024];
            int readLen = length == -1 ? (int)buffer.Length : (int)stream.Length;
            int toRead = Math.Min(readLen, buffer.Length);
            int read;
            while ((read = buffered.Read(buffer, 0, toRead)) > 0)
            {
                Write(buffer, 0, read);
            }
            if (read > 0)
                Write(buffer, 0, read);
            buffered.Close();
            buffered.Dispose();
        }

        public void Write(engenious.Graphics.VertexPositionNormalTexture v)
        {
            Write(v.Position);
            Write(v.Normal);
            Write(v.TextureCoordinate);
        }

        public void Write(engenious.Graphics.VertexPositionColor v)
        {
            Write(v.Position);
            Write(v.Color);
        }

        public void Write(engenious.Graphics.VertexPositionColorTexture v)
        {
            Write(v.Position);
            Write(v.Color);
            Write(v.TextureCoordinate);
        }

        public void Write(engenious.Graphics.VertexPositionTexture v)
        {
            Write(v.Position);
            Write(v.TextureCoordinate);
        }

        public void Write(Matrix matrix)
        {
            Write(matrix.Row0);//TODO: perhaps better saving per Column?
            Write(matrix.Row1);
            Write(matrix.Row2);
            Write(matrix.Row3);
        }

        public void Write(Quaternion quaternion)
        {
            Write(quaternion.X);
            Write(quaternion.Y);
            Write(quaternion.Z);
            Write(quaternion.W);
        }

        public void Write(Vector2 vector)
        {
            Write(vector.X);
            Write(vector.Y);
        }

        public void Write(Vector3 vector)
        {
            Write(vector.X);
            Write(vector.Y);
            Write(vector.Z);
        }

        public void Write(Vector4 vector)
        {
            Write(vector.X);
            Write(vector.Y);
            Write(vector.Z);
            Write(vector.W);
        }

        public void Write(Color color)
        {
            Write(color.R);
            Write(color.G);
            Write(color.B);
            Write(color.A);
        }
    }
}

