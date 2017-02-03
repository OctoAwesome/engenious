namespace engenious
{
    public struct Size
    {
        public Size(Point point)
        {
            Width = point.X;
            Height = point.Y;
        }

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Width{ get; set; }

        public int Height{ get; set; }

        public override string ToString()
        {
            return $"[Size: Width={Width}, Height={Height}]";
        }

        public override int GetHashCode()
        {
            return Width ^ Height;//TODO
        }

        public override bool Equals(object obj)
        {
            if (obj is Size)
            {
                Size sec = (Size)obj;
                return Width == sec.Width && Height == sec.Height;
            }
            return false;
        }

        public static bool operator ==(Size a, Size b)
        {
            return a.Width == b.Width && a.Height == b.Height;
        }

        public static bool operator !=(Size a, Size b)
        {
            return a.Width != b.Width || a.Height != b.Height;
        }

        public static Size operator +(Size value1, Size value2)
        {
            return new Size(value1.Width + value2.Width, value1.Height + value2.Height);
        }

        public static Size operator /(Size divident, Size divisor)
        {
            return new Size(divident.Width / divisor.Width, divident.Height / divisor.Height);
        }


        public static Size operator *(Size value1, Size value2)
        {
            return new Size(value1.Width * value2.Width, value1.Height * value2.Height);
        }

        public static Size operator -(Size value1, Size value2)
        {
            return new Size(value1.Width - value2.Width, value1.Height - value2.Height);
        }

        public static implicit operator Size(System.Drawing.Size col)
        {
            return new Size(col.Width, col.Width);
        }
    }
}

