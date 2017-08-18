using engenious.Graphics;

namespace engenious.Content.Serialization
{
    [ContentTypeReaderAttribute(typeof(SpriteFont))]
    public class SpriteFontTypeReader:ContentTypeReader<SpriteFont>
    {
        public override SpriteFont Read(ContentManager manager, ContentReader reader)
        {
            var texture = reader.Read<Texture2D>(manager);
            var font = new SpriteFont(texture);

            font.Spacing = reader.ReadSingle();
            font.LineSpacing = reader.ReadInt32();
            font.BaseLine = reader.ReadInt32();
            var hasDefaultChar = reader.ReadBoolean();
            if (hasDefaultChar)
                font.DefaultCharacter = reader.ReadChar();
            else
                font.DefaultCharacter = null;

            var kerningCount = reader.ReadInt32();

            for (var i = 0; i < kerningCount; i++)
            {
                var key = reader.ReadInt32();
                var kerning = reader.ReadInt32();
                font.Kernings.Add(key, kerning);
            }
            var characterMapCount = reader.ReadInt32();
            for (var i = 0; i < characterMapCount; i++)
            {

                var key = reader.ReadChar();
                var offset = reader.ReadVector2();
                var fntChar = new FontCharacter(key, new RectangleF(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()), offset, reader.ReadSingle());
                font.CharacterMap.Add(key, fntChar);
            }

            return font;
        }
    }
}

