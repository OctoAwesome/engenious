using System;
using engenious.Graphics;
using OpenTK;

namespace engenious.Content.Serialization
{
    [ContentTypeReaderAttribute(typeof(SpriteFont))]
    public class SpriteFontTypeReader:ContentTypeReader<SpriteFont>
    {
        public override SpriteFont Read(ContentManager manager, ContentReader reader)
        {
            Texture2D texture = reader.Read<Texture2D>(manager);
            SpriteFont font = new SpriteFont(texture);

            font.Spacing = reader.ReadSingle();
            font.LineSpacing = reader.ReadInt32();
            font.BaseLine = reader.ReadInt32();
            bool hasDefaultChar = reader.ReadBoolean();
            if (hasDefaultChar)
                font.DefaultCharacter = reader.ReadChar();
            else
                font.DefaultCharacter = null;

            int kerningCount = reader.ReadInt32();

            for (int i = 0; i < kerningCount; i++)
            {
                int key = reader.ReadInt32();
                int kerning = reader.ReadInt32();
                font.Kernings.Add(key, kerning);
            }
            int characterMapCount = reader.ReadInt32();
            for (int i = 0; i < characterMapCount; i++)
            {

                char key = reader.ReadChar();
                Vector2 offset = reader.ReadVector2();
                FontCharacter fntChar = new FontCharacter(key, new RectangleF(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()), offset, reader.ReadSingle());
                font.CharacterMap.Add(key, fntChar);
            }

            return font;
        }
    }
}

