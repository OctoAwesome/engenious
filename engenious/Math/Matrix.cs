using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Fast = System.Numerics;
namespace engenious
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    [TypeConverter(typeof(MatrixConverter))]
    public struct Matrix :IEquatable<Matrix>
    {
        [FieldOffset(0)]
        private unsafe fixed float items[16];
        [FieldOffset(0)]
        public float M11;
        [FieldOffset(4)]
        public float M12;
        [FieldOffset(8)]
        public float M13;
        [FieldOffset(12)]
        public float M14;

        [FieldOffset(16)]
        public float M21;
        [FieldOffset(20)]
        public float M22;
        [FieldOffset(24)]
        public float M23;
        [FieldOffset(28)]
        public float M24;

        [FieldOffset(32)]
        public float M31;
        [FieldOffset(36)]
        public float M32;
        [FieldOffset(40)]
        public float M33;
        [FieldOffset(44)]
        public float M34;

        [FieldOffset(48)]
        public float M41;
        [FieldOffset(52)]
        public float M42;
        [FieldOffset(56)]
        public float M43;
        [FieldOffset(60)]
        public float M44;

        [FieldOffset(0)]
        public Vector4 Row0;
        [FieldOffset(16)]
        public Vector4 Row1;
        [FieldOffset(32)]
        public Vector4 Row2;
        [FieldOffset(48)]
        public Vector4 Row3;

        public Matrix(float m11, float m21, float m31, float m41,
                      float m12, float m22, float m32, float m42,
                      float m13, float m23, float m33, float m43,
                      float m14, float m24, float m34, float m44)
            : this()
        {
            M11 = m11;
            M21 = m21;
            M31 = m31;
            M41 = m41;

            M12 = m12;
            M22 = m22;
            M32 = m32;
            M42 = m42;

            M13 = m13;
            M23 = m23;
            M33 = m33;
            M43 = m43;

            M14 = m14;
            M24 = m24;
            M34 = m34;
            M44 = m44;
        }

        public Matrix(Vector4 row0, Vector4 row1, Vector4 row2, Vector4 row3)
            : this()
        {
            Row0 = row0;
            Row1 = row1;
            Row2 = row2;
            Row3 = row3;
        }

        public Vector3 Translation
        {
            get{ return new Vector3(M41, M42, M43); }
            set
            {
                M41 = value.X;
                M42 = value.Y;
                M43 = value.Z;
            }
        }

        public Vector4 Column0
        {
            get { return new Vector4(Row0.X, Row1.X, Row2.X, Row3.X); }
            set
            {
                Row0.X = value.X;
                Row1.X = value.Y;
                Row2.X = value.Z;
                Row3.X = value.W;
            }
        }

        public Vector4 Column1
        {
            get { return new Vector4(Row0.Y, Row1.Y, Row2.Y, Row3.Y); }
            set
            {
                Row0.Y = value.X;
                Row1.Y = value.Y;
                Row2.Y = value.Z;
                Row3.Y = value.W;
            }
        }

        public Vector4 Column2
        {
            get { return new Vector4(Row0.Z, Row1.Z, Row2.Z, Row3.Z); }
            set
            {
                Row0.Z = value.X;
                Row1.Z = value.Y;
                Row2.Z = value.Z;
                Row3.Z = value.W;
            }
        }

        public Vector4 Column3
        {
            get { return new Vector4(Row0.W, Row1.W, Row2.W, Row3.W); }
            set
            {
                Row0.W = value.X;
                Row1.W = value.Y;
                Row2.W = value.Z;
                Row3.W = value.W;
            }
        }

        public float Determinant =>   M11 * M22 * M33 * M44 - M11 * M22 * M34 * M43 + M11 * M23 * M34 * M42 - M11 * M23 * M32 * M44
                                    + M11 * M24 * M32 * M43 - M11 * M24 * M33 * M42 - M12 * M23 * M34 * M41 + M12 * M23 * M31 * M44
                                    - M12 * M24 * M31 * M43 + M12 * M24 * M33 * M41 - M12 * M21 * M33 * M44 + M12 * M21 * M34 * M43
                                    + M13 * M24 * M31 * M42 - M13 * M24 * M32 * M41 + M13 * M21 * M32 * M44 - M13 * M21 * M34 * M42
                                    + M13 * M22 * M34 * M41 - M13 * M22 * M31 * M44 - M14 * M21 * M32 * M43 + M14 * M21 * M33 * M42
                                    - M14 * M22 * M33 * M41 + M14 * M22 * M31 * M43 - M14 * M23 * M31 * M42 + M14 * M23 * M32 * M41;

        public unsafe float this [int columnIndex, int rowIndex]
        {
            get
            {
                if (rowIndex < 0 || columnIndex < 0 || rowIndex >= 4 || columnIndex >= 4)
                    throw new IndexOutOfRangeException();
                fixed(float* ptr = items)
                    return ptr[rowIndex + columnIndex * 4];
            }
            set
            {
                if (rowIndex < 0 || columnIndex < 0 || rowIndex >= 4 || columnIndex >= 4)
                    throw new IndexOutOfRangeException();
                fixed(float* ptr = items)
                    ptr[rowIndex + columnIndex * 4] = value;
            }
        }

        public unsafe float this [int index]
        {
            get
            {
                if (index < 0 || index >= 16)
                    throw new IndexOutOfRangeException();
                fixed(float* ptr = items)
                    return ptr[index];
            }
            set
            {
                if (index < 0 || index >= 16)
                    throw new IndexOutOfRangeException();
                fixed(float* ptr = items)
                    ptr[index] = value;
            }
        }

        public void Transpose()
        {
            this = new Matrix(M11, M12, M13, M14,
                M21, M22, M23, M24,
                M31, M32, M33, M34,
                M41, M42, M43, M44);
        }

        public Matrix Transposed()
        {
            return new Matrix(M11, M12, M13, M14,
                M21, M22, M23, M24,
                M31, M32, M33, M34,
                M41, M42, M43, M44);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        #region IEquatable implementation

        public override bool Equals(object other)
        {
            if (other is Matrix)
                return Equals((Matrix)other);
            return false;
        }

        public bool Equals(Matrix other)
        {
            return this == other;
        }

        #endregion

        public static unsafe bool operator==(Matrix value1, Matrix value2)
        {
            for (var i = 0; i < 16; i++)
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value1.items[i] != value2.items[i])
                    return false;
            }
            return true;
        }

        public static unsafe bool operator!=(Matrix value1, Matrix value2)
        {
            for (var i = 0; i < 16; i++)
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value1.items[i] != value2.items[i])
                    return true;
            }
            return false;
        }

        public static unsafe Matrix operator+(Matrix value1, Matrix value2)
        {
            for (var i = 0; i < 16; i++)
            {
                value1.items[i] += value2.items[i];
            }
            return value1;
        }

        public static unsafe Matrix operator-(Matrix value1, Matrix value2)
        {
            for (var i = 0; i < 16; i++)
            {
                value1.items[i] -= value2.items[i];
            }
            return value1;
        }

        public static unsafe Matrix operator*(float scalar, Matrix value)
        {
            for (var i = 0; i < 16; i++)
            {
                value.items[i] *= scalar;
            }
            return value;
        }

        public static Matrix operator*(Matrix value, float scalar)
        {
            return scalar * value;
        }

        public static unsafe Matrix operator*(Matrix value1, Matrix value2)
        {
            var multiply = new Matrix();
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    float sum = 0;
                    for (var k = 0; k < 4; k++)
                    {
                        sum = sum + value1.items[i + k * 4] * value2.items[k + j * 4];
                    }

                    multiply[i + j * 4] = sum;
                }
            }

            return multiply;
            /*return new Matrix(value1.M11 * value2.M11 + value1.M21 * value2.M12 + value1.M31 * value2.M13 + value1.M41 * value2.M14,
                value1.M12 * value2.M11 + value1.M22 * value2.M12 + value1.M32 * value2.M13 + value1.M42 * value2.M14,
                value1.M13 * value2.M11 + value1.M23 * value2.M12 + value1.M33 * value2.M13 + value1.M43 * value2.M14,
                value1.M14 * value2.M11 + value1.M24 * value2.M12 + value1.M34 * value2.M13 + value1.M44 * value2.M14,

                value1.M11 * value2.M21 + value1.M21 * value2.M22 + value1.M31 * value2.M23 + value1.M41 * value2.M24,
                value1.M12 * value2.M21 + value1.M22 * value2.M22 + value1.M32 * value2.M23 + value1.M42 * value2.M24,
                value1.M13 * value2.M21 + value1.M23 * value2.M22 + value1.M33 * value2.M23 + value1.M43 * value2.M24,
                value1.M14 * value2.M21 + value1.M24 * value2.M22 + value1.M34 * value2.M23 + value1.M44 * value2.M24,

                value1.M11 * value2.M31 + value1.M21 * value2.M32 + value1.M31 * value2.M33 + value1.M41 * value2.M34,
                value1.M12 * value2.M31 + value1.M22 * value2.M32 + value1.M32 * value2.M33 + value1.M42 * value2.M34,
                value1.M13 * value2.M31 + value1.M23 * value2.M32 + value1.M33 * value2.M33 + value1.M43 * value2.M34,
                value1.M14 * value2.M31 + value1.M24 * value2.M32 + value1.M34 * value2.M33 + value1.M44 * value2.M34,

                value1.M11 * value2.M41 + value1.M21 * value2.M42 + value1.M31 * value2.M43 + value1.M41 * value2.M44,
                value1.M12 * value2.M41 + value1.M22 * value2.M42 + value1.M32 * value2.M43 + value1.M42 * value2.M44,
                value1.M13 * value2.M41 + value1.M23 * value2.M42 + value1.M33 * value2.M43 + value1.M43 * value2.M44,
                value1.M14 * value2.M41 + value1.M24 * value2.M42 + value1.M34 * value2.M43 + value1.M44 * value2.M44);*/

        }


        public static void CreatePerspectiveFieldOfView(float fovY, float aspect, float near, float far, out Matrix result)
        {

            var tangent = (float)Math.Tan(fovY / 2/* * DEG2RAD*/); // tangent of half fovY
            var height = near * tangent;         // half height of near plane
            var width = height * aspect;          // half width of near plane

            // params: left, right, bottom, top, near, far
            CreatePerspectiveOffCenter(-width, width, height, -height, near, far, out result);

        }

        public static Matrix CreatePerspectiveFieldOfView(float fovY, float aspect, float near, float far)
        {
            var tangent = (float)Math.Tan(fovY / 2/* * DEG2RAD*/); // tangent of half fovY
            var height = near * tangent;         // half height of near plane
            var width = height * aspect;          // half width of near plane

            // params: left, right, bottom, top, near, far
            Matrix result;
            CreatePerspectiveOffCenter(-width, width, height, -height, near, far, out result);
            return result;
        }

        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public static unsafe void CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far, out Matrix result)
        {
            if (left  == right)
                throw new ArgumentOutOfRangeException($"{nameof(left)} or {nameof(right)}");
            if (bottom == top)
                throw new ArgumentOutOfRangeException($"{nameof(bottom)} or {nameof(top)}");
            if (near == far)
                throw new ArgumentOutOfRangeException($"{nameof(near)} or {nameof(far)}");
            var m = Identity;
            m.items[0] = 2.0f * near / (right - left);
            m.items[5] = 2.0f * near / (top - bottom);
            m.items[8] = (right + left) / (right - left);
            m.items[9] = (top + bottom) / (top - bottom);
            m.items[10] = -(far + near) / (far - near);
            m.items[11] = -1;
            m.items[12] = 0;
            m.items[13] = 0;
            m.items[14] = -2 * (far * near) / (far - near);
            m.items[15] = 0;
            result = m;
        }

        public static unsafe Matrix CreateOrthographic(float width, float height, float near, float far)
        {
            var res = Identity;
            res.items[0] = 2f / width;
            res.items[5] = -2f / height;
            res.items[10] = 1f / (near - far);
            res.items[12] = res.items[13] = 0;
            res.items[14] = -(far + near) / (far - near);

            return res;
        }

        public unsafe static Matrix CreateOrthographicOffCenter(float left, float right, float bottom, float top, float near, float far)
        {


            var res = Identity;
            res.items[0] = 2.0f / (right - left);
            res.items[5] = 2.0f / (top - bottom);
            res.items[10] = -2.0f / (far - near);
            res.items[12] = -(right + left) / (right - left);
            res.items[13] = -(top + bottom) / (top - bottom);
            res.items[14] = -(far + near) / (far - near);
            return res;
        }

        public static void CreateOrthographicOffCenter(float left, float right, float bottom, float top, float near, float far, out Matrix result)
        {
            result = CreateOrthographicOffCenter(left, right, bottom, top, near, far);
        }

        public unsafe static Matrix CreateLookAt(Vector3 eyePos, Vector3 lookAt, Vector3 up)
        {


            var forward = (lookAt - eyePos).Normalized();
            up = up.Normalized();
            var side = forward.Cross(up).Normalized();


            var newUp = side.Cross(forward).Normalized();

            var m = new Matrix();
            m.items[0] = side.X;
            m.items[4] = side.Y;
            m.items[8] = side.Z;

            m.items[1] = newUp.X;
            m.items[5] = newUp.Y;
            m.items[9] = newUp.Z;

            m.items[2] = -forward.X;
            m.items[6] = -forward.Y;
            m.items[10] = -forward.Z;

            m.items[3] = m.items[7] = m.items[11] = 0.0f;

            m.items[12] = -side.Dot(eyePos);
            m.items[13] = -newUp.Dot(eyePos);
            m.items[14] = forward.Dot(eyePos);
            m.items[15] = 1.0f;


            return m;
        }

        public static Matrix CreateFromQuaternion(Quaternion quat)
        {
            return CreateFromQuaternion(quat.X, quat.Y, quat.Z, quat.W);
        }

        public static Matrix CreateFromQuaternion(float x, float y, float z, float w)
        {
            var n = w * w + x * x + y * y + z * z;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            var s = n == 0 ? 0 : 2 / n;

            var sw = s * w;
            var sx = s * x;
            var sy = s * y;

            var wx = sw * x;
            var wy = sw * y;
            var wz = sw * z;
            var xx = sx * x;
            var xy = sx * y;
            var xz = sx * z;
            var yy = sy * y;
            var yz = sy * z;
            var zz = s * z * z;

            return new Matrix(1 - (yy + zz), xy - wz, xz + wy, 0,
                xy + wz, 1 - (xx + zz), yz - wx, 0,
                xz - wy, yz + wx, 1 - (xx + yy), 0,
                0, 0, 0, 1);
        }

        public static Matrix CreateScaling(float x, float y, float z)
        {
            return new Matrix(x, 0, 0, 0,
                0, y, 0, 0,
                0, 0, z, 0,
                0, 0, 0, 1);
        }

        public static Matrix CreateScaling(Vector3 vec)
        {
            return CreateScaling(vec.X, vec.Y, vec.Z);
        }

        public static Matrix CreateTranslation(Vector3 translation)
        {
            return CreateTranslation(translation.X, translation.Y, translation.Z);
        }

        public static Matrix CreateTranslation(float x, float y, float z)
        {
            var res = Identity;
            res.M41 = x;
            res.M42 = y;
            res.M43 = z;
            return res;

        }

        public static Matrix CreateRotationX(float rot)
        {
            var ret = Identity;
            ret.M22 = ret.M33 = (float)Math.Cos(rot);
            ret.M32 = (float)Math.Sin(rot);
            ret.M23 = -ret.M32;
            return ret;
        }

        public static Matrix CreateRotationY(float rot)
        {
            var ret = Identity;
            ret.M11 = ret.M33 = (float)Math.Cos(rot);
            ret.M13 = (float)Math.Sin(rot);
            ret.M31 = -ret.M13;
            return ret;
        }

        public static Matrix CreateRotationZ(float rot)
        {
            var ret = Identity;
            ret.M11 = ret.M22 = (float)Math.Cos(rot);
            ret.M12 = (float)Math.Sin(rot);
            ret.M21 = -ret.M12;
            return ret;
        }

        public static unsafe Matrix Lerp(Matrix val1, Matrix val2, float amount)
        {
            var res = new Matrix();
            for (var i = 0; i < 16; i++)
            {
                res.items[i] = val1.items[i] + (val2.items[i] - val1.items[i]) * amount;
            }
            return res;
        }

        public static Matrix Invert(Matrix m)
        {
            float det;
            var inv = new Matrix();
            inv[0] = m[5] * m[10] * m[15] -
            m[5] * m[11] * m[14] -
            m[9] * m[6] * m[15] +
            m[9] * m[7] * m[14] +
            m[13] * m[6] * m[11] -
            m[13] * m[7] * m[10];
            inv[4] = -m[4] * m[10] * m[15] +
            m[4] * m[11] * m[14] +
            m[8] * m[6] * m[15] -
            m[8] * m[7] * m[14] -
            m[12] * m[6] * m[11] +
            m[12] * m[7] * m[10];
            inv[8] = m[4] * m[9] * m[15] -
            m[4] * m[11] * m[13] -
            m[8] * m[5] * m[15] +
            m[8] * m[7] * m[13] +
            m[12] * m[5] * m[11] -
            m[12] * m[7] * m[9];
            inv[12] = -m[4] * m[9] * m[14] +
            m[4] * m[10] * m[13] +
            m[8] * m[5] * m[14] -
            m[8] * m[6] * m[13] -
            m[12] * m[5] * m[10] +
            m[12] * m[6] * m[9];
            inv[1] = -m[1] * m[10] * m[15] +
            m[1] * m[11] * m[14] +
            m[9] * m[2] * m[15] -
            m[9] * m[3] * m[14] -
            m[13] * m[2] * m[11] +
            m[13] * m[3] * m[10];
            inv[5] = m[0] * m[10] * m[15] -
            m[0] * m[11] * m[14] -
            m[8] * m[2] * m[15] +
            m[8] * m[3] * m[14] +
            m[12] * m[2] * m[11] -
            m[12] * m[3] * m[10];
            inv[9] = -m[0] * m[9] * m[15] +
            m[0] * m[11] * m[13] +
            m[8] * m[1] * m[15] -
            m[8] * m[3] * m[13] -
            m[12] * m[1] * m[11] +
            m[12] * m[3] * m[9];
            inv[13] = m[0] * m[9] * m[14] -
            m[0] * m[10] * m[13] -
            m[8] * m[1] * m[14] +
            m[8] * m[2] * m[13] +
            m[12] * m[1] * m[10] -
            m[12] * m[2] * m[9];
            inv[2] = m[1] * m[6] * m[15] -
            m[1] * m[7] * m[14] -
            m[5] * m[2] * m[15] +
            m[5] * m[3] * m[14] +
            m[13] * m[2] * m[7] -
            m[13] * m[3] * m[6];
            inv[6] = -m[0] * m[6] * m[15] +
            m[0] * m[7] * m[14] +
            m[4] * m[2] * m[15] -
            m[4] * m[3] * m[14] -
            m[12] * m[2] * m[7] +
            m[12] * m[3] * m[6];
            inv[10] = m[0] * m[5] * m[15] -
            m[0] * m[7] * m[13] -
            m[4] * m[1] * m[15] +
            m[4] * m[3] * m[13] +
            m[12] * m[1] * m[7] -
            m[12] * m[3] * m[5];
            inv[14] = -m[0] * m[5] * m[14] +
            m[0] * m[6] * m[13] +
            m[4] * m[1] * m[14] -
            m[4] * m[2] * m[13] -
            m[12] * m[1] * m[6] +
            m[12] * m[2] * m[5];
            inv[3] = -m[1] * m[6] * m[11] +
            m[1] * m[7] * m[10] +
            m[5] * m[2] * m[11] -
            m[5] * m[3] * m[10] -
            m[9] * m[2] * m[7] +
            m[9] * m[3] * m[6];
            inv[7] = m[0] * m[6] * m[11] -
            m[0] * m[7] * m[10] -
            m[4] * m[2] * m[11] +
            m[4] * m[3] * m[10] +
            m[8] * m[2] * m[7] -
            m[8] * m[3] * m[6];
            inv[11] = -m[0] * m[5] * m[11] +
            m[0] * m[7] * m[9] +
            m[4] * m[1] * m[11] -
            m[4] * m[3] * m[9] -
            m[8] * m[1] * m[7] +
            m[8] * m[3] * m[5];
            inv[15] = m[0] * m[5] * m[10] -
            m[0] * m[6] * m[9] -
            m[4] * m[1] * m[10] +
            m[4] * m[2] * m[9] +
            m[8] * m[1] * m[6] -
            m[8] * m[2] * m[5];
            det = m[0] * inv[0] + m[1] * inv[4] + m[2] * inv[8] + m[3] * inv[12];
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (det == 0)
                throw new ArgumentException("Not invertible",nameof(m));

            det = 1.0f / det;

            for (var i = 0; i < 16; i++)
                inv[i] = inv[i] * det;
            return inv;
        }

        public static readonly Matrix Identity = new Matrix(Vector4.UnitX, Vector4.UnitY, Vector4.UnitZ, Vector4.UnitW);

        public override string ToString()
        {
            return $"[Matrix: {{{Column0},{Column1},{Column2},{Column3}}}]";
        }
    }
}
