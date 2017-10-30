using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenTK;
using Fast = System.Numerics;
// ReSharper disable CompareOfdoublesByEqualityOperator
namespace engenious
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [TypeConverter(typeof(Vector2dConverter))]
    public unsafe struct Vector2d : IEquatable<Vector2d>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2d(double w)
        {
            X = Y = w;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2d(double x, double y)
        {
            X = x;
            Y = y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Dot(Vector2d value2)
        {
            return Dot(this,value2);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2d Cross()
        {
            return new Vector2d(X, -Y);
        }
        [Browsable(false)]
        public double Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get{return (double)Math.Sqrt(LengthSquared);}
        }
        [Browsable(false)]
        public double LengthSquared
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get{return Dot(this,this);}
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Normalize()
        {
            this /= Length;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2d Normalized()
        {
            return this / Length;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2d Transform(Matrix matrix)
        {
            return new Vector2d((X * matrix.M11) + (Y * matrix.M21) + matrix.M41, (X * matrix.M12) + (Y * matrix.M22) + matrix.M42);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        #region IEquatable implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (obj is Vector2d)
                return Equals((Vector2d)obj);
            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector2d other)
        {
            return X == other.X && Y == other.Y;
        }

        #endregion
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector2d value1, Vector2d value2)
        {
            return value1.X == value2.X && value1.Y == value2.Y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector2d value1, Vector2d value2)
        {
            return value1.X != value2.X || value1.Y != value2.Y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2d operator +(Vector2d value1, Vector2d value2)
        {
#if USE_SIMD
            Fast.Vector2d res= (*(Fast.Vector2d*)&value1 + *(Fast.Vector2d*)&value2);
            return *(Vector2d*)&res;
#else
            value1.X += value2.X;
            value1.Y += value2.Y;
            return value1;
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2d operator -(Vector2d value1, Vector2d value2)
        {
#if USE_SIMD
            Fast.Vector2d res= (*(Fast.Vector2d*)&value1 - *(Fast.Vector2d*)&value2);
            return *(Vector2d*)&res;
#else
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            return value1;
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2d operator -(Vector2d value)
        {
            value.X = -value.X;
            value.Y = -value.Y;
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2d operator *(Vector2d value, double scalar)
        {
#if USE_SIMD
            Fast.Vector2d res= (*(Fast.Vector2d*)&value * scalar);
            return *(Vector2d*)&res;
#else
            value.X *= scalar;
            value.Y *= scalar;
            return value;
#endif
        }

        public static Vector2d operator *(double scalar, Vector2d value)
        {
            return value * scalar;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2d operator *(Vector2d value1, Vector2d value2)//TODO: ugly as hell
        {
#if USE_SIMD
            Fast.Vector2d res= (*(Fast.Vector2d*)&value1 * *(Fast.Vector2d*)&value2);
            return *(Vector2d*)&res;
#else
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            return value1;
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2d operator /(Vector2d value, double scalar)
        {
#if USE_SIMD
            Fast.Vector2d res= (*(Fast.Vector2d*)&value / scalar);
            return *(Vector2d*)&res;
#else
            value.X /= scalar;
            value.Y /= scalar;
            return value;
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2d operator /(Vector2d value1, Vector2d value2)//TODO: ugly as hell?
        {
#if USE_SIMD
            Fast.Vector2d res= (*(Fast.Vector2d*)&value1 / *(Fast.Vector2d*)&value2);
            return *(Vector2d*)&res;
#else
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            return value1;
#endif
        }

            
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2d Clamp(Vector2d value, Vector2d min, Vector2d max)
        {
            return Min(Max(value,min),max);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clamp(Vector2d value, Vector2d min, Vector2d max, out Vector2d output)
        {
            output = Min(Max(value,min),max);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Dot(Vector2d value1, Vector2d value2)
        {
#if USE_SIMD
            Fast.Vector2d res= (*(Fast.Vector2d*)&value1 * *(Fast.Vector2d*)&value2);
            return res.X+res.Y;
#else
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            return value1.X+value2.Y;
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Distance(Vector2d value1, Vector2d value2)
        {
            return (value1 - value2).Length;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DistanceSquared(Vector2d value1, Vector2d value2)
        {
            return (value1 - value2).LengthSquared;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2d Lerp(Vector2d value1, Vector2d value2, double amount)
        {
            return value1 + (value2 - value1) * amount;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2d Max(Vector2d value1, Vector2d value2)
        {
#if USE_SIMD
            Fast.Vector2d res = Fast.Vector2d.Max(*(Fast.Vector2d*)&value1,*(Fast.Vector2d*)&value2);
            return *(Vector2d*)&res;
#else
            return new Vector2d(Math.Max(value1.X, value2.X), Math.Max(value1.Y, value2.Y));
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2d Min(Vector2d value1, Vector2d value2)
        {
#if USE_SIMD
            Fast.Vector2d res = Fast.Vector2d.Min(*(Fast.Vector2d*)&value1,*(Fast.Vector2d*)&value2);
            return *(Vector2d*)&res;
#else
            return new Vector2d(Math.Min(value1.X, value2.X), Math.Min(value1.Y, value2.Y));
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2d Reflect(Vector2d vector, Vector2d normal)
        {
            normal.Normalize();
            return 2 * (normal.Dot(vector) * normal - vector); //TODO: normalize normal?
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2d Transform(Vector2d position, Matrix matrix)
        {
            //{v1*x1+v2*y1+w1,v1*x2+v2*y2+w2,v1*x3+v2*y3+w3,v1*x4+v2*y4+w4}
            //{v1*x1+v2*x2+x4,v1*y1+v2*y2+y4,v1*z1+v2*z2+z4,v1*w1+v2*w2+w4}
            return new Vector2d(position.X * matrix.M11 + position.Y * matrix.M12 + matrix.M14, position.X * matrix.M21 + position.Y * matrix.M22 + matrix.M24);//TODO: SIMD
        }

        public static Vector2d TransformNormal(Vector2d normal, Matrix matrix)
        {
            throw new NotImplementedException();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Transform(int count, Vector2d* positions, ref Matrix matrix, Vector2d* output)
        {
            for (var i = 0; i < count; i++, positions++, output++)
            {
                *output = new Vector2d(
                    ((*positions).X * matrix.M11 + (*positions).Y * matrix.M12) + matrix.M14,
                    ((*positions).X * matrix.M21 + (*positions).Y * matrix.M22) + matrix.M24);//TODO: SIMD
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform(Vector2d[] positions, ref Matrix matrix, Vector2d[] output)
        {
            var index = 0;
            foreach (var position in positions)//TODO: SIMD
            {
                output[index++] = new Vector2d(position.X * matrix.M11 + position.Y * matrix.M12 + matrix.M14, position.X * matrix.M21 + position.Y * matrix.M22 + matrix.M24);
            }
        }

        public static readonly Vector2d UnitX = new Vector2d(1, 0);
        public static readonly Vector2d UnitY = new Vector2d(0, 1);
        public static readonly Vector2d One = new Vector2d(1, 1);
        public static readonly Vector2d Zero;
    }
}

