using System;

using OpenTK.Graphics.OpenGL4;

namespace engenious.Graphics
{
    internal class VertexAttributes:IDisposable
    {
        internal int vbo;
        private int vao;

        public VertexAttributes()
        {
            
            vao = GL.GenVertexArray();
        }

        private void Add(VertexElement el, int stride, int perInstances = -1)
        {
            switch (el.VertexElementUsage)
            {
                case VertexElementUsage.Color:
                    
                    GL.EnableVertexAttribArray((int)el.VertexElementUsage);
                    GL.VertexAttribPointer((int)el.VertexElementUsage, el.ByteCount / el.GetGlVertexDataTypeSize(), el.GetGLVertexDataType(), el.IsNormalized, stride, new IntPtr(el.Offset));
                    break;
                case VertexElementUsage.Position:
                    GL.EnableVertexAttribArray((int)el.VertexElementUsage);
                    GL.VertexAttribPointer((int)el.VertexElementUsage, el.ByteCount / el.GetGlVertexDataTypeSize(), el.GetGLVertexDataType(), el.IsNormalized, stride, new IntPtr(el.Offset));
                    break;
                case VertexElementUsage.Normal:
                    GL.EnableVertexAttribArray((int)el.VertexElementUsage);
                    GL.VertexAttribPointer((int)el.VertexElementUsage, el.ByteCount / el.GetGlVertexDataTypeSize(), el.GetGLVertexDataType(), el.IsNormalized, stride, new IntPtr(el.Offset));
                    break;
                case VertexElementUsage.TextureCoordinate:
                    GL.EnableVertexAttribArray((int)el.VertexElementUsage);
                    GL.VertexAttribPointer((int)el.VertexElementUsage, el.ByteCount / el.GetGlVertexDataTypeSize(), el.GetGLVertexDataType(), el.IsNormalized, stride, new IntPtr(el.Offset));
                    break;
                default:
                    GL.VertexAttribPointer((int)el.VertexElementUsage, el.ByteCount / el.GetGlVertexDataTypeSize(), el.GetGLVertexDataType(), el.IsNormalized, stride, new IntPtr(el.Offset));
                    GL.EnableVertexAttribArray(el.UsageIndex);//TODO:Is this the Intended usage?
                       
                    break;
            }
            if (perInstances > 0)
                GL.VertexAttribDivisor(el.UsageIndex, perInstances);
        }

        public void Bind()
        {
            GL.BindVertexArray(vao);
        }

        public static void ApplyAttributes(VertexAttributes attribs, VertexDeclaration declaration)
        {
            attribs.Bind();
            foreach (var el in declaration.VertexElements)
            {
                attribs.Add(el, declaration.VertexStride,declaration.InstanceDivisor);
            }
            GL.BindVertexArray(0);
        }

        public bool IsDisposed{ get; private set; }

        private static void DeleteVertexArray(object that)
        {
            VertexAttributes va = (VertexAttributes) that;
            GL.DeleteVertexArray(va.vao);
        }
        public void Dispose()
        {
            if (!IsDisposed)
                ThreadingHelper.OnUiThread(DeleteVertexArray,this);
            IsDisposed = true;
        }
    }
}

