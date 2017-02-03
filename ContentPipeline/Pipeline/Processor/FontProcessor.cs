using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using engenious.Graphics;

namespace engenious.Content.Pipeline
{
    public class CompiledSpriteFont
    {
        public CompiledSpriteFont()
        {
            Kernings = new Dictionary<int, int>();
            CharacterMap = new Dictionary<char, FontCharacter>();
        }

        internal Dictionary<int, int> Kernings;
        internal Dictionary<char, FontCharacter> CharacterMap;
        internal TextureContent Texture;

        public char? DefaultCharacter { get; set; }

        public int LineSpacing { get; set; }

        public float Spacing { get; set; }

        public int BaseLine { get; set; }
    }

    [ContentProcessor(DisplayName = "Font Processor")]
    public class FontProcessor : ContentProcessor<FontContent, CompiledSpriteFont>
    {
        public override CompiledSpriteFont Process(FontContent input,string filename, ContentProcessorContext context)
        {
            try
            {
                CompiledSpriteFont font = new CompiledSpriteFont();
                var text = new Bitmap(input.TextureFile);
                var textData = text.LockBits(new System.Drawing.Rectangle(0,0,text.Width,text.Height),ImageLockMode.ReadOnly,System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                font.Texture = new TextureContent(context.GraphicsDevice,false,1,textData.Scan0,text.Width,text.Height,TextureContentFormat.Png,TextureContentFormat.Png);
                text.UnlockBits(textData);  
                text.Dispose();

                string[] lines = input.Content.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                int lineOffset = 0;

                if (!lines[lineOffset].StartsWith("common "))
                    throw new Exception("No common data found");

                {
                    string[] splt = lines[lineOffset].Substring("common ".Length).Split(' ');
                    foreach (string pair in splt)
                    {
                        string[] kv = pair.Split(new[] { '=' }, 2);
                        if (kv.Length == 1)
                            throw new Exception("Invalid common data");
                        if (kv[0] == "lineHeight")
                            font.LineSpacing = int.Parse(kv[1]);
                        else if (kv[0] == "base")
                            font.BaseLine = int.Parse(kv[1]);
                    }
                }

                while (lineOffset < lines.Length)
                {
                    if (lines[lineOffset].StartsWith("chars count="))
                        break;
                    lineOffset++;
                }
                if (lineOffset >= lines.Length)
                    throw new Exception("Invalid char count");
                int charCount = int.Parse(lines[lineOffset++].Substring("chars count=".Length));

                Dictionary<int, char> idCharMap = new Dictionary<int, char>();
                for (int i = 0; i < charCount - 1; i++)
                {
                    string line = lines[lineOffset];
                    if (!line.StartsWith("char id="))
                        throw new Exception("Invalid char definition");
                    string[] splt = line.Substring("char ".Length).Split(new[] { ' ' }, 11);

                    int id = 0;//x=2 y=2 width=25 height=80 xoffset=0 yoffset=15 xadvance=28 page=0 chnl=0 letter="}"
                    int x = 0, y = 0, width = 0, height = 0;
                    int xOffset = 0, yOffset = 0;
                    int advance = 0;
                    char letter = '\0';
                    foreach (string pair in splt)
                    {
                        string[] pairSplit = pair.Split(new[] { '=' }, 2);
                        string key = pairSplit[0].ToLower();
                        string value = pairSplit[1];

                        if (key == "id")
                        {
                            id = int.Parse(value);
                        }
                        else if (key == "x")
                        {
                            x = int.Parse(value);
                        }
                        else if (key == "y")
                        {
                            y = int.Parse(value);
                        }
                        else if (key == "width")
                        {
                            width = int.Parse(value);
                        }
                        else if (key == "height")
                        {
                            height = int.Parse(value);
                        }
                        else if (key == "xoffset")
                        {
                            xOffset = int.Parse(value);
                        }
                        else if (key == "yoffset")
                        {
                            yOffset = int.Parse(value);
                        }
                        else if (key == "xadvance")
                        {
                            advance = int.Parse(value);
                        }
                        else if (key == "letter")
                        {
                            letter = value.Trim().ToCharArray()[1];
                        }

                    }
                    lineOffset++;
                    if (idCharMap.ContainsKey(id))
                        continue;
                    idCharMap.Add(id, letter);
                    FontCharacter fontChar = new FontCharacter(letter, new Rectangle(0, 0, font.Texture.Width, font.Texture.Height), new Rectangle(x, y, width, height), new Vector2(xOffset, yOffset), advance);

                    if (font.CharacterMap.ContainsKey(letter))
                        continue;
                    font.CharacterMap.Add(letter, fontChar);

                }
                int kerningCount = int.Parse(lines[lineOffset++].Substring("kernings count=".Length));

                for (int i = 0; i < kerningCount; i++)
                {
                    string line = lines[lineOffset];
                    if (!line.StartsWith("kerning "))
                        throw new Exception("Invalid kerning definition");
                    string[] splt = line.Substring("kerning ".Length).Split(' ');
                    char first = '\0', second = '\0';
                    int amount = 0;
                    foreach (string pair in splt)
                    {
                        string[] pairSplit = pair.Split('=');
                        string key = pairSplit[0].ToLower();
                        string value = pairSplit[1];
                        if (key == "first")
                        {
                            first = idCharMap[int.Parse(value)];
                        }
                        else if (key == "second")
                        {
                            second = idCharMap[int.Parse(value)];
                        }
                        else if (key == "amount")
                        {
                            amount = int.Parse(value);
                        }
                    }
                    int kerningKey = SpriteFont.GetKerningKey(first, second);
                    if (!font.Kernings.ContainsKey(kerningKey))
                        font.Kernings.Add(kerningKey, amount);
                    lineOffset++;

                }

                return font;
            }
            catch (Exception ex)
            {
                context.RaiseBuildMessage(filename , ex.Message, BuildMessageEventArgs.BuildMessageType.Error);
            }
            return null;

        }
    }
}

