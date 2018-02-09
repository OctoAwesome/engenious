using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using engenious.Content.Pipeline;

namespace engenious.Pipeline
{
    [ContentImporter(".spritefont", DisplayName = "SpriteFontImporter", DefaultProcessor = "SpriteFontProcessor")]
    public class SpriteFontImporter : ContentImporter<SpriteFontContent>
    {
        #region implemented abstract members of ContentImporter

        public override SpriteFontContent Import(string filename, ContentImporterContext context)
        {
            return new SpriteFontContent(filename);
        }

        #endregion
    }

    public class SpriteFontContent
    {
        public SpriteFontContent(string fileName)
        {
            CharacterRegions = new List<CharacterRegion>();
            var doc = new XmlDocument();
            doc.Load(fileName);
            XmlElement rootNode = null;
            foreach (var node in doc.ChildNodes.OfType<XmlElement>())
            {
                if (node.Name == "EngeniousFont")
                {
                    rootNode = node;
                    break;
                }
            }
            if (rootNode == null)
                throw new FormatException("Not a valid Spritefont file");
            foreach (XmlElement element in rootNode.ChildNodes.OfType<XmlElement>())
            {
                switch (element.Name)
                {
                    case "FontName":
                        FontName = element.InnerText;
                        break;
                    case "Size":
                        Size = int.Parse(element.InnerText);
                        break;
                    case "Spacing":
                        Spacing = int.Parse(element.InnerText);
                        break;
                    case "UseKerning":
                        UseKerning = bool.Parse(element.InnerText);
                        break;
                    case "Style":
                        Style = parseStyle(element.InnerText);
                        break;
                    case "DefaultCharacter":
                        if (element.InnerText.Length > 1)
                            throw new FormatException("MultiChars not allowed");
                        DefaultCharacter = element.InnerText.ToCharArray().FirstOrDefault();
                        break;
                    case "CharacterRegions":
                        ParseCharacterRegion(element);
                        break;
                }
            }
        }

        public void Save(string fileName)
        {
            var doc = new XmlDocument();
            using (var xml = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                xml.Formatting = Formatting.Indented;
                xml.WriteStartDocument();

                xml.WriteStartElement("EngeniousFont");

                xml.WriteElementString("FontName", FontName);
                xml.WriteElementString("Size", Size.ToString());
                xml.WriteElementString("Spacing", Spacing.ToString());
                xml.WriteElementString("UseKerning", UseKerning.ToString());
                xml.WriteElementString("Style", styleToString(Style));
                xml.WriteElementString("DefaultCharacter", DefaultCharacter.ToString());


                xml.WriteStartElement("CharacterRegions");

                foreach (var cr in CharacterRegions)
                    cr.WriteToXml(xml);

                xml.WriteEndElement();


                xml.WriteEndElement();

                xml.WriteEndDocument();
            }
        }

        private void ParseCharacterRegion(XmlElement rootNode)
        {
            foreach (var region in rootNode.ChildNodes.OfType<XmlElement>())
            {
                if (region.Name == "CharacterRegion")
                {
                    string start = null, end = null;
                    foreach (XmlElement element in region.ChildNodes.OfType<XmlElement>())
                    {
                        switch (element.Name)
                        {
                            case "Start":
                                start = element.InnerText;
                                break;
                            case "End":
                                end = element.InnerText;
                                break;
                        }
                    }
                    if (start != null && end != null)
                    {
                        CharacterRegions.Add(new CharacterRegion(start, end,
                            DefaultCharacter ?? '*')); //TODO: default default character
                    }
                }
            }
        }


        private string styleToString(FontStyle fontStyle)
        {
            if (fontStyle == FontStyle.Regular)
                return fontStyle.ToString();
            StringBuilder str = new StringBuilder();
            foreach (var style in Enum.GetValues(typeof(FontStyle)).OfType<FontStyle>())
            {
                if (style == FontStyle.Regular)
                    continue;
                if (fontStyle.HasFlag(style))
                {
                    if (str.Length > 0)
                        str.Append(' ');

                    str.Append(style.ToString());
                }
            }
            return str.ToString();
        }

        private FontStyle parseStyle(string styles)
        {
            FontStyle fontStyle = FontStyle.Regular;
            foreach (var style in styles.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries))
            {
                switch (style)
                {
                    case "Regular":
                        fontStyle |= FontStyle.Regular;
                        break;
                    case "Bold":
                        fontStyle |= FontStyle.Bold;
                        break;
                    case "Italic":
                        fontStyle |= FontStyle.Italic;
                        break;
                    case "Underline":
                        fontStyle |= FontStyle.Underline;
                        break;
                    case "Strikeout":
                        fontStyle |= FontStyle.Strikeout;
                        break;
                }
            }
            return fontStyle;
        }


        public string FontName { get; set; }

        public int Size { get; set; }

        public int Spacing { get; set; }

        public bool UseKerning { get; set; }

        public FontStyle Style { get; set; }

        public char? DefaultCharacter { get; set; }

        public List<CharacterRegion> CharacterRegions { get; set; }
    }

    public class CharacterRegion : IEquatable<CharacterRegion>
    {
        private static int ParseAddress(string characterAddress)
        {
            if (characterAddress.StartsWith("0x"))
                return Convert.ToInt32(characterAddress.Substring(2), 16);
            return int.Parse(characterAddress);
        }

        private static char ToChar(int characterAddress)
        {
            char[] value = Encoding.Unicode.GetChars(BitConverter.GetBytes(characterAddress));

            return value[0];
        }

        private char _defaultChar;

        public CharacterRegion(string start, string end, char defaultChar = '*')
            : this(ParseAddress(start), ParseAddress(end), defaultChar)
        {
        }

        public CharacterRegion(int start, int end, char defaultChar = '*')
        {
            Start = start;
            End = end;
            _defaultChar = defaultChar;
        }

        public IEnumerable<char> GetChararcters()
        {
            for (int i = Start; i <= End; i++)
            {
                yield return ToChar(i);
            }
        }

        internal void WriteToXml(XmlTextWriter xml)
        {
            xml.WriteStartElement("CharacterRegion");

            xml.WriteElementString("Start", Start.ToString());
            xml.WriteElementString("End", End.ToString());

            xml.WriteEndElement();
        }

        public int Start { get; }

        public int End { get; }

        public bool Equals(CharacterRegion other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _defaultChar == other._defaultChar && Start == other.Start && End == other.End;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CharacterRegion) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Start;
                hashCode = (hashCode * 397) ^ End;
                return hashCode;
            }
        }
    }
}