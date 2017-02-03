namespace engenious
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
    public struct Color
    {
        private Vector4 _color;

        public Color(byte r, byte g, byte b, byte a = 255)
        {
            _color = new Vector4(r / 255f, g / 255f, b / 255f, a / 255f);
        }

        public Color(float r, float g, float b, float a = 1.0f)
        {
            _color = new Vector4(r, g, b, a);
        }

        public Color(Color color, float a)
            : this(color.R, color.G, color.B, a)
        {
        }

        public Color(System.Drawing.Color color)
            : this(color.R, color.G, color.B, color.A)
        {
        }

        public float R => _color.X;

        public float G => _color.Y;

        public float B => _color.Z;

        public float A => _color.W;

        public override int GetHashCode()
        {
            return (int)(A + R + G + B);//TODO?
        }

        public static implicit operator ColorByte(Color col)
        {
            return new ColorByte(col.R, col.G, col.B, col.A);
        }

        public static implicit operator Vector4(Color col)
        {
            return col.ToVector4();
        }

        public override bool Equals(object obj)
        {
            if (obj is Color)
            {
                Color sec = (Color)obj;
                return A == sec.A && R == sec.R && G == sec.G && B == sec.B;
            }
            return false;
        }

        public static bool operator ==(Color a, Color b)
        {
            return a.A == b.A && a.R == b.R && a.G == b.G && a.B == b.B;
        }

        public static bool operator !=(Color a, Color b)
        {
            return a.A != b.A || a.R != b.R || a.G != b.G || a.B != b.B;
        }

        public static Color operator *(Color value, float scale)
        {
            return new Color(value.R * scale, value.G * scale, value.B * scale, value.A * scale);
        }

        public static Color operator +(Color value1, Color value2)
        {
            return new Color(value1.R + value2.R, value1.G + value2.G, value1.B + value2.B, value1.A + value2.A);
        }

        public Vector4 ToVector4()
        {
            return _color;
        }

        #region Colors

        static Color()
        {
            AliceBlue = new Color(System.Drawing.Color.AliceBlue);
            AntiqueWhite = new Color(System.Drawing.Color.AntiqueWhite);
            Aqua = new Color(System.Drawing.Color.Aqua);
            Aquamarine = new Color(System.Drawing.Color.Aquamarine);
            Azure = new Color(System.Drawing.Color.Azure);
            Beige = new Color(System.Drawing.Color.Beige);
            Bisque = new Color(System.Drawing.Color.Bisque);
            Black = new Color(0.0f, 0.0f, 0.0f);
            BlanchedAlmond = new Color(System.Drawing.Color.BlanchedAlmond);
            Blue = new Color(0.0f, 0.0f, 1.0f);
            BlueViolet = new Color(System.Drawing.Color.BlueViolet);
            Brown = new Color(System.Drawing.Color.Brown);
            BurlyWood = new Color(System.Drawing.Color.BurlyWood);
            CadetBlue = new Color(System.Drawing.Color.CadetBlue);
            Chartreuse = new Color(System.Drawing.Color.Chartreuse);
            Chocolate = new Color(System.Drawing.Color.Chocolate);
            Coral = new Color(System.Drawing.Color.Coral);
            CornflowerBlue = new Color(System.Drawing.Color.CornflowerBlue);
            Cornsilk = new Color(System.Drawing.Color.Cornsilk);
            Crimson = new Color(System.Drawing.Color.Crimson);
            Cyan = new Color(System.Drawing.Color.Cyan);
            DarkBlue = new Color(System.Drawing.Color.DarkBlue);
            DarkCyan = new Color(System.Drawing.Color.DarkCyan);
            DarkGoldenrod = new Color(System.Drawing.Color.DarkGoldenrod);
            DarkGray = new Color(System.Drawing.Color.DarkGray);
            DarkGreen = new Color(System.Drawing.Color.DarkGreen);
            DarkKhaki = new Color(System.Drawing.Color.DarkKhaki);
            DarkMagenta = new Color(System.Drawing.Color.DarkMagenta);
            DarkOliveGreen = new Color(System.Drawing.Color.DarkOliveGreen);
            DarkOrange = new Color(System.Drawing.Color.DarkOrange);
            DarkOrchid = new Color(System.Drawing.Color.DarkOrchid);
            DarkRed = new Color(System.Drawing.Color.DarkRed);
            DarkSalmon = new Color(System.Drawing.Color.DarkSalmon);
            DarkSeaGreen = new Color(System.Drawing.Color.DarkSeaGreen);
            DarkSlateBlue = new Color(System.Drawing.Color.DarkSlateBlue);
            DarkSlateGray = new Color(System.Drawing.Color.DarkSlateGray);
            DarkTurquoise = new Color(System.Drawing.Color.DarkTurquoise);
            DarkViolet = new Color(System.Drawing.Color.DarkViolet);
            DeepPink = new Color(System.Drawing.Color.DeepPink);
            DeepSkyBlue = new Color(System.Drawing.Color.DeepSkyBlue);
            DimGray = new Color(System.Drawing.Color.DimGray);
            DodgerBlue = new Color(System.Drawing.Color.DodgerBlue);
            Firebrick = new Color(System.Drawing.Color.Firebrick);
            FloralWhite = new Color(System.Drawing.Color.FloralWhite);
            ForestGreen = new Color(System.Drawing.Color.ForestGreen);
            Fuchsia = new Color(System.Drawing.Color.Fuchsia);
            Gainsboro = new Color(System.Drawing.Color.Gainsboro);
            GhostWhite = new Color(System.Drawing.Color.GhostWhite);
            Gold = new Color(System.Drawing.Color.Gold);
            Goldenrod = new Color(System.Drawing.Color.Goldenrod);
            Gray = new Color(System.Drawing.Color.Gray);
            Green = new Color(0.0f, 1.0f, 0.0f);
            GreenYellow = new Color(System.Drawing.Color.GreenYellow);
            Honeydew = new Color(System.Drawing.Color.Honeydew);
            HotPink = new Color(System.Drawing.Color.HotPink);
            IndianRed = new Color(System.Drawing.Color.IndianRed);
            Indigo = new Color(System.Drawing.Color.Indigo);
            Ivory = new Color(System.Drawing.Color.Ivory);
            Khaki = new Color(System.Drawing.Color.Khaki);
            Lavender = new Color(System.Drawing.Color.Lavender);
            LavenderBlush = new Color(System.Drawing.Color.LavenderBlush);
            LawnGreen = new Color(System.Drawing.Color.LawnGreen);
            LemonChiffon = new Color(System.Drawing.Color.LemonChiffon);
            LightBlue = new Color(System.Drawing.Color.LightBlue);
            LightCoral = new Color(System.Drawing.Color.LightCoral);
            LightCyan = new Color(System.Drawing.Color.LightCyan);
            LightGoldenrodYellow = new Color(System.Drawing.Color.LightGoldenrodYellow);
            LightGray = new Color(System.Drawing.Color.LightGray);
            LightGreen = new Color(System.Drawing.Color.LightGreen);
            LightPink = new Color(System.Drawing.Color.LightPink);
            LightSalmon = new Color(System.Drawing.Color.LightSalmon);
            LightSeaGreen = new Color(System.Drawing.Color.LightSeaGreen);
            LightSkyBlue = new Color(System.Drawing.Color.LightSkyBlue);
            LightSlateGray = new Color(System.Drawing.Color.LightSlateGray);
            LightSteelBlue = new Color(System.Drawing.Color.LightSteelBlue);
            LightYellow = new Color(System.Drawing.Color.LightYellow);
            Lime = new Color(System.Drawing.Color.Lime);
            LimeGreen = new Color(System.Drawing.Color.LimeGreen);
            Linen = new Color(System.Drawing.Color.Linen);
            Magenta = new Color(System.Drawing.Color.Magenta);
            Maroon = new Color(System.Drawing.Color.Maroon);
            MediumAquamarine = new Color(System.Drawing.Color.MediumAquamarine);
            MediumBlue = new Color(System.Drawing.Color.MediumBlue);
            MediumOrchid = new Color(System.Drawing.Color.MediumOrchid);
            MediumPurple = new Color(System.Drawing.Color.MediumPurple);
            MediumSeaGreen = new Color(System.Drawing.Color.MediumSeaGreen);
            MediumSlateBlue = new Color(System.Drawing.Color.MediumSlateBlue);
            MediumSpringGreen = new Color(System.Drawing.Color.MediumSpringGreen);
            MediumTurquoise = new Color(System.Drawing.Color.MediumTurquoise);
            MediumVioletRed = new Color(System.Drawing.Color.MediumVioletRed);
            MidnightBlue = new Color(System.Drawing.Color.MidnightBlue);
            MintCream = new Color(System.Drawing.Color.MintCream);
            MistyRose = new Color(System.Drawing.Color.MistyRose);
            Moccasin = new Color(System.Drawing.Color.Moccasin);
            NavajoWhite = new Color(System.Drawing.Color.NavajoWhite);
            Navy = new Color(System.Drawing.Color.Navy);
            OldLace = new Color(System.Drawing.Color.OldLace);
            Olive = new Color(System.Drawing.Color.Olive);
            OliveDrab = new Color(System.Drawing.Color.OliveDrab);
            Orange = new Color(System.Drawing.Color.Orange);
            OrangeRed = new Color(System.Drawing.Color.OrangeRed);
            Orchid = new Color(System.Drawing.Color.Orchid);
            PaleGoldenrod = new Color(System.Drawing.Color.PaleGoldenrod);
            PaleGreen = new Color(System.Drawing.Color.PaleGreen);
            PaleTurquoise = new Color(System.Drawing.Color.PaleTurquoise);
            PaleVioletRed = new Color(System.Drawing.Color.PaleVioletRed);
            PapayaWhip = new Color(System.Drawing.Color.PapayaWhip);
            PeachPuff = new Color(System.Drawing.Color.PeachPuff);
            Peru = new Color(System.Drawing.Color.Peru);
            Pink = new Color(System.Drawing.Color.Pink);
            Plum = new Color(System.Drawing.Color.Plum);
            PowderBlue = new Color(System.Drawing.Color.PowderBlue);
            Purple = new Color(System.Drawing.Color.Purple);
            Red = new Color(1.0f, 0.0f, 0.0f);
            RosyBrown = new Color(System.Drawing.Color.RosyBrown);
            RoyalBlue = new Color(System.Drawing.Color.RoyalBlue);
            SaddleBrown = new Color(System.Drawing.Color.SaddleBrown);
            Salmon = new Color(System.Drawing.Color.Salmon);
            SandyBrown = new Color(System.Drawing.Color.SandyBrown);
            SeaGreen = new Color(System.Drawing.Color.SeaGreen);
            SeaShell = new Color(System.Drawing.Color.SeaShell);
            Sienna = new Color(System.Drawing.Color.Sienna);
            Silver = new Color(System.Drawing.Color.Silver);
            SkyBlue = new Color(System.Drawing.Color.SkyBlue);
            SlateBlue = new Color(System.Drawing.Color.AliceBlue);
            SlateGray = new Color(System.Drawing.Color.SlateGray);
            Snow = new Color(System.Drawing.Color.Snow);
            SpringGreen = new Color(System.Drawing.Color.SpringGreen);
            SteelBlue = new Color(System.Drawing.Color.SteelBlue);
            Tan = new Color(System.Drawing.Color.Tan);
            Teal = new Color(System.Drawing.Color.Teal);
            Thistle = new Color(System.Drawing.Color.Thistle);
            Tomato = new Color(System.Drawing.Color.Tomato);
            Transparent = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            Turquoise = new Color(System.Drawing.Color.Turquoise);
            Violet = new Color(System.Drawing.Color.Violet);
            Wheat = new Color(System.Drawing.Color.Wheat);
            White = new Color(1.0f, 1.0f, 1.0f);
            WhiteSmoke = new Color(System.Drawing.Color.WhiteSmoke);
            Yellow = new Color(System.Drawing.Color.Yellow);
            YellowGreen = new Color(System.Drawing.Color.YellowGreen);
        }

        public static Color AliceBlue
        {
            get;
            private set;
        }

        public static Color AntiqueWhite
        {
            get;
            private set;
        }

        public static Color Aqua
        {
            get;
            private set;
        }

        public static Color Aquamarine
        {
            get;
            private set;
        }

        public static Color Azure
        {
            get;
            private set;
        }

        public static Color Beige
        {
            get;
            private set;
        }

        public static Color Bisque
        {
            get;
            private set;
        }

        public static Color Black
        {
            get;
            private set;
        }

        public static Color BlanchedAlmond
        {
            get;
            private set;
        }

        public static Color Blue
        {
            get;
            private set;
        }

        public static Color BlueViolet
        {
            get;
            private set;
        }

        public static Color Brown
        {
            get;
            private set;
        }

        public static Color BurlyWood
        {
            get;
            private set;
        }

        public static Color CadetBlue
        {
            get;
            private set;
        }

        public static Color Chartreuse
        {
            get;
            private set;
        }

        public static Color Chocolate
        {
            get;
            private set;
        }

        public static Color Coral
        {
            get;
            private set;
        }

        public static Color CornflowerBlue
        {
            get;
            private set;
        }

        public static Color Cornsilk
        {
            get;
            private set;
        }

        public static Color Crimson
        {
            get;
            private set;
        }

        public static Color Cyan
        {
            get;
            private set;
        }

        public static Color DarkBlue
        {
            get;
            private set;
        }

        public static Color DarkCyan
        {
            get;
            private set;
        }

        public static Color DarkGoldenrod
        {
            get;
            private set;
        }

        public static Color DarkGray
        {
            get;
            private set;
        }

        public static Color DarkGreen
        {
            get;
            private set;
        }

        public static Color DarkKhaki
        {
            get;
            private set;
        }

        public static Color DarkMagenta
        {
            get;
            private set;
        }

        public static Color DarkOliveGreen
        {
            get;
            private set;
        }

        public static Color DarkOrange
        {
            get;
            private set;
        }

        public static Color DarkOrchid
        {
            get;
            private set;
        }

        public static Color DarkRed
        {
            get;
            private set;
        }

        public static Color DarkSalmon
        {
            get;
            private set;
        }

        public static Color DarkSeaGreen
        {
            get;
            private set;
        }

        public static Color DarkSlateBlue
        {
            get;
            private set;
        }

        public static Color DarkSlateGray
        {
            get;
            private set;
        }

        public static Color DarkTurquoise
        {
            get;
            private set;
        }

        public static Color DarkViolet
        {
            get;
            private set;
        }

        public static Color DeepPink
        {
            get;
            private set;
        }

        public static Color DeepSkyBlue
        {
            get;
            private set;
        }

        public static Color DimGray
        {
            get;
            private set;
        }

        public static Color DodgerBlue
        {
            get;
            private set;
        }

        public static Color Firebrick
        {
            get;
            private set;
        }

        public static Color FloralWhite
        {
            get;
            private set;
        }

        public static Color ForestGreen
        {
            get;
            private set;
        }

        public static Color Fuchsia
        {
            get;
            private set;
        }

        public static Color Gainsboro
        {
            get;
            private set;
        }

        public static Color GhostWhite
        {
            get;
            private set;
        }

        public static Color Gold
        {
            get;
            private set;
        }

        public static Color Goldenrod
        {
            get;
            private set;
        }

        public static Color Gray
        {
            get;
            private set;
        }

        public static Color Green
        {
            get;
            private set;
        }

        public static Color GreenYellow
        {
            get;
            private set;
        }

        public static Color Honeydew
        {
            get;
            private set;
        }

        public static Color HotPink
        {
            get;
            private set;
        }

        public static Color IndianRed
        {
            get;
            private set;
        }

        public static Color Indigo
        {
            get;
            private set;
        }

        public static Color Ivory
        {
            get;
            private set;
        }

        public static Color Khaki
        {
            get;
            private set;
        }

        public static Color Lavender
        {
            get;
            private set;
        }

        public static Color LavenderBlush
        {
            get;
            private set;
        }

        public static Color LawnGreen
        {
            get;
            private set;
        }

        public static Color LemonChiffon
        {
            get;
            private set;
        }

        public static Color LightBlue
        {
            get;
            private set;
        }

        public static Color LightCoral
        {
            get;
            private set;
        }

        public static Color LightCyan
        {
            get;
            private set;
        }

        public static Color LightGoldenrodYellow
        {
            get;
            private set;
        }

        public static Color LightGray
        {
            get;
            private set;
        }

        public static Color LightGreen
        {
            get;
            private set;
        }

        public static Color LightPink
        {
            get;
            private set;
        }

        public static Color LightSalmon
        {
            get;
            private set;
        }

        public static Color LightSeaGreen
        {
            get;
            private set;
        }

        public static Color LightSkyBlue
        {
            get;
            private set;
        }

        public static Color LightSlateGray
        {
            get;
            private set;
        }

        public static Color LightSteelBlue
        {
            get;
            private set;
        }

        public static Color LightYellow
        {
            get;
            private set;
        }

        public static Color Lime
        {
            get;
            private set;
        }

        public static Color LimeGreen
        {
            get;
            private set;
        }

        public static Color Linen
        {
            get;
            private set;
        }

        public static Color Magenta
        {
            get;
            private set;
        }

        public static Color Maroon
        {
            get;
            private set;
        }

        public static Color MediumAquamarine
        {
            get;
            private set;
        }

        public static Color MediumBlue
        {
            get;
            private set;
        }

        public static Color MediumOrchid
        {
            get;
            private set;
        }

        public static Color MediumPurple
        {
            get;
            private set;
        }

        public static Color MediumSeaGreen
        {
            get;
            private set;
        }

        public static Color MediumSlateBlue
        {
            get;
            private set;
        }

        public static Color MediumSpringGreen
        {
            get;
            private set;
        }

        public static Color MediumTurquoise
        {
            get;
            private set;
        }

        public static Color MediumVioletRed
        {
            get;
            private set;
        }

        public static Color MidnightBlue
        {
            get;
            private set;
        }

        public static Color MintCream
        {
            get;
            private set;
        }

        public static Color MistyRose
        {
            get;
            private set;
        }

        public static Color Moccasin
        {
            get;
            private set;
        }

        public static Color NavajoWhite
        {
            get;
            private set;
        }

        public static Color Navy
        {
            get;
            private set;
        }

        public static Color OldLace
        {
            get;
            private set;
        }

        public static Color Olive
        {
            get;
            private set;
        }

        public static Color OliveDrab
        {
            get;
            private set;
        }

        public static Color Orange
        {
            get;
            private set;
        }

        public static Color OrangeRed
        {
            get;
            private set;
        }

        public static Color Orchid
        {
            get;
            private set;
        }

        public static Color PaleGoldenrod
        {
            get;
            private set;
        }

        public static Color PaleGreen
        {
            get;
            private set;
        }

        public static Color PaleTurquoise
        {
            get;
            private set;
        }

        public static Color PaleVioletRed
        {
            get;
            private set;
        }

        public static Color PapayaWhip
        {
            get;
            private set;
        }

        public static Color PeachPuff
        {
            get;
            private set;
        }

        public static Color Peru
        {
            get;
            private set;
        }

        public static Color Pink
        {
            get;
            private set;
        }

        public static Color Plum
        {
            get;
            private set;
        }

        public static Color PowderBlue
        {
            get;
            private set;
        }

        public static Color Purple
        {
            get;
            private set;
        }

        public static Color Red
        {
            get;
            private set;
        }

        public static Color RosyBrown
        {
            get;
            private set;
        }

        public static Color RoyalBlue
        {
            get;
            private set;
        }

        public static Color SaddleBrown
        {
            get;
            private set;
        }

        public static Color Salmon
        {
            get;
            private set;
        }

        public static Color SandyBrown
        {
            get;
            private set;
        }

        public static Color SeaGreen
        {
            get;
            private set;
        }

        public static Color SeaShell
        {
            get;
            private set;
        }

        public static Color Sienna
        {
            get;
            private set;
        }

        public static Color Silver
        {
            get;
            private set;
        }

        public static Color SkyBlue
        {
            get;
            private set;
        }

        public static Color SlateBlue
        {
            get;
            private set;
        }

        public static Color SlateGray
        {
            get;
            private set;
        }

        public static Color Snow
        {
            get;
            private set;
        }

        public static Color SpringGreen
        {
            get;
            private set;
        }

        public static Color SteelBlue
        {
            get;
            private set;
        }

        public static Color Tan
        {
            get;
            private set;
        }

        public static Color Teal
        {
            get;
            private set;
        }

        public static Color Thistle
        {
            get;
            private set;
        }

        public static Color Tomato
        {
            get;
            private set;
        }

        public static Color Transparent
        {
            get;
            private set;
        }

        public static Color Turquoise
        {
            get;
            private set;
        }

        public static Color Violet
        {
            get;
            private set;
        }

        public static Color Wheat
        {
            get;
            private set;
        }

        public static Color White
        {
            get;
            private set;
        }

        public static Color WhiteSmoke
        {
            get;
            private set;
        }

        public static Color Yellow
        {
            get;
            private set;
        }

        public static Color YellowGreen
        {
            get;
            private set;
        }

        #endregion
    }
}
