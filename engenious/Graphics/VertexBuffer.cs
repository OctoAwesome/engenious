using System;
using System.Runtime.InteropServices;
using engenious.Helper;
using OpenTK.Graphics.OpenGL;

namespace engenious.Graphics
{
    public class VertexBuffer : GraphicsResource
    {
        internal int Vbo;
        internal int TempVbo = -1;
        internal VertexAttributes Vao;

        private VertexBuffer(GraphicsDevice graphicsDevice, int vertexCount, BufferUsageHint usage = BufferUsageHint.StaticDraw)
            : base(graphicsDevice)
        {

            VertexCount = vertexCount;
            BufferUsage = usage;
        }

        private static void ExchangeVao(object that)
        {
            var vb = (VertexBuffer) that;
            vb.Vao = new VertexAttributes();
            vb.Vao.Vbo = vb.Vbo;
            vb.Vao.Bind();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vb.Vbo);
            VertexAttributes.ApplyAttributes(vb.Vao, vb.VertexDeclaration);

            GL.BindVertexArray(0);
        }
        public VertexBuffer(GraphicsDevice graphicsDevice, Type vertexType, int vertexCount, BufferUsageHint usage = BufferUsageHint.StaticDraw)
            : this(graphicsDevice, vertexCount, usage)
        {
            var tp = Activator.CreateInstance(vertexType) as IVertexType;
            if (tp == null)
                throw new ArgumentException("must be a vertexType");
			
            VertexDeclaration = tp.VertexDeclaration;
            using (Execute.OnUiContext)
            {
                Vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);
                GL.BufferData(
                    BufferTarget.ArrayBuffer,
                    new IntPtr(vertexCount * VertexDeclaration.VertexStride),
                    IntPtr.Zero,
                    (OpenTK.Graphics.OpenGL.BufferUsageHint) BufferUsage);
            }
            ThreadingHelper.OnUiThread(ExchangeVao,this);
            GraphicsDevice.CheckError();
        }

        internal bool Bind()
        {
            if (Vao == null)
                return false;
            Vao.Bind();
            GraphicsDevice.CheckError();
            return true;
        }

        public VertexBuffer(GraphicsDevice graphicsDevice, VertexDeclaration vertexDeclaration, int vertexCount, BufferUsageHint usage = BufferUsageHint.StaticDraw)
            : this(graphicsDevice, vertexCount, usage)
        {
            VertexDeclaration = vertexDeclaration;
            using (Execute.OnUiContext)
            {
                Vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(vertexCount * VertexDeclaration.VertexStride), IntPtr.Zero, (OpenTK.Graphics.OpenGL.BufferUsageHint)BufferUsage);
            }
            ThreadingHelper.OnUiThread(ExchangeVao,this);
            GraphicsDevice.CheckError();
        }

        public void Resize(int vertexCount, bool keepData = false)
        {
            using (Execute.OnUiContext)
            {
                GL.BindVertexArray(0);
                var tempVbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, tempVbo);
                GL.BufferData(
                    BufferTarget.ArrayBuffer,
                    new IntPtr(vertexCount * VertexDeclaration.VertexStride),
                    IntPtr.Zero,
                    (OpenTK.Graphics.OpenGL.BufferUsageHint) BufferUsage);
                GraphicsDevice.CheckError();

                VertexCount = vertexCount;
                GL.DeleteBuffer(Vbo);
                Vbo = tempVbo;
                GraphicsDevice.CheckError();
            }
            //ThreadingHelper.BlockOnUIThread(() =>
             //   {

               // }, true);*/
            if (keepData)
            {
                //TODO:
                throw new NotImplementedException();
            }

        }

        internal void EnsureVao()
        {
            if (Vao != null && Vao.Vbo != Vbo)
            {
                Vao.Vbo = Vbo;
                Vao.Bind();
                GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);
                VertexAttributes.ApplyAttributes(Vao, VertexDeclaration);

                GL.BindVertexArray(0);
            }
        }

        public BufferUsageHint BufferUsage{ get; private set; }

        public int VertexCount{ get; private set; }

        public VertexDeclaration VertexDeclaration{ get; private set; }

        public void SetData<T>(T[] data) where T:struct
        {
            using (Execute.OnUiContext)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);

                var buffer = GCHandle.Alloc(data, GCHandleType.Pinned);
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, new IntPtr(data.Length * VertexDeclaration.VertexStride), buffer.AddrOfPinnedObject());
                    //TODO use bufferusage
                buffer.Free();
            }
            GraphicsDevice.CheckError();
        }

        public void SetData(IntPtr ptr, int sizeInBytes)
        {
            SetData(ptr, 0, sizeInBytes);
        }
        public void SetData(IntPtr ptr,int offsetInBytes,int sizeInBytes)
        {
            using (Execute.OnUiContext)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);

                GL.BufferSubData(BufferTarget.ArrayBuffer, new IntPtr(offsetInBytes), new IntPtr(sizeInBytes), ptr);
            }
            GraphicsDevice.CheckError();
        }

        public void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct
        {
            using (Execute.OnUiContext)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);

                var buffer = GCHandle.Alloc(data, GCHandleType.Pinned);
                GL.BufferSubData(
                    BufferTarget.ArrayBuffer,
                    IntPtr.Zero,
                    new IntPtr(elementCount * VertexDeclaration.VertexStride),
                    buffer.AddrOfPinnedObject() + startIndex * VertexDeclaration.VertexStride); //TODO use bufferusage

                buffer.Free();
            }
            GraphicsDevice.CheckError();
        }

        public void SetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount, int vertexStride) where T : struct
        {
            using (Execute.OnUiContext)
            {
                //vao.Bind();//TODO: verify
                GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);

                var buffer = GCHandle.Alloc(data, GCHandleType.Pinned);
                GL.BufferSubData(
                    BufferTarget.ArrayBuffer,
                    new IntPtr(offsetInBytes),
                    new IntPtr(elementCount * vertexStride),
                    buffer.AddrOfPinnedObject() + startIndex * vertexStride);

                buffer.Free();
            }
            GraphicsDevice.CheckError();
        }

        public override void Dispose()
        {
            using (Execute.OnUiContext)
            {
                    GL.DeleteBuffer(Vbo);
                }
            Vao.Dispose();
            base.Dispose();
        }
    }
}

