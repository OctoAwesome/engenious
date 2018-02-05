using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Fast = System.Numerics;
// ReSharper disable CompareOfFloatsByEqualityOperator
namespace engenious
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [System.ComponentModel.TypeConverter(typeof(Vector4Converter))]
    public unsafe struct Vector4
    {
        public float X
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }
        public float Y
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }
        public float Z
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }
        public float W
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector4(float val)
        {
            X = val;
            Y = val;
            Z = val;
            W = val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector4(Vector2 val, float z, float w)
        {
            X = val.X;
            Y = val.Y;
            Z = z;
            W = w;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector4(Vector3 val, float w)
        {
            X = val.X;
            Y = val.Y;
            Z = val.Z;
            W = w;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Dot(Vector4 value2)
        {
            return Dot(this, value2);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector4 Cross(Vector4 value2, Vector4 value3)
        {
            return Cross(this, value2, value3);
        }
        [System.ComponentModel.Browsable(false)]
        public float Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return (float)Math.Sqrt(LengthSquared); }
        }

        [System.ComponentModel.Browsable(false)]
        public float LengthSquared => Dot(this, this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Normalize()
        {
            this /= Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector4 Normalized()
        {
            return this / Length;
        }


        #region IEquatable implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (obj is Vector4)
                return Equals((Vector4)obj);
            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector4 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
        }

        #endregion
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector4 value1, Vector4 value2)
        {
            return value1.X == value2.X && value1.Y == value2.Y && value1.Z == value2.Z && value1.W == value2.W;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector4 value1, Vector4 value2)
        {
            return value1.X != value2.X || value1.Y != value2.Y || value1.Z != value2.Z || value1.W != value2.W;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 operator +(Vector4 value1, Vector4 value2)
        {
#if USE_SIMD
            var tempVal1 = &value1;
            var tempVal2 = &value2;
            Fast.Vector4 res = *(Fast.Vector4*)tempVal1 + *(Fast.Vector4*)tempVal2;

            var tempRes = &res;
            return *(Vector4*)tempRes;
#else
            value1.X += value2.X;
            value1.Y += value2.Y;
            value1.Z += value2.Z;
            value1.W += value2.W;
            return value1;
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 operator -(Vector4 value1, Vector4 value2)
        {
#if USE_SIMD
            var tempVal1 = &value1;
            var tempVal2 = &value2;

            Fast.Vector4 res = *(Fast.Vector4*)tempVal1 - *(Fast.Vector4*)tempVal2;

            var tempRes = &res;
            return *(Vector4*)tempRes;
#else
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            value1.Z -= value2.Z;
            value1.W -= value2.W;
            return value1;
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 operator -(Vector4 value)
        {
#if USE_SIMD
            var tempVal = &value;
            Fast.Vector4 res = -*(Fast.Vector4*)tempVal;

            var tempRes = &res;
            return *(Vector4*)tempRes;
#else
            value.X = -value.X;
            value.Y = -value.Y;
            value.Z = -value.Z;
            value.W = -value.W;
            return value;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 operator *(Vector4 value, float scalar)
        {
#if USE_SIMD
            var tempVal = &value;
            Fast.Vector4 res = *(Fast.Vector4*)tempVal * scalar;

            var tempRes = &res;
            return *(Vector4*)tempRes;
#else
            value.X *= scalar;
            value.Y *= scalar;
            value.Z *= scalar;
            value.W *= scalar;
            return value;
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 operator *(float scalar, Vector4 value)
        {
            return value * scalar;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 operator *(Vector4 value1, Vector4 value2)//TODO: ugly as hell
        {
#if USE_SIMD
            var tempVal1 = &value1;
            var tempVal2 = &value2;
            Fast.Vector4 res = *(Fast.Vector4*)tempVal1 * *(Fast.Vector4*)tempVal2;

            var tempRes = &res;
            return *(Vector4*)tempRes;
#else
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            value1.Z *= value2.Z;
            value1.W *= value2.W;
            return value1;
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 operator /(Vector4 value, float scalar)
        {
#if USE_SIMD
            var tempVal = &value;
            Fast.Vector4 res = *(Fast.Vector4*)tempVal / scalar;

            var tempRes = &res;
            return *(Vector4*)tempRes;
#else
            value.X /= scalar;
            value.Y /= scalar;
            value.Z /= scalar;
            value.W /= scalar;
            return value;
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 operator /(Vector4 value1, Vector4 value2)//TODO: ugly as hell?
        {
#if USE_SIMD
            var tempVal1 = &value1;
            var tempVal2 = &value2;
            Fast.Vector4 res = *(Fast.Vector4*)tempVal1 / *(Fast.Vector4*)tempVal2;

            var tempRes = &res;
            return *(Vector4*)tempRes;
#else
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            value1.Z /= value2.Z;
            value1.W /= value2.W;
            return value1;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Cross(Vector4 value1, Vector4 value2, Vector4 value3)
        {
            throw new NotImplementedException();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Clamp(Vector4 value, Vector4 min, Vector4 max)
        {
            return Min(Max(min, value), max);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clamp(Vector4 value, Vector4 min, Vector4 max, out Vector4 output)
        {
            output = Min(Max(min, value), max);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(Vector4 value1, Vector4 value2)
        {
            var res = value1 * value2;
            return res.X + res.Y + res.Z + res.W;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(Vector4 value1, Vector4 value2)
        {
            return (value1 - value2).Length;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceSquared(Vector4 value1, Vector4 value2)
        {
            return (value1 - value2).LengthSquared;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Lerp(Vector4 value1, Vector4 value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Max(Vector4 value1, Vector4 value2)
        {
#if USE_SIMD
            var tempVal1 = &value1;
            var tempVal2 = &value2;
            Fast.Vector4 res = Fast.Vector4.Max(*(Fast.Vector4*)tempVal1,*(Fast.Vector4*)tempVal2);

            var tempRes = &res;
            return *(Vector4*)tempRes;
#else
            return new Vector4(Math.Max(value1.X, value2.X), Math.Max(value1.Y, value2.Y), Math.Max(value1.Z, value2.Z), Math.Max(value1.W, value2.W));
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Min(Vector4 value1, Vector4 value2)
        {
#if USE_SIMD
            var tempVal1 = &value1;
            var tempVal2 = &value2;
            Fast.Vector4 res = Fast.Vector4.Min(*(Fast.Vector4*)tempVal1,*(Fast.Vector4*)tempVal2);

            var tempRes = &res;
            return *(Vector4*)tempRes;
#else
            return new Vector4(Math.Min(value1.X, value2.X), Math.Min(value1.Y, value2.Y), Math.Min(value1.Z, value2.Z), Math.Min(value1.W, value2.W));
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Reflect(Vector4 vector, Vector4 normal)
        {
            normal.Normalize();
            return 2 * (normal.Dot(vector) * normal - vector); //TODO: normalize normal?
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Transform(Vector4 position, Quaternion quaternion)
        {
            return Transform(position, Matrix.CreateFromQuaternion(quaternion));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Transform(Vector4 position, Matrix matrix)
        {

            return new Vector4(position.X * matrix.M11 + position.Y * matrix.M12 + position.Z * matrix.M13 + position.W * matrix.M14,
                position.X * matrix.M21 + position.Y * matrix.M22 + position.Z * matrix.M23 + position.W * matrix.M24,
                position.X * matrix.M31 + position.Y * matrix.M32 + position.Z * matrix.M33 + position.W * matrix.M34,
                position.X * matrix.M41 + position.Y * matrix.M42 + position.Z * matrix.M43 + position.W * matrix.M44);
        }

        public static readonly Vector4 One = new Vector4(1, 1, 1, 1);
        public static readonly Vector4 Zero;

        public static readonly Vector4 UnitX = new Vector4(1, 0, 0, 0);
        public static readonly Vector4 UnitY = new Vector4(0, 1, 0, 0);
        public static readonly Vector4 UnitZ = new Vector4(0, 0, 1, 0);
        public static readonly Vector4 UnitW = new Vector4(0, 0, 0, 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                hashCode = (hashCode * 397) ^ W.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return
                $"[{X.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)}, {Y.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)}, {Z.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)}]";
        }

    }
}
