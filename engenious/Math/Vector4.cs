using System;
using System.Runtime.InteropServices;

namespace engenious
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [System.ComponentModel.TypeConverter(typeof(Vector4Converter))]
    public struct Vector4
    {
        public float X{get;set;}
        public float Y{get;set;}
        public float Z{get;set;}
        public float W{get;set;}

        public Vector4(float val)
        {
            X = val;
            Y = val;
            Z = val;
            W = val;
        }

        public Vector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public Vector4(Vector2 val, float z, float w)
        {
            X = val.X;
            Y = val.Y;
            Z = z;
            W = w;
        }

        public Vector4(Vector3 val, float w)
        {
            X = val.X;
            Y = val.Y;
            Z = val.Z;
            W = w;
        }

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

        public override string ToString()
        {
            return $"{{{X}, {Y}, {Z}, {W}}}";
        }
    }
}
