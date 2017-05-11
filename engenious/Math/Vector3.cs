using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Fast = System.Numerics;
namespace engenious
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [System.ComponentModel.TypeConverter(typeof(Vector3Converter))]
    public struct Vector3:IEquatable<Vector3>
    {
        public float X
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]set;
        }
        public float Y
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]set;
        }
        public float Z
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]set;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3(float w)
        {
            X = w;
            Y = w;
            Z = w;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3(float x, float y, float z=0.0f)
        {
            X = x;
            Y = y;
            Z = z;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Dot(Vector3 value2)
        {
            return Vector3.Dot(this,value2);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 Cross(Vector3 value2)
        {
            return Vector3.Cross(this,value2);
        }
        [System.ComponentModel.Browsable(false)]
        public float Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get{return (float)Math.Sqrt(LengthSquared);}
        }

        [System.ComponentModel.Browsable(false)]
        public float LengthSquared
        {
            get{return Vector3.Dot(this,this);}
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Normalize()
        {
            this /= Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 Normalized()
        {
            return this / Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (obj is Vector3)
                return Equals((Vector3)obj);
            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector3 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        #endregion
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector3 value1, Vector3 value2)
        {
            return value1.X == value2.X && value1.Y == value2.Y && value1.Z == value2.Z;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector3 value1, Vector3 value2)
        {
            return value1.X != value2.X || value1.Y != value2.Y || value1.Z != value2.Z;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static Vector3 operator +(Vector3 value1, Vector3 value2)
        {
#if USE_SIMD
            Fast.Vector3 res= (*(Fast.Vector3*)&value1 + *(Fast.Vector3*)&value2);
            return *(Vector3*)&res;
#else
            value1.X += value2.X;
            value1.Y += value2.Y;
            value1.Z += value2.Z;
            return value1;
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static Vector3 operator -(Vector3 value1, Vector3 value2)
        {
#if USE_SIMD
            Fast.Vector3 res= (*(Fast.Vector3*)&value1 - *(Fast.Vector3*)&value2);
            return *(Vector3*)&res;
#else
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            value1.Z -= value2.Z;
            return value1;
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static Vector3 operator -(Vector3 value)
        {
#if USE_SIMD
            Fast.Vector3 res= (-*(Fast.Vector3*)&value);
            return *(Vector3*)&res;
#else
            value.X = -value.X;
            value.Y = -value.Y;
            value.Z = -value.Z;
            return value;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static Vector3 operator *(Vector3 value, float scalar)
        {
#if USE_SIMD
            Fast.Vector3 res= (scalar * *(Fast.Vector3*)&value);
            return *(Vector3*)&res;
#else
            value.X *= scalar;
            value.Y *= scalar;
            value.Z *= scalar;
            return value;
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(float scalar, Vector3 value)
        {
            return value * scalar;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static Vector3 operator *(Vector3 value1, Vector3 value2)//TODO: ugly as hell
        {
#if USE_SIMD
            Fast.Vector3 res= (*(Fast.Vector3*)&value1 * *(Fast.Vector3*)&value2);
            return *(Vector3*)&res;
#else
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            value1.Z *= value2.Z;
            return value1;
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static Vector3 operator /(Vector3 value, float scalar)
        {
#if USE_SIMD
            Fast.Vector3 res= (*(Fast.Vector3*)&value / scalar);
            return *(Vector3*)&res;
#else
            value.X /= scalar;
            value.Y /= scalar;
            value.Z /= scalar;
            return value;
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static Vector3 operator /(Vector3 value1, Vector3 value2)//TODO: ugly as hell?
        {
#if USE_SIMD
            Fast.Vector3 res= (*(Fast.Vector3*)&value1 / *(Fast.Vector3*)&value2);
            return *(Vector3*)&res;
#else
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            value1.Z /= value2.Z;
            return value1;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static Vector3 Cross(Vector3 value1, Vector3 value2)
        {
#if USE_SIMD
            Fast.Vector3 a1 = new Fast.Vector3(value1.Y,value1.Z,value1.X);
            Fast.Vector3 a2 = new Fast.Vector3(value2.Z,value2.X,value2.Y);
            Fast.Vector3 b1 = new Fast.Vector3(value1.Z,value1.X,value1.Y);
            Fast.Vector3 b2 = new Fast.Vector3(value2.Y,value2.Z,value2.X);

            Fast.Vector3 res = a1*a2 - b1*b2;
            return *(Vector3*)&res;
#else
            return new Vector3(value1.Y*value2.Z - value1.Z*value2.Y,
                value1.Z*value2.X - value1.X*value2.Z,
                value1.X*value2.Y - value1.Y*value2.X);
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max)
        {
            return Vector3.Min(Vector3.Max(min,value),max);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void Clamp(Vector3 value, Vector3 min, Vector3 max, out Vector3 output)
        {
            output = Vector3.Min(Vector3.Max(min,value),max);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(Vector3 value1, Vector3 value2)
        {
            Vector3 res = value1*value2;
            return res.X+res.Y+res.Z;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(Vector3 value1, Vector3 value2)
        {
            return (value1 - value2).Length;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceSquared(Vector3 value1, Vector3 value2)
        {
            return (value1 - value2).LengthSquared;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Lerp(Vector3 value1, Vector3 value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static Vector3 Max(Vector3 value1, Vector3 value2)
        {
#if USE_SIMD
            Fast.Vector3 res= Fast.Vector3.Max(*(Fast.Vector3*)&value1,*(Fast.Vector3*)&value2);
            return *(Vector3*)&res;
#else
            return new Vector3(Math.Max(value1.X, value2.X), Math.Max(value1.Y, value2.Y), Math.Max(value1.Z, value2.Z));
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static Vector3 Min(Vector3 value1, Vector3 value2)
        {
#if USE_SIMD
            Fast.Vector3 res = Fast.Vector3.Min(*(Fast.Vector3*)&value1,*(Fast.Vector3*)&value2);
            return *(Vector3*)&res;
#else
            return new Vector3(Math.Min(value1.X, value2.X), Math.Min(value1.Y, value2.Y), Math.Min(value1.Z, value2.Z));
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Reflect(Vector3 vector, Vector3 normal)
        {
            normal.Normalize();
            return 2 * (normal.Dot(vector) * normal - vector); //TODO: normalize normal?
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Transform(Vector3 position, Matrix matrix)
        {
            return new Vector3(position.X * matrix.M11 + position.Y * matrix.M21 + position.Z * matrix.M31 + matrix.M41,
                position.X * matrix.M12 + position.Y * matrix.M22 + position.Z * matrix.M32 + matrix.M42,
                position.X * matrix.M13 + position.Y * matrix.M23 + position.Z * matrix.M33 + matrix.M43);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Transform(Vector3 position,Quaternion quaternion)
        {
            return Vector3.Transform(position,Matrix.CreateFromQuaternion(quaternion));
        }

        public static readonly Vector3 One = new Vector3(1, 1, 1);
        public static readonly Vector3 Zero;

        public static readonly Vector3 UnitX = new Vector3(1, 0);
        public static readonly Vector3 UnitY = new Vector3(0, 1);
        public static readonly Vector3 UnitZ = new Vector3(0, 0, 1);
    }
}

