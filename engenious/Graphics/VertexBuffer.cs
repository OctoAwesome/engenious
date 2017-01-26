using System;
using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;

namespace engenious.Graphics
{
    public class VertexBuffer : GraphicsResource
    {
        internal int vbo = -1;
        internal int tempVBO = -1;
        internal VertexAttributes vao = null;

        private VertexBuffer(GraphicsDevice graphicsDevice, int vertexCount, BufferUsageHint usage = BufferUsageHint.StaticDraw)
            : base(graphicsDevice)
        {

            this.VertexCount = vertexCount;
            this.BufferUsage = usage;
        }

        public VertexBuffer(GraphicsDevice graphicsDevice, Type vertexType, int vertexCount, BufferUsageHint usage = BufferUsageHint.StaticDraw)
            : this(graphicsDevice, vertexCount, usage)
        {
            IVertexType tp = Activator.CreateInstance(vertexType) as IVertexType;
            if (tp == null)
                throw new ArgumentException("must be a vertexType");
			
            this.VertexDeclaration = tp.VertexDeclaration;
            using (Execute.OnUiThread)
            {
                vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.BufferData(
                    BufferTarget.ArrayBuffer,
                    new IntPtr(vertexCount * VertexDeclaration.VertexStride),
                    IntPtr.Zero,
                    (OpenTK.Graphics.OpenGL4.BufferUsageHint) BufferUsage);
            }
            ThreadingHelper.BlockOnUIThread(() =>
                {
                    vao = new VertexAttributes();
                    vao.vbo = vbo;
                    vao.Bind();
                    GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                    VertexAttributes.ApplyAttributes(vao, VertexDeclaration);

                    GL.BindVertexArray(0);
                }, true);
            GraphicsDevice.CheckError();
        }

        internal bool Bind()
        {
            if (vao == null)
                return false;
            vao.Bind();
            GraphicsDevice.CheckError();
            return true;
        }

        public VertexBuffer(GraphicsDevice graphicsDevice, VertexDeclaration vertexDeclaration, int vertexCount, BufferUsageHint usage = BufferUsageHint.StaticDraw)
            : this(graphicsDevice, vertexCount, usage)
        {
            this.VertexDeclaration = vertexDeclaration;
            using (Execute.OnUiThread)
            {
                vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(vertexCount * VertexDeclaration.VertexStride), IntPtr.Zero, (OpenTK.Graphics.OpenGL4.BufferUsageHint)BufferUsage);
            }
            ThreadingHelper.BlockOnUIThread(() =>
                {
                    vao = new VertexAttributes();
                    vao.vbo = vbo;
                    vao.Bind();
                    GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                    VertexAttributes.ApplyAttributes(vao, VertexDeclaration);
                    GL.BindVertexArray(0);

                }, true);
            GraphicsDevice.CheckError();
        }

        public void Resize(int vertexCount, bool keepData = false)
        {
            
            int tempVBO=0;
            using (Execute.OnUiThread)
            {
                GL.BindVertexArray(0);
                tempVBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, tempVBO);
                GL.BufferData(
                    BufferTarget.ArrayBuffer,
                    new IntPtr(vertexCount * VertexDeclaration.VertexStride),
                    IntPtr.Zero,
                    (OpenTK.Graphics.OpenGL4.BufferUsageHint) BufferUsage);
                GraphicsDevice.CheckError();
            }

            using (Execute.OnUiThread)
            {
                this.VertexCount = vertexCount;
                GL.DeleteBuffer(vbo);
                vbo = tempVBO;
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

        internal void EnsureVAO()
        {
            if (vao != null && vao.vbo != vbo)
            {
                vao.vbo = vbo;
                vao.Bind();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                VertexAttributes.ApplyAttributes(vao, VertexDeclaration);

                GL.BindVertexArray(0);
            }
        }

        public BufferUsageHint BufferUsage{ get; private set; }

        public int VertexCount{ get; private set; }

        public VertexDeclaration VertexDeclaration{ get; private set; }

        public void SetData<T>(T[] data) where T:struct
        {
            using (Execute.OnUiThread)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

                GCHandle buffer = GCHandle.Alloc(data, GCHandleType.Pinned);
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, new IntPtr(data.Length * VertexDeclaration.VertexStride), buffer.AddrOfPinnedObject());
                    //TODO use bufferusage
                buffer.Free();
            }
            GraphicsDevice.CheckError();
        }

        public void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct
        {
            using (Execute.OnUiThread)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

                GCHandle buffer = GCHandle.Alloc(data, GCHandleType.Pinned);
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
            using (Execute.OnUiThread)
            {
                //vao.Bind();//TODO: verify
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

                GCHandle buffer = GCHandle.Alloc(data, GCHandleType.Pinned);
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
            using (Execute.OnUiThread)
            {
                    GL.DeleteBuffer(vbo);
                }
            vao.Dispose();
            base.Dispose();
        }
    }
}

