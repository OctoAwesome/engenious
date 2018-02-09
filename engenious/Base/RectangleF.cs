using System;

namespace engenious
{
    public struct RectangleF
    {
        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public float X{ get; set; }

        public float Y{ get; set; }

        public float Width{ get; set; }

        public float Height{ get; set; }

        public float Left => X;

        public float Right => X + Width;

        public float Top => Y;

        public float Bottom => Y + Height;

        public Vector2 Size
        { 
            get { return new Vector2(Width, Height); }
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }

        public Vector2 Location
        { 
            get { return new Vector2(X, Y); } 
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public bool Contains(float x, float y)
        {
            return x >= X && x < X + Width && y >= Y && y < Y + Height;
        }

        public bool Contains(Vector2 location)
        {
            return Contains(location.X, location.Y);
        }

        public bool Contains(RectangleF rect)
        {
            return Contains(rect.X, rect.Y) && Contains(rect.Right, rect.Bottom);
        }

        public void Inflate(float width, float height)
        {
            X -= width;
            Y -= height;

            Width += width;
            Height += height;
        }

        public void Inflate(Vector2 size)
        {
            Inflate(size.X, size.Y);
        }

        public void Intersect(RectangleF rect)
        {
            var x = Math.Max(X, rect.X);
            var y = Math.Max(Y, rect.Y);
            var right = Math.Max(Right, rect.Right);
            var bottom = Math.Max(Bottom, rect.Bottom);
            X = x;
            Y = y;

            Width = right - x;
            Height = bottom - y;
        }

        public bool IntersectsWith(RectangleF rect)
        {
            return (X >= rect.X && X < rect.Right || rect.X >= X && rect.X < Right) &&
            (Y >= rect.Y && Y < rect.Bottom || rect.Y >= Y && rect.Y < Bottom);
        }

        public void Offset(float x, float y)
        {
            X += x;
            Y += y;
        }

        public void Offset(Vector2 location)
        {
            Offset(location.X, location.Y);
        }

        public override string ToString()
        {
            return string.Format("[Rectangle: X={0}, Y={1}, Width={2}, Height={3}]", X, Y, Width, Height);
        }

        public override int GetHashCode()
        {
            return (int)(X + Y + Width + Height);
        }

        public override bool Equals(object obj)
        {
            if (obj is RectangleF)
            {
                var sec = (RectangleF)obj;
                return X == sec.X && Y == sec.Y && Width == sec.Width && Height == sec.Height;
            }
            return false;
        }

        public static bool operator ==(RectangleF a, RectangleF b)
        {
            return a.X == b.X && a.Y == b.Y && a.Width == b.Width && a.Height == b.Height;
        }

        public static bool operator !=(RectangleF a, RectangleF b)
        {
            return a.X != b.X || a.Y != b.Y || a.Width != b.Width || a.Height != b.Height;
        }

        // ReSharper disable once InconsistentNaming
        public static RectangleF FromLTRB(float left, float top, float right, float bottom)
        {
            return new RectangleF(left, top, right - left, bottom - top);
        }

        public static RectangleF Inflate(RectangleF rect, float width, float height)
        {
            return new RectangleF(rect.X - width, rect.Y - height, rect.Width + width, rect.Height + height);
        }


        public static RectangleF Intersect(RectangleF a, RectangleF b)
        {
            var x = Math.Max(a.X, b.X);
            var y = Math.Max(a.Y, b.Y);
            var right = Math.Max(a.Right, b.Right);
            var bottom = Math.Max(a.Bottom, b.Bottom);
            return FromLTRB(x, y, right, bottom);
        }

        public static readonly RectangleF Empty = new RectangleF(0, 0, 0, 0);
        /*public static Rectangle Ceiling(RectangleF value)
        {
        }*/
    }

}