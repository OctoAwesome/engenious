using System.Runtime.InteropServices;

namespace engenious
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ColorByte
    {

        public ColorByte(byte r, byte g, byte b, byte a = 255)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public ColorByte(float r, float g, float b, float a = 1.0f)
            : this((byte)(r * 255), (byte)(g * 255), (byte)(b * 255), (byte)(a * 255))
        {
            
        }

        public ColorByte(Color color, float a)
            : this(color.R, color.G, color.B, a)
        {
        }

        public ColorByte(System.Drawing.Color color)
            : this(color.R, color.G, color.B, color.A)
        {
        }

        public byte R { get; }

        public byte G { get; }

        public byte B { get; }

        public byte A { get; }

        public override int GetHashCode()
        {
            return A << 24 | R << 16 | G << 8 | B;//TODO?
        }

        public static implicit operator Color(ColorByte col)
        {
            return new Color(col.R, col.G, col.B, col.A);
        }
    }
}

