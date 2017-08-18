using System;
using engenious.Helper;
using OpenTK.Graphics.OpenGL4;

namespace engenious.Graphics
{
    internal class VertexAttributes:IDisposable
    {
        internal int Vbo;
        private readonly int _vao;

        public VertexAttributes()
        {
            
            _vao = GL.GenVertexArray();
        }

        private static void Add(VertexElement el, int stride, int perInstances = -1)
        {
            switch (el.VertexElementUsage)
            {
                case VertexElementUsage.Color:
                    
                    GL.EnableVertexAttribArray((int)el.VertexElementUsage);
                    GL.VertexAttribPointer((int)el.VertexElementUsage, el.ByteCount / el.GetGlVertexDataTypeSize(), el.GetGlVertexDataType(), el.IsNormalized, stride, new IntPtr(el.Offset));
                    break;
                case VertexElementUsage.Position:
                    GL.EnableVertexAttribArray((int)el.VertexElementUsage);
                    GL.VertexAttribPointer((int)el.VertexElementUsage, el.ByteCount / el.GetGlVertexDataTypeSize(), el.GetGlVertexDataType(), el.IsNormalized, stride, new IntPtr(el.Offset));
                    break;
                case VertexElementUsage.Normal:
                    GL.EnableVertexAttribArray((int)el.VertexElementUsage);
                    GL.VertexAttribPointer((int)el.VertexElementUsage, el.ByteCount / el.GetGlVertexDataTypeSize(), el.GetGlVertexDataType(), el.IsNormalized, stride, new IntPtr(el.Offset));
                    break;
                case VertexElementUsage.TextureCoordinate:
                    GL.EnableVertexAttribArray((int)el.VertexElementUsage);
                    GL.VertexAttribPointer((int)el.VertexElementUsage, el.ByteCount / el.GetGlVertexDataTypeSize(), el.GetGlVertexDataType(), el.IsNormalized, stride, new IntPtr(el.Offset));
                    break;
                default:
                    GL.VertexAttribPointer((int)el.VertexElementUsage, el.ByteCount / el.GetGlVertexDataTypeSize(), el.GetGlVertexDataType(), el.IsNormalized, stride, new IntPtr(el.Offset));
                    GL.EnableVertexAttribArray(el.UsageIndex);//TODO:Is this the Intended usage?
                       
                    break;
            }
            if (perInstances > 0)
                GL.VertexAttribDivisor(el.UsageIndex, perInstances);
        }

        public void Bind()
        {
            GL.BindVertexArray(_vao);
        }

        public static void ApplyAttributes(VertexAttributes attribs, VertexDeclaration declaration)
        {
            attribs.Bind();
            foreach (var el in declaration.VertexElements)
            {
                Add(el, declaration.VertexStride,declaration.InstanceDivisor);
            }
            GL.BindVertexArray(0);
        }

        public bool IsDisposed{ get; private set; }

        private static void DeleteVertexArray(object that)
        {
            var va = (VertexAttributes) that;
            GL.DeleteVertexArray(va._vao);
        }
        public void Dispose()
        {
            if (!IsDisposed)
                ThreadingHelper.OnUiThread(DeleteVertexArray,this);
            IsDisposed = true;
        }
    }
}

