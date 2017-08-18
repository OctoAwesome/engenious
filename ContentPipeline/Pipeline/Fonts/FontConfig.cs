using System;
using engenious.Helper;

namespace engenious.Pipeline
{
    public abstract class FontConfig
    {
        private static FontConfig _fontConfig;

        public static FontConfig Instance
        {
            get
            {
                if (_fontConfig != null) return _fontConfig;
                switch (PlatformHelper.RunningPlatform())
                {
                    case Platform.Linux:
                        _fontConfig = new FontConfigLinux();
                        break;
                    case Platform.Mac:
                        _fontConfig = new FontConfigMac();
                        break;
                    case Platform.Windows:
                        _fontConfig = new FontConfigWindows();
                        break;
                    default:
                        throw new NotSupportedException();
                }
                return _fontConfig;
            }
        }

        public abstract bool GetFontFile(string fontName, int fontSize, System.Drawing.FontStyle style,
            out string fileName);
    }
}