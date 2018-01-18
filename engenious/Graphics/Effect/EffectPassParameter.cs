using System;
using System.Linq;
using OpenTK.Graphics.OpenGL4;

namespace engenious.Graphics
{
    public class EffectPassParameter
    {
        internal int Location;
        internal EffectPass Pass;
        internal ActiveUniformType Type;

        internal EffectPassParameter(EffectPass pass, string name, int location,ActiveUniformType type=(ActiveUniformType)0x7FFFFFFF)
        {
            Pass = pass;
            Location = location;
            Name = name;
            Type = type;
        }

        public string Name{ get; private set; }

        public void SetValue(bool value)
        {
            Pass.Apply();
            GL.Uniform1(Location, value ? 1 : 0);
        }

        public void SetValue(bool[] values)
        {
            Pass.Apply();
            GL.Uniform1(Location, values.Length, values.Cast<int>().ToArray());
        }

        public void SetValue(int value)
        {
            Pass.Apply();
            GL.Uniform1(Location, value);
        }

        public void SetValue(int[] values)
        {
            Pass.Apply();
            GL.Uniform1(Location, values.Length, values);
        }

        public void SetValue(uint value)
        {
            GL.Uniform1(Location, value);
        }

        public void SetValue(uint[] values)
        {
            GL.Uniform1(Location, values.Length, values);
        }

        public void SetValue(float value)
        {
            GL.Uniform1(Location, value);
        }

        public void SetValue(float[] values)
        {
            GL.Uniform1(Location, values.Length, values);
        }
        
        public void SetValue(double values)
        {
            GL.Uniform1(Location,values);
        }
        
        public void SetValue(double[] values)
        {
            GL.Uniform1(Location,values.Length,values);
        }

        public void SetValue(string value)
        {
            throw new NotImplementedException();
        }

        public void SetValue(Texture value)
        {
            var dev = value.GraphicsDevice;
            var val = dev.Textures.InsertFree(value);
            if (val == -1)
                throw new Exception("Out of textures");
            GL.Uniform1(Location, val);
        }

        public unsafe void SetValue(Vector2 value)
        {
            GL.Uniform2(Location, 1, (float*)&value);
        }

        public void SetValue(Vector2[] values)
        {
            unsafe
            {
                fixed(Vector2* ptr = values)
                {
                    GL.Uniform2(Location, values.Length, (float*)ptr);//TODO: verify?
                }
            }
        }

        public unsafe void SetValue(Vector3 value)
        {
            GL.Uniform3(Location, 1, (float*)&value);
        }

        public void SetValue(Vector3[] values)
        {
            unsafe
            {
                fixed(Vector3* ptr = values)
                {
                    GL.Uniform3(Location, values.Length, (float*)ptr);//TODO: verify?
                }
            }
        }

        public unsafe void SetValue(Vector4 value)
        {
            GL.Uniform4(Location, 1, (float*)&value);
        }

        public unsafe void SetValue(Vector4[] values)
        {
            fixed(Vector4* ptr = values)
            {
                GL.Uniform4(Location, values.Length, (float*)ptr);//TODO: verify?
            }
        }

        public unsafe void SetValue(Matrix value)
        {
            GL.UniformMatrix4(Location, 1, false, (float*)&value);
        }

        public unsafe void SetValue(Matrix[] values)
        {
            fixed(Matrix* ptr = values)
            {
                GL.UniformMatrix4(Location, values.Length, false, (float*)ptr);//TODO: verify?
            }
        }

        public unsafe void SetValue(Quaternion value)
        {
            GL.Uniform4(Location, 1, (float*)&value);
        }

        public void SetValue(Quaternion[] values)
        {
            unsafe
            {
                fixed(Quaternion* ptr = values)
                {
                    GL.Uniform4(Location, values.Length, (float*)ptr);//TODO: verify?
                }
            }
        }
        public void SetValue(ConstantBuffer value)
        {
            GL.UniformBlockBinding(Pass.Program,Location,value.Ubo);
        }
    }
}

