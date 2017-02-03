using System;
using System.Runtime.InteropServices;

namespace engenious
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [System.ComponentModel.TypeConverter(typeof(Vector3Converter))]
    public struct Vector3:IEquatable<Vector3>
    {
        public float X{get;set;}
        public float Y{get;set;}
        public float Z{get;set;}

        public Vector3(float w)
        {
            X = w;
            Y = w;
            Z = w;
        }
        public Vector3(float x, float y, float z=0.0f)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float Dot(Vector3 value2)
        {
            return X * value2.X + Y * value2.Y + Z * value2.Z;
        }

        public Vector3 Cross(Vector3 value2)
        {
            return new Vector3(Y * value2.Z - Z * value2.Y,
                Z * value2.X - X * value2.Z,
                X * value2.Y - Y * value2.X);
        }
        [System.ComponentModel.Browsable(false)]
        public float Length => (float)Math.Sqrt(X * X + Y * Y + Z * Z);

        [System.ComponentModel.Browsable(false)]
        public float LengthSquared => X * X + Y * Y + Z * Z;

        public void Normalize()
        {
            float len = Length;
            X /= len;
            Y /= len;
            Z /= len;
        }

        public Vector3 Normalized()
        {
            return this / Length;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        public override string ToString()
        {
            return
                $"[{X.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)}, {Y.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)}, {Z.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)}]";
        }

        #region IEquatable implementation

        public override bool Equals(object obj)
        {
            if (obj is Vector3)
                return Equals((Vector3)obj);
            return false;
        }

        public bool Equals(Vector3 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        #endregion

        public static bool operator ==(Vector3 value1, Vector3 value2)
        {
            return value1.X == value2.X && value1.Y == value2.Y && value1.Z == value2.Z;
        }

        public static bool operator !=(Vector3 value1, Vector3 value2)
        {
            return value1.X != value2.X || value1.Y != value2.Y || value1.Z != value2.Z;
        }

        public static Vector3 operator +(Vector3 value1, Vector3 value2)
        {
            value1.X += value2.X;
            value1.Y += value2.Y;
            value1.Z += value2.Z;
            return value1;
        }

        public static Vector3 operator -(Vector3 value1, Vector3 value2)
        {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            value1.Z -= value2.Z;
            return value1;
        }

        public static Vector3 operator -(Vector3 value)
        {
            value.X = -value.X;
            value.Y = -value.Y;
            value.Z = -value.Z;
            return value;
        }


        public static Vector3 operator *(Vector3 value, float scalar)
        {
            value.X *= scalar;
            value.Y *= scalar;
            value.Z *= scalar;
            return value;
        }

        public static Vector3 operator *(float scalar, Vector3 value)
        {
            return value * scalar;
        }

        public static Vector3 operator *(Vector3 value1, Vector3 value2)//TODO: ugly as hell
        {
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            value1.Z *= value2.Z;
            return value1;
        }

        public static Vector3 operator /(Vector3 value, float scalar)
        {
            value.X /= scalar;
            value.Y /= scalar;
            value.Z /= scalar;
            return value;
        }

        public static Vector3 operator /(Vector3 value1, Vector3 value2)//TODO: ugly as hell?
        {
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            value1.Z /= value2.Z;
            return value1;
        }


        public static Vector3 Cross(Vector3 value1, Vector3 value2)
        {
            return value1.Cross(value2);
        }

        public static Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max)
        {
            value.X = Math.Min(Math.Max(min.X, value.X), max.X);
            value.Y = Math.Min(Math.Max(min.Y, value.Y), max.Y);
            value.Z = Math.Min(Math.Max(min.Z, value.Z), max.Y);
            return value;
        }

        public static void Clamp(Vector3 value, Vector3 min, Vector3 max, out Vector3 output)
        {
            output = new Vector3(Math.Min(Math.Max(min.X, value.X), max.X),
                                    Math.Min(Math.Max(min.Y, value.Y), max.Y),
                                    Math.Min(Math.Max(min.Z, value.Z), max.Z));
        }

        public static float Dot(Vector3 value1, Vector3 value2)
        {
            return value1.Dot(value2);
        }

        public static float Distance(Vector3 value1, Vector3 value2)
        {
            return (value1 - value2).Length;
        }

        public static float DistanceSquared(Vector3 value1, Vector3 value2)
        {
            return (value1 - value2).LengthSquared;
        }

        public static Vector3 Lerp(Vector3 value1, Vector3 value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        public static Vector3 Max(Vector3 value1, Vector3 value2)
        {
            return new Vector3(Math.Max(value1.X, value2.X), Math.Max(value1.Y, value2.Y), Math.Max(value1.Z, value2.Z));
        }

        public static Vector3 Min(Vector3 value1, Vector3 value2)
        {
            return new Vector3(Math.Min(value1.X, value2.X), Math.Min(value1.Y, value2.Y), Math.Min(value1.Z, value2.Z));
        }

        public static Vector3 Reflect(Vector3 vector, Vector3 normal)
        {
            normal.Normalize();
            return 2 * (normal.Dot(vector) * normal - vector); //TODO: normalize normal?
        }

        public static Vector3 Transform(Vector3 position, Matrix matrix)
        {
            return new Vector3(position.X * matrix.M11 + position.Y * matrix.M21 + position.Z * matrix.M31 + matrix.M41,
                position.X * matrix.M12 + position.Y * matrix.M22 + position.Z * matrix.M32 + matrix.M42,
                position.X * matrix.M13 + position.Y * matrix.M23 + position.Z * matrix.M33 + matrix.M43);
        }

        public static readonly Vector3 One = new Vector3(1, 1, 1);
        public static readonly Vector3 Zero;

        public static readonly Vector3 UnitX = new Vector3(1, 0);
        public static readonly Vector3 UnitY = new Vector3(0, 1);
        public static readonly Vector3 UnitZ = new Vector3(0, 0, 1);
    }
}

