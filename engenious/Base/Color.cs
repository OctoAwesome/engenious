// ReSharper disable CompareOfFloatsByEqualityOperator
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
            if (!(obj is Color)) return false;
            var sec = (Color)obj;
            return A == sec.A && R == sec.R && G == sec.G && B == sec.B;
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
        }

        public static Color AntiqueWhite
        {
            get;
        }

        public static Color Aqua
        {
            get;
        }

        public static Color Aquamarine
        {
            get;
        }

        public static Color Azure
        {
            get;
        }

        public static Color Beige
        {
            get;
        }

        public static Color Bisque
        {
            get;
        }

        public static Color Black
        {
            get;
        }

        public static Color BlanchedAlmond
        {
            get;
        }

        public static Color Blue
        {
            get;
        }

        public static Color BlueViolet
        {
            get;
        }

        public static Color Brown
        {
            get;
        }

        public static Color BurlyWood
        {
            get;
        }

        public static Color CadetBlue
        {
            get;
        }

        public static Color Chartreuse
        {
            get;
        }

        public static Color Chocolate
        {
            get;
        }

        public static Color Coral
        {
            get;
        }

        public static Color CornflowerBlue
        {
            get;
        }

        public static Color Cornsilk
        {
            get;
        }

        public static Color Crimson
        {
            get;
        }

        public static Color Cyan
        {
            get;
        }

        public static Color DarkBlue
        {
            get;
        }

        public static Color DarkCyan
        {
            get;
        }

        public static Color DarkGoldenrod
        {
            get;
        }

        public static Color DarkGray
        {
            get;
        }

        public static Color DarkGreen
        {
            get;
        }

        public static Color DarkKhaki
        {
            get;
        }

        public static Color DarkMagenta
        {
            get;
        }

        public static Color DarkOliveGreen
        {
            get;
        }

        public static Color DarkOrange
        {
            get;
        }

        public static Color DarkOrchid
        {
            get;
        }

        public static Color DarkRed
        {
            get;
        }

        public static Color DarkSalmon
        {
            get;
        }

        public static Color DarkSeaGreen
        {
            get;
        }

        public static Color DarkSlateBlue
        {
            get;
        }

        public static Color DarkSlateGray
        {
            get;
        }

        public static Color DarkTurquoise
        {
            get;
        }

        public static Color DarkViolet
        {
            get;
        }

        public static Color DeepPink
        {
            get;
        }

        public static Color DeepSkyBlue
        {
            get;
        }

        public static Color DimGray
        {
            get;
        }

        public static Color DodgerBlue
        {
            get;
        }

        public static Color Firebrick
        {
            get;
        }

        public static Color FloralWhite
        {
            get;
        }

        public static Color ForestGreen
        {
            get;
        }

        public static Color Fuchsia
        {
            get;
        }

        public static Color Gainsboro
        {
            get;
        }

        public static Color GhostWhite
        {
            get;
        }

        public static Color Gold
        {
            get;
        }

        public static Color Goldenrod
        {
            get;
        }

        public static Color Gray
        {
            get;
        }

        public static Color Green
        {
            get;
        }

        public static Color GreenYellow
        {
            get;
        }

        public static Color Honeydew
        {
            get;
        }

        public static Color HotPink
        {
            get;
        }

        public static Color IndianRed
        {
            get;
        }

        public static Color Indigo
        {
            get;
        }

        public static Color Ivory
        {
            get;
        }

        public static Color Khaki
        {
            get;
        }

        public static Color Lavender
        {
            get;
        }

        public static Color LavenderBlush
        {
            get;
        }

        public static Color LawnGreen
        {
            get;
        }

        public static Color LemonChiffon
        {
            get;
        }

        public static Color LightBlue
        {
            get;
        }

        public static Color LightCoral
        {
            get;
        }

        public static Color LightCyan
        {
            get;
        }

        public static Color LightGoldenrodYellow
        {
            get;
        }

        public static Color LightGray
        {
            get;
        }

        public static Color LightGreen
        {
            get;
        }

        public static Color LightPink
        {
            get;
        }

        public static Color LightSalmon
        {
            get;
        }

        public static Color LightSeaGreen
        {
            get;
        }

        public static Color LightSkyBlue
        {
            get;
        }

        public static Color LightSlateGray
        {
            get;
        }

        public static Color LightSteelBlue
        {
            get;
        }

        public static Color LightYellow
        {
            get;
        }

        public static Color Lime
        {
            get;
        }

        public static Color LimeGreen
        {
            get;
        }

        public static Color Linen
        {
            get;
        }

        public static Color Magenta
        {
            get;
        }

        public static Color Maroon
        {
            get;
        }

        public static Color MediumAquamarine
        {
            get;
        }

        public static Color MediumBlue
        {
            get;
        }

        public static Color MediumOrchid
        {
            get;
        }

        public static Color MediumPurple
        {
            get;
        }

        public static Color MediumSeaGreen
        {
            get;
        }

        public static Color MediumSlateBlue
        {
            get;
        }

        public static Color MediumSpringGreen
        {
            get;
        }

        public static Color MediumTurquoise
        {
            get;
        }

        public static Color MediumVioletRed
        {
            get;
        }

        public static Color MidnightBlue
        {
            get;
        }

        public static Color MintCream
        {
            get;
        }

        public static Color MistyRose
        {
            get;
        }

        public static Color Moccasin
        {
            get;
        }

        public static Color NavajoWhite
        {
            get;
        }

        public static Color Navy
        {
            get;
        }

        public static Color OldLace
        {
            get;
        }

        public static Color Olive
        {
            get;
        }

        public static Color OliveDrab
        {
            get;
        }

        public static Color Orange
        {
            get;
        }

        public static Color OrangeRed
        {
            get;
        }

        public static Color Orchid
        {
            get;
        }

        public static Color PaleGoldenrod
        {
            get;
        }

        public static Color PaleGreen
        {
            get;
        }

        public static Color PaleTurquoise
        {
            get;
        }

        public static Color PaleVioletRed
        {
            get;
        }

        public static Color PapayaWhip
        {
            get;
        }

        public static Color PeachPuff
        {
            get;
        }

        public static Color Peru
        {
            get;
        }

        public static Color Pink
        {
            get;
        }

        public static Color Plum
        {
            get;
        }

        public static Color PowderBlue
        {
            get;
        }

        public static Color Purple
        {
            get;
        }

        public static Color Red
        {
            get;
        }

        public static Color RosyBrown
        {
            get;
        }

        public static Color RoyalBlue
        {
            get;
        }

        public static Color SaddleBrown
        {
            get;
        }

        public static Color Salmon
        {
            get;
        }

        public static Color SandyBrown
        {
            get;
        }

        public static Color SeaGreen
        {
            get;
        }

        public static Color SeaShell
        {
            get;
        }

        public static Color Sienna
        {
            get;
        }

        public static Color Silver
        {
            get;
        }

        public static Color SkyBlue
        {
            get;
        }

        public static Color SlateBlue
        {
            get;
        }

        public static Color SlateGray
        {
            get;
        }

        public static Color Snow
        {
            get;
        }

        public static Color SpringGreen
        {
            get;
        }

        public static Color SteelBlue
        {
            get;
        }

        public static Color Tan
        {
            get;
        }

        public static Color Teal
        {
            get;
        }

        public static Color Thistle
        {
            get;
        }

        public static Color Tomato
        {
            get;
        }

        public static Color Transparent
        {
            get;
        }

        public static Color Turquoise
        {
            get;
        }

        public static Color Violet
        {
            get;
        }

        public static Color Wheat
        {
            get;
        }

        public static Color White
        {
            get;
        }

        public static Color WhiteSmoke
        {
            get;
        }

        public static Color Yellow
        {
            get;
        }

        public static Color YellowGreen
        {
            get;
        }

        #endregion
    }
}
