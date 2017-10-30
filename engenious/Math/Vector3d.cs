using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Fast = System.Numerics;
namespace engenious
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [System.ComponentModel.TypeConverter(typeof(Vector3dConverter))]
    public unsafe struct Vector3d:IEquatable<Vector3d>
    {
        public double X
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]set;
        }
        public double Y
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]set;
        }
        public double Z
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]set;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3d(double w)
        {
            X = w;
            Y = w;
            Z = w;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3d(double x, double y, double z=0.0f)
        {
            X = x;
            Y = y;
            Z = z;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Dot(Vector3d value2)
        {
            return Dot(this,value2);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3d Cross(Vector3d value2)
        {
            return Cross(this,value2);
        }
        [System.ComponentModel.Browsable(false)]
        public double Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get{return (double)Math.Sqrt(LengthSquared);}
        }

        [System.ComponentModel.Browsable(false)]
        public double LengthSquared => Dot(this,this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Normalize()
        {
            this /= Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3d Normalized()
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
            if (obj is Vector3d)
                return Equals((Vector3d)obj);
            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector3d other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        #endregion
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector3d value1, Vector3d value2)
        {
            return value1.X == value2.X && value1.Y == value2.Y && value1.Z == value2.Z;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector3d value1, Vector3d value2)
        {
            return value1.X != value2.X || value1.Y != value2.Y || value1.Z != value2.Z;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator +(Vector3d value1, Vector3d value2)
        {
#if USE_SIMD
            Fast.Vector3d res= (*(Fast.Vector3d*)&value1 + *(Fast.Vector3d*)&value2);
            return *(Vector3d*)&res;
#else
            value1.X += value2.X;
            value1.Y += value2.Y;
            value1.Z += value2.Z;
            return value1;
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator -(Vector3d value1, Vector3d value2)
        {
#if USE_SIMD
            Fast.Vector3d res= (*(Fast.Vector3d*)&value1 - *(Fast.Vector3d*)&value2);
            return *(Vector3d*)&res;
#else
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            value1.Z -= value2.Z;
            return value1;
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator -(Vector3d value)
        {
#if USE_SIMD
            Fast.Vector3d res= (-*(Fast.Vector3d*)&value);
            return *(Vector3d*)&res;
#else
            value.X = -value.X;
            value.Y = -value.Y;
            value.Z = -value.Z;
            return value;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator *(Vector3d value, double scalar)
        {
#if USE_SIMD
            Fast.Vector3d res= (scalar * *(Fast.Vector3d*)&value);
            return *(Vector3d*)&res;
#else
            value.X *= scalar;
            value.Y *= scalar;
            value.Z *= scalar;
            return value;
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator *(double scalar, Vector3d value)
        {
            return value * scalar;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator *(Vector3d value1, Vector3d value2)//TODO: ugly as hell
        {
#if USE_SIMD
            Fast.Vector3d res= (*(Fast.Vector3d*)&value1 * *(Fast.Vector3d*)&value2);
            return *(Vector3d*)&res;
#else
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            value1.Z *= value2.Z;
            return value1;
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator /(Vector3d value, double scalar)
        {
#if USE_SIMD
            Fast.Vector3d res= (*(Fast.Vector3d*)&value / scalar);
            return *(Vector3d*)&res;
#else
            value.X /= scalar;
            value.Y /= scalar;
            value.Z /= scalar;
            return value;
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator /(Vector3d value1, Vector3d value2)//TODO: ugly as hell?
        {
#if USE_SIMD
            Fast.Vector3d res= (*(Fast.Vector3d*)&value1 / *(Fast.Vector3d*)&value2);
            return *(Vector3d*)&res;
#else
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            value1.Z /= value2.Z;
            return value1;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d Cross(Vector3d value1, Vector3d value2)
        {
#if USE_SIMD
            Fast.Vector3d a1 = new Fast.Vector3d(value1.Y,value1.Z,value1.X);
            Fast.Vector3d a2 = new Fast.Vector3d(value2.Z,value2.X,value2.Y);
            Fast.Vector3d b1 = new Fast.Vector3d(value1.Z,value1.X,value1.Y);
            Fast.Vector3d b2 = new Fast.Vector3d(value2.Y,value2.Z,value2.X);

            Fast.Vector3d res = a1*a2 - b1*b2;
            return *(Vector3d*)&res;
#else
            return new Vector3d(value1.Y*value2.Z - value1.Z*value2.Y,
                value1.Z*value2.X - value1.X*value2.Z,
                value1.X*value2.Y - value1.Y*value2.X);
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d Clamp(Vector3d value, Vector3d min, Vector3d max)
        {
            return Min(Max(min,value),max);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clamp(Vector3d value, Vector3d min, Vector3d max, out Vector3d output)
        {
            output = Min(Max(min,value),max);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Dot(Vector3d value1, Vector3d value2)
        {
            var res = value1*value2;
            return res.X+res.Y+res.Z;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Distance(Vector3d value1, Vector3d value2)
        {
            return (value1 - value2).Length;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DistanceSquared(Vector3d value1, Vector3d value2)
        {
            return (value1 - value2).LengthSquared;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d Lerp(Vector3d value1, Vector3d value2, double amount)
        {
            return value1 + (value2 - value1) * amount;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d Max(Vector3d value1, Vector3d value2)
        {
#if USE_SIMD
            Fast.Vector3d res= Fast.Vector3d.Max(*(Fast.Vector3d*)&value1,*(Fast.Vector3d*)&value2);
            return *(Vector3d*)&res;
#else
            return new Vector3d(Math.Max(value1.X, value2.X), Math.Max(value1.Y, value2.Y), Math.Max(value1.Z, value2.Z));
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d Min(Vector3d value1, Vector3d value2)
        {
#if USE_SIMD
            Fast.Vector3d res = Fast.Vector3d.Min(*(Fast.Vector3d*)&value1,*(Fast.Vector3d*)&value2);
            return *(Vector3d*)&res;
#else
            return new Vector3d(Math.Min(value1.X, value2.X), Math.Min(value1.Y, value2.Y), Math.Min(value1.Z, value2.Z));
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d Reflect(Vector3d vector, Vector3d normal)
        {
            normal.Normalize();
            return 2 * (normal.Dot(vector) * normal - vector); //TODO: normalize normal?
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d Transform(Vector3d position, Matrix matrix)
        {
            return new Vector3d(position.X * matrix.M11 + position.Y * matrix.M21 + position.Z * matrix.M31 + matrix.M41,
                position.X * matrix.M12 + position.Y * matrix.M22 + position.Z * matrix.M32 + matrix.M42,
                position.X * matrix.M13 + position.Y * matrix.M23 + position.Z * matrix.M33 + matrix.M43);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d Transform(Vector3d position,Quaternion quaternion)
        {
            return Transform(position,Matrix.CreateFromQuaternion(quaternion));
        }

        public static readonly Vector3d One = new Vector3d(1, 1, 1);
        public static readonly Vector3d Zero;

        public static readonly Vector3d UnitX = new Vector3d(1, 0);
        public static readonly Vector3d UnitY = new Vector3d(0, 1);
        public static readonly Vector3d UnitZ = new Vector3d(0, 0, 1);
    }
}

