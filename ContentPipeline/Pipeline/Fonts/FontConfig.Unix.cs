using System;
using System.Collections.Generic;
using System.Linq;

namespace engenious.Pipeline
{
    public class FontConfigUnix:FontConfig
    {
        protected FontConfigUnix()
        {
        }
        protected enum FcMatchKind {
            FcMatchPattern = 0,
            FcMatchFont = 1
        }
        protected enum FcResult {
            FcResultMatch = 0,
            FcResultNoMatch = 1,
            FcResultTypeMismatch = 2,
            FcResultNoId = 3,
            FcResultOutOfMemory = 4
        }
        protected delegate IntPtr FcFontMatchDelegate(IntPtr config,IntPtr pattern,out FcResult result);
        protected delegate FcResult FcPatternGetStringDelegate(IntPtr pattern, string name, int n, out IntPtr resultString);
        protected const string FcFile = "file";
        protected Func<IntPtr> FcInitLoadConfigAndFonts;
        protected Func<IntPtr> FcPatternCreate;
        protected Func<string,IntPtr> FcNameParse;
        protected Func<IntPtr,IntPtr,FcMatchKind,bool> FcConfigSubstitute;
        protected Action<IntPtr> FcDefaultSubstitute;
        protected FcFontMatchDelegate FcFontMatch;
        protected Action<IntPtr> FcPatternDestroy;
        protected FcPatternGetStringDelegate FcPatternGetString;

        #region implemented abstract members of FontConfig

        public override bool GetFontFile(string fontName, int fontSize, System.Drawing.FontStyle style,out string fileName)
        {
            fileName = null;
            var config = FcInitLoadConfigAndFonts();

            var fontStyles=new List<string>();
            foreach(var val in Enum.GetValues(typeof(System.Drawing.FontStyle)).OfType<System.Drawing.FontStyle>().Skip(1))
            {
                if (style.HasFlag(val))
                    fontStyles.Add(val.ToString().ToLower());
            }

            // configure the search pattern, 
            // assume "name" is a std::string with the desired font name in it
            string styles = string.Join(":",fontStyles);
            var pat = FcNameParse(fontName+"-"+fontSize.ToString()+":"+styles);
            FcConfigSubstitute(config, pat, FcMatchKind.FcMatchPattern);
            FcDefaultSubstitute(pat);

            // find the font
            FcResult tst;
            var font = FcFontMatch(IntPtr.Zero, pat, out tst);
            if (font != IntPtr.Zero)
            {
                IntPtr resultPtr;
                if (FcPatternGetString(font, FcFile, 0, out resultPtr) == FcResult.FcResultMatch)
                {
                    // save the file to another std::string
                    fileName = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(resultPtr);
                }

                FcPatternDestroy(font);
            }

            FcPatternDestroy(pat);

            return true;
        }

        #endregion
    }
}

