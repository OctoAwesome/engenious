using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace engenious
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [TypeConverter(typeof(Vector2Converter))]
    public struct Vector2 : IEquatable<Vector2>
    {
        public float X{get;set;}
        public float Y{get;set;}

        public Vector2(float w)
        {
            X = Y = w;
        }

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float Dot(Vector2 value2)
        {
            return X * value2.X + Y * value2.Y;
        }

        public Vector2 Cross()
        {
            return new Vector2(X, -Y);
        }
        [Browsable(false)]
        public float Length => (float)Math.Sqrt(X * X + Y * Y);

        [Browsable(false)]
        public float LengthSquared => X * X + Y * Y;

        public void Normalize()
        {
            float len = Length;
            X /= len;
            Y /= len;
        }

        public Vector2 Normalized()
        {
            return this / Length;
        }

        public Vector2 Transform(Matrix matrix)
        {
            return new Vector2((X * matrix.M11) + (Y * matrix.M21) + matrix.M41, (X * matrix.M12) + (Y * matrix.M22) + matrix.M42);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        #region IEquatable implementation

        public override bool Equals(object obj)
        {
            if (obj is Vector2)
                return Equals((Vector2)obj);
            return false;
        }

        public bool Equals(Vector2 other)
        {
            return X == other.X && Y == other.Y;
        }

        #endregion

        public static bool operator ==(Vector2 value1, Vector2 value2)
        {
            return value1.X == value2.X && value1.Y == value2.Y;
        }

        public static bool operator !=(Vector2 value1, Vector2 value2)
        {
            return value1.X != value2.X || value1.Y != value2.Y;
        }

        public static Vector2 operator +(Vector2 value1, Vector2 value2)
        {
            value1.X += value2.X;
            value1.Y += value2.Y;
            return value1;
        }

        public static Vector2 operator -(Vector2 value1, Vector2 value2)
        {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            return value1;
        }

        public static Vector2 operator -(Vector2 value)
        {
            value.X = -value.X;
            value.Y = -value.Y;
            return value;
        }

        public static Vector2 operator *(Vector2 value, float scalar)
        {
            value.X *= scalar;
            value.Y *= scalar;
            return value;
        }

        public static Vector2 operator *(float scalar, Vector2 value)
        {
            return value * scalar;
        }

        public static Vector2 operator *(Vector2 value1, Vector2 value2)//TODO: ugly as hell
        {
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            return value1;
        }

        public static Vector2 operator /(Vector2 value, float scalar)
        {
            value.X /= scalar;
            value.Y /= scalar;
            return value;
        }

        public static Vector2 operator /(Vector2 value1, Vector2 value2)//TODO: ugly as hell?
        {
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            return value1;
        }

            

        public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max)
        {
            value.X = Math.Min(Math.Max(min.X, value.X), max.X);
            value.Y = Math.Min(Math.Max(min.Y, value.Y), max.Y);
            return value;
        }

        public static void Clamp(Vector2 value, Vector2 min, Vector2 max, out Vector2 output)
        {
            output =new Vector2( Math.Min(Math.Max(min.X, value.X), max.X),
                                Math.Min(Math.Max(min.Y, value.Y), max.Y));
        }

        public static float Dot(Vector2 value1, Vector2 value2)
        {
            return value1.Dot(value2);
        }

        public static float Distance(Vector2 value1, Vector2 value2)
        {
            return (value1 - value2).Length;
        }

        public static float DistanceSquared(Vector2 value1, Vector2 value2)
        {
            return (value1 - value2).LengthSquared;
        }

        public static Vector2 Lerp(Vector2 value1, Vector2 value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        public static Vector2 Max(Vector2 value1, Vector2 value2)
        {
            return new Vector2(Math.Max(value1.X, value2.X), Math.Max(value1.Y, value2.Y));
        }

        public static Vector2 Min(Vector2 value1, Vector2 value2)
        {
            return new Vector2(Math.Min(value1.X, value2.X), Math.Min(value1.Y, value2.Y));
        }

        public static Vector2 Reflect(Vector2 vector, Vector2 normal)
        {
            normal.Normalize();
            return 2 * (normal.Dot(vector) * normal - vector); //TODO: normalize normal?
        }

        public static Vector2 Transform(Vector2 position, Matrix matrix)
        {
            //{v1*x1+v2*y1+w1,v1*x2+v2*y2+w2,v1*x3+v2*y3+w3,v1*x4+v2*y4+w4}
            //{v1*x1+v2*x2+x4,v1*y1+v2*y2+y4,v1*z1+v2*z2+z4,v1*w1+v2*w2+w4}
            return new Vector2(position.X * matrix.M11 + position.Y * matrix.M12 + matrix.M14, position.X * matrix.M21 + position.Y * matrix.M22 + matrix.M24);
        }

        public static Vector2 TransformNormal(Vector2 normal, Matrix matrix)
        {
            throw new NotImplementedException();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Transform(int count, Vector2* positions, ref Matrix matrix, Vector2* output)
        {
            for (int i = 0; i < count; i++, positions++, output++)
            {
                *output = new Vector2(
                    (float)((*positions).X * (double)matrix.M11 + (*positions).Y * (double)matrix.M12) + matrix.M14,
                    (float)((*positions).X * (double)matrix.M21 + (*positions).Y * (double)matrix.M22) + matrix.M24);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform(Vector2[] positions, ref Matrix matrix, Vector2[] output)
        {
            int index = 0;
            foreach (var position in positions)
            {
                output[index++] = new Vector2(position.X * matrix.M11 + position.Y * matrix.M12 + matrix.M14, position.X * matrix.M21 + position.Y * matrix.M22 + matrix.M24);
            }
        }

        public static readonly Vector2 UnitX = new Vector2(1, 0);
        public static readonly Vector2 UnitY = new Vector2(0, 1);
        public static readonly Vector2 One = new Vector2(1, 1);
        public static readonly Vector2 Zero;
    }
}

