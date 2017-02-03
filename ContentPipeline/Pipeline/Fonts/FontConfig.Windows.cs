using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace engenious.Pipeline
{
    public class FontConfigWindows : FontConfig
	{
        private readonly Dictionary<string,string> _fontFileMap = new Dictionary<string, string>();

        private static bool FindFondFile(ref string fileName)
        {
            if (Path.GetExtension(fileName) != ".ttf") return false;
            string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), fileName);
            if (File.Exists(file))
            {
                fileName = file;
                return true;
            }
            if (File.Exists(fileName)) return true;
            return false;
        }
		public FontConfigWindows()
		{
		    var fontKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts");
		    if (fontKey == null)
		        return;
		    foreach (string fontName in fontKey.GetValueNames())
		    {
		        var value = fontKey.GetValue(fontName, null);
		        if (value == null) continue;
		        var file = value.ToString();
		        if (FindFondFile(ref file))
		        {
		            string name = fontName;
		            if (name.EndsWith(" (TrueType)"))
                        name = name.Substring(0, name.Length - " (TrueType)".Length);//TODO: better solution?
		            _fontFileMap.Add(name, file);
		        }
		    }
		}

        #region implemented abstract members of FontConfig
       

        public override bool GetFontFile(string fontName, int fontSize, FontStyle style,out string fileName)
        {
            fileName = null;

            Font fnt = new Font(fontName, fontSize, style, GraphicsUnit.Point);

            var names = GetFontNames(fnt);
            foreach (var name in names)
            {
                if (_fontFileMap.TryGetValue(name.Name, out fileName))
                    break;
            }
            //Check if requested Font != Default Font
            return fnt.OriginalFontName == FontFamily.GenericSansSerif.Name || fnt.OriginalFontName == fnt.Name;
        }

        private List<NameRecord> GetFontNames(Font font)
        {
            return GetFontNames(font.ToHfont());
        }
        private List<NameRecord> GetFontNames(IntPtr hFont)
        {
            var fontNames = new List<NameRecord>();
            IntPtr dc = GetDC(IntPtr.Zero);

            SelectObject(dc, hFont);

            using (BinaryReader br = new BinaryReader(new MemoryStream(LoadFontMetricsNameTable(dc))))
            {
                // Read selector (always = 0) to advance reader position by 2 bytes

                ToLittleEndian(br.ReadUInt16());//ushort selector =

                // Get number of records and offset byte value, from where font descriptions start

                ushort records = ToLittleEndian(br.ReadUInt16());
                ushort offset = ToLittleEndian(br.ReadUInt16());

                // Get the correct name record
                for (ushort i = 0; i < records; i++)
                {
                    NameRecord nameRecord = new NameRecord(br);
                    if (nameRecord.IsWindowsUnicodeFullFontName)
                        fontNames.Add(nameRecord);
                }
                foreach(var record in fontNames)
                    record.ReadName(br,offset);


                br.Close();
            }

            ReleaseDC(IntPtr.Zero, dc);

            return fontNames;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);
        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        private static extern IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);
        [DllImport("gdi32.dll")]
        private static extern uint GetFontData(IntPtr hdc, uint dwTable, uint dwOffset, [Out] byte[] lpvBuffer, uint cbData);

        private const uint NameTableKey = 0x656D616E;
        /// <summary>
        /// Loads the font metrics name table as raw bytes from the font in the <paramref name="dc"/>.
        /// </summary>
        /// <param name="dc">The DC hosting the Font.</param>
        /// <returns>The metrics table as raw bytes.</returns>
        private static byte[] LoadFontMetricsNameTable(IntPtr dc)
        {
            // First determine the amount of bytes in the font metrics name table

            uint byteCount = GetFontData(dc, NameTableKey, 0, null, 0);
            // Now init the byte array and load the data by calling GetFontData once again

            var fontData = new byte[byteCount];
            GetFontData(dc, NameTableKey, 0, fontData, byteCount);

            return fontData;
        }
        /// <summary>
        /// Helper function to convert any <see cref="ushort"/> value in font metrics name table to little endian,
        ///  because all stored in big endian.
        /// </summary>
        /// <param name="value">The value to be converted into little endian byte order.</param>
        /// <returns>The corresponding <see cref="ushort"/> value in little endian byte order.</returns>
        private static ushort ToLittleEndian(ushort value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return (ushort) (value << 8 | value >> 8);
            }

            return value;
        }


        #endregion

        #region TTF NameRecord Class

        /// <summary>
        /// Encapsulates a name record as specified in True Type font specification for the font metrics name table
        /// http://www.microsoft.com/typography/otspec/name.htm
        /// </summary>
        private class NameRecord
        {

            private readonly ushort _platformId;
            private readonly ushort _encodingId;
            private readonly ushort _nameId;
            private readonly ushort _nameLength;
            private readonly ushort _byteOffset;

            public string Name { get; private set; }




            /// <summary>
            /// Gets a value indicating whether this <see cref="NameRecord"/> represents a Windows Unicode full font name.
            /// </summary>
            /// <value>
            ///     <c>true</c> if this <see cref="NameRecord"/> represents a Windows Unicode full font name; otherwise, <c>false</c>.
            /// </value>
            internal bool IsWindowsUnicodeFullFontName => _platformId == 3 && _encodingId == 1 && _nameId == 4;

            /// <summary>
            /// Initializes a new instance of the <see cref="NameRecord"/> class.
            /// </summary>
            /// <param name="br">The <see cref="BinaryReader"/> for interpretation of the bytes.</param>
            internal NameRecord(BinaryReader br)
            {
                // Read the unsigned 16-bit integers and convert to little endian

                _platformId = ToLittleEndian(br.ReadUInt16());
                _encodingId = ToLittleEndian(br.ReadUInt16());

                // Only read to advance reader position by 2 bytes

                ToLittleEndian(br.ReadUInt16());

                _nameId = ToLittleEndian(br.ReadUInt16());
                _nameLength = ToLittleEndian(br.ReadUInt16());
                _byteOffset = ToLittleEndian(br.ReadUInt16());
            }

            internal void ReadName(BinaryReader br, int recordOffset)
            {
                // Search the start position of the font name

                int totalOffset = recordOffset + _byteOffset;
                br.BaseStream.Seek(totalOffset, SeekOrigin.Begin);

                // Now read the amount of bytes specified in the name record
                // and convert to a string

                byte[] nameBytes = br.ReadBytes(_nameLength);
                Name = Encoding.GetEncoding(1201).GetString((nameBytes));
            }

            /// <summary>
            /// Returns a <see cref="System.String"/> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                return
                    $"Name = {Name ?? "{Not Available}"};NameRecord - Key; Platform ID = {_platformId}, Encoding ID = {_encodingId}, Name ID = {_nameId}";
            }

        }

        #endregion
    }
}

