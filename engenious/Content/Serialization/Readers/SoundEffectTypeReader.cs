using engenious.Audio;

namespace engenious.Content.Serialization
{
    [ContentTypeReaderAttribute(typeof(SoundEffect))]
    public class SoundEffectTypeReader:ContentTypeReader<SoundEffect>
    {
        public override SoundEffect Read(ContentManager manager, ContentReader reader)
        {
            var format = (SoundEffect.AudioFormat)reader.ReadByte();
            return new SoundEffect(reader.BaseStream,format);
        }
    }
}