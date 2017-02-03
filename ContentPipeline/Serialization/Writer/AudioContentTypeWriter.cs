using engenious.Pipeline;

namespace engenious.Content.Serialization
{
    [ContentTypeWriterAttribute()]
    public class AudioContentTypeWriter: ContentTypeWriter<AudioContent>
    {
        public override void Write(ContentWriter writer, AudioContent value)
        {
            writer.Write((byte)value.OutputFormat);
            writer.Write(value.Data);
        }

        public override string RuntimeReaderName => typeof(Texture2DTypeReader).FullName;
    }
}