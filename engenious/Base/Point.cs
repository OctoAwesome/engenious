using System.ComponentModel;

namespace engenious
{
    public struct Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X{ get; set; }

        public int Y{ get; set; }

        [Browsable(false)]
        public bool IsEmpty => X == 0 && Y == 0;

        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }

        public static readonly Point Zero = new Point();

        public override string ToString()
        {
            return $"[Point: X={X}, Y={Y}]";
        }

        public override int GetHashCode()
        {
            return X ^ Y;//TODO
        }

        public override bool Equals(object obj)
        {
            if (obj is Point)
            {
                Point sec = (Point)obj;
                return X == sec.X && Y == sec.Y;
            }
            return false;
        }

        public static bool operator ==(Point a, Point b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Point a, Point b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public static Point operator +(Point value1, Point value2)
        {
            return new Point(value1.X + value2.X, value1.Y + value2.Y);
        }

        public static Point operator /(Point divident, Point divisor)
        {
            return new Point(divident.X / divisor.X, divident.Y / divisor.Y);
        }

        public static Point operator -(Point value1, Point value2)
        {
            return new Point(value1.X - value2.X, value1.Y - value2.Y);
        }

        public static implicit operator Point(System.Drawing.Point col)
        {
            return new Point(col.X, col.Y);
        }
    }

}