using System;
using System.Runtime.InteropServices;

namespace engenious
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Quaternion
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public Quaternion(Matrix matrix)
        {
            //matrix.Transpose();
            //TODO: transpose?
            float tr = matrix.M11 + matrix.M22 + matrix.M33;

            if (tr > 0)
            { 
                float s = (float)(Math.Sqrt(tr + 1.0f) * 2); // S=4*qw
                W = 0.25f * s;
                X = (matrix.M32 - matrix.M23) / s;
                Y = (matrix.M13 - matrix.M31) / s;
                Z = (matrix.M21 - matrix.M12) / s;
            }
            else if ((matrix.M11 > matrix.M22) & (matrix.M11 > matrix.M33))
            { 
                float s = (float)(Math.Sqrt(1.0f + matrix.M11 - matrix.M22 - matrix.M33) * 2); // S=4*qx
                W = (matrix.M32 - matrix.M23) / s;
                X = 0.25f * s;
                Y = (matrix.M12 + matrix.M21) / s;
                Z = (matrix.M13 + matrix.M31) / s;
            }
            else if (matrix.M22 > matrix.M33)
            { 
                float s = (float)(Math.Sqrt(1.0f + matrix.M22 - matrix.M11 - matrix.M33) * 2); // S=4*qy
                W = (matrix.M13 - matrix.M31) / s;
                X = (matrix.M12 + matrix.M21) / s;
                Y = 0.25f * s;
                Z = (matrix.M23 + matrix.M32) / s;
            }
            else
            { 
                float s = (float)(Math.Sqrt(1.0f + matrix.M33 - matrix.M11 - matrix.M22) * 2); // S=4*qz
                W = (matrix.M21 - matrix.M12) / s;
                X = (matrix.M13 + matrix.M31) / s;
                Y = (matrix.M23 + matrix.M32) / s;
                Z = 0.25f * s;
            }
        }

        public Quaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static Quaternion operator +(Quaternion val1, Quaternion val2)
        {
            val1.X += val2.X;
            val1.Y += val2.Y;
            val1.Z += val2.Z;
            val1.W += val2.W;
            return val1;
        }

        public static Quaternion operator*(Quaternion val, float scale)
        {
            return new Quaternion(val.X * scale, val.Y * scale, val.Z * scale, val.W * scale);
        }

        public static Quaternion operator*(Quaternion val1, Quaternion val2)
        {
            return new Quaternion(val1.W * val2.X + val1.X * val2.W + val1.Y * val2.Z - val1.Z * val2.Y, val1.W * val2.Y + val1.Y * val2.W + val1.Z * val2.X - val1.X * val2.Z, val1.W * val2.Z + val1.Z * val2.W + val1.X * val2.Y - val1.Y * val2.X, val1.W * val2.W - val1.X * val2.X - val1.Y * val2.Y - val1.Z * val2.Z);
        }

        public static Quaternion Lerp(Quaternion quaternion1, Quaternion quaternion2, float amount)//copied from MonoGame
        {
            float num = amount;
            float num2 = 1f - num;
            Quaternion quaternion = new Quaternion();
            float num5 = (((quaternion1.X * quaternion2.X) + (quaternion1.Y * quaternion2.Y)) + (quaternion1.Z * quaternion2.Z)) + (quaternion1.W * quaternion2.W);
            if (num5 >= 0f)
            {
                quaternion.X = (num2 * quaternion1.X) + (num * quaternion2.X);
                quaternion.Y = (num2 * quaternion1.Y) + (num * quaternion2.Y);
                quaternion.Z = (num2 * quaternion1.Z) + (num * quaternion2.Z);
                quaternion.W = (num2 * quaternion1.W) + (num * quaternion2.W);
            }
            else
            {
                quaternion.X = (num2 * quaternion1.X) - (num * quaternion2.X);
                quaternion.Y = (num2 * quaternion1.Y) - (num * quaternion2.Y);
                quaternion.Z = (num2 * quaternion1.Z) - (num * quaternion2.Z);
                quaternion.W = (num2 * quaternion1.W) - (num * quaternion2.W);
            }
            float num4 = (((quaternion.X * quaternion.X) + (quaternion.Y * quaternion.Y)) + (quaternion.Z * quaternion.Z)) + (quaternion.W * quaternion.W);
            if (num4 == 0)
                return quaternion;
            float num3 = (float)(1.0 / Math.Sqrt(num4));
            quaternion.X *= num3;
            quaternion.Y *= num3;
            quaternion.Z *= num3;
            quaternion.W *= num3;
            return quaternion;
        }

        public Matrix ToMatrix()
        {
            Matrix m=new Matrix();

            float x2 = 2*X*X;
            float y2 = 2*Y*Y;
            float z2 = 2*Z*Z;

            float xy = 2*X*Y;
            float xz = 2*X*Z;
            float xw = 2*X*W;

            float yz = 2*Y*Z;
            float yw = 2*Y*W;

            float zw = 2*Z*W;


            m.M11 = 1 - y2-z2;
            m.M12 = xy-zw;
            m.M13 = xz+ yw;

            m.M21 = xy+zw;
            m.M22 = 1-x2-z2;
            m.M23 = yz-xw;

            m.M31 = xz-yw;
            m.M32 = yz+xw;
            m.M33 = 1-x2-y2;
            //TODO: transpose?
            return m;
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}, {2}, {3}]", X.ToString(System.Globalization.NumberFormatInfo.InvariantInfo), Y.ToString(System.Globalization.NumberFormatInfo.InvariantInfo), Z.ToString(System.Globalization.NumberFormatInfo.InvariantInfo), W.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
        }

        public static bool operator ==(Quaternion q1, Quaternion q2)
        {
            return q1.X == q2.X && q1.Y == q2.Y && q1.Z == q2.Z && q1.W == q2.W;
        }
        public static bool operator !=(Quaternion q1, Quaternion q2)
        {
            return q1.X != q2.X || q1.Y != q2.Y || q1.Z != q2.Z || q1.W != q2.W;
        }

        public static readonly Quaternion Identity = new Quaternion(0,0,0,1);
    }
}

