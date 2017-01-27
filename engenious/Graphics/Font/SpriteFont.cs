using System;
using System.Collections.ObjectModel;
using System.Text;
using OpenTK;
using System.Collections.Generic;
using System.Linq;

namespace engenious.Graphics
{
    public sealed class SpriteFont : IDisposable
    {
        internal Dictionary<int,int> kernings;
        internal Dictionary<char,FontCharacter> characterMap;
        internal Texture2D texture;

        internal SpriteFont(Texture2D texture)
        {
            this.texture = texture;
            kernings = new Dictionary<int, int>();
            characterMap = new Dictionary<char, FontCharacter>();
        }

        internal static int getKerningKey(char first, char second)
        {
            return (int)first << 16 | (int)second;
        }

        ReadOnlyCollection<char> characters;

        public ReadOnlyCollection<char> Characters
        {
            get
            {
                if (characters == null)
                {
                    characters = new ReadOnlyCollection<char>(characterMap.Keys.ToList());
                }
                return characters;
            }
        }

        public Nullable<char> DefaultCharacter { get; set; }

        public int LineSpacing { get; set; }

        public int BaseLine{ get; internal set; }

        public float Spacing { get; set; }

        public Vector2 MeasureString(StringBuilder text)
        {
            return MeasureString(text.ToString());
        }

        public Vector2 MeasureString(string text)
        {
            float width = 0.0f;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                FontCharacter fontChar;
                if (!characterMap.TryGetValue(c, out fontChar))
                {
                    if (!DefaultCharacter.HasValue || !characterMap.TryGetValue(DefaultCharacter.Value, out fontChar))
                    {
                        continue;
                    }
                }

                if (fontChar == null)
                    continue;
                width += fontChar.Advance;
                if (i < text.Length - 1)
                {
                    int kerning = 0;
                    if (kernings.TryGetValue(SpriteFont.getKerningKey(c, text[i + 1]), out kerning))
                        width += kerning;
                }
            }
            return new Vector2(width, LineSpacing);//TODO height?
        }

        public void Dispose()
        {
            texture?.Dispose();
        }
    }
}

