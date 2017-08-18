using System;
using System.Runtime.InteropServices;
using engenious.Helper;
using OpenTK.Graphics.OpenGL4;

namespace engenious.Graphics
{

    public class IndexBuffer :GraphicsResource
    {
        private int _ibo;
        //private byte[] buffer;
        private readonly int _elementSize;
        public IndexBuffer(GraphicsDevice graphicsDevice, Type indexType, int indexCount, BufferUsageHint usage = BufferUsageHint.StaticDraw)
            : base(graphicsDevice)
        {
            _elementSize = Marshal.SizeOf(indexType);
            if (_elementSize <= 8)
                IndexElementSize = DrawElementsType.UnsignedByte;
            else if (_elementSize <= 16)
                IndexElementSize = DrawElementsType.UnsignedShort;
            else if (_elementSize <= 32)
                IndexElementSize = DrawElementsType.UnsignedInt;
            else
                throw new ArgumentException("Invalid Type(bigger than 32 bits)");
			
            IndexCount = indexCount;
            BufferUsage = usage;
            using (Execute.OnUiContext)
            {
                _ibo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ibo);
                GL.BufferData(
                    BufferTarget.ElementArrayBuffer,
                    new IntPtr(indexCount * _elementSize),
                    IntPtr.Zero,
                    (OpenTK.Graphics.OpenGL4.BufferUsageHint) BufferUsage);
            }
            GraphicsDevice.CheckError();
            //buffer = new byte[indexCount * (int)IndexElementSize / 8];
        }

        public IndexBuffer(GraphicsDevice graphicsDevice, DrawElementsType indexElementSize, int indexCount, BufferUsageHint usage = BufferUsageHint.StaticDraw)
            : base(graphicsDevice)
        {
            IndexElementSize = indexElementSize;
            IndexCount = indexCount;
            BufferUsage = usage;
            using (Execute.OnUiContext)
            {
                _ibo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ibo);
                _elementSize = (indexElementSize == DrawElementsType.UnsignedByte ? 1 : (indexElementSize == DrawElementsType.UnsignedShort ? 2 : 4));
                GL.BufferData(
                    BufferTarget.ElementArrayBuffer,
                    new IntPtr(indexCount * _elementSize),
                    IntPtr.Zero,
                    (OpenTK.Graphics.OpenGL4.BufferUsageHint) BufferUsage);
            }
            GraphicsDevice.CheckError();
            //buffer = new byte[indexCount * (int)IndexElementSize / 8];
        }

        public BufferUsageHint BufferUsage{ get; private set; }

        public int IndexCount{ get; private set; }

        public DrawElementsType IndexElementSize{ get; private set; }


        public void SetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount) where T : struct //TODO: valuetype?
        {
            if (elementCount == 0 || data.Length == 0)
                return;



            var elSize = Marshal.SizeOf(typeof(T));

            var buffer = GCHandle.Alloc(data, GCHandleType.Pinned);
            using (Execute.OnUiContext)
            {
                Bind();
                GL.BufferSubData(
                    BufferTarget.ElementArrayBuffer,
                    new IntPtr(offsetInBytes),
                    new IntPtr(elementCount * elSize),
                    buffer.AddrOfPinnedObject() + startIndex * elSize); //TODO:
            }
            buffer.Free();
            GraphicsDevice.CheckError();
            //Buffer.BlockCopy (data, startIndex, buffer, offsetInBytes, elementCount);
        }

        public void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct
        {
            if (elementCount == 0 || data.Length == 0)
                return;


            var elSize = Marshal.SizeOf(typeof(T));

            var buffer = GCHandle.Alloc(data, GCHandleType.Pinned);
            using (Execute.OnUiContext)
            {
                Bind();

                GL.BufferSubData(
                    BufferTarget.ElementArrayBuffer,
                    IntPtr.Zero,
                    new IntPtr(elementCount * elSize),
                    buffer.AddrOfPinnedObject() + startIndex * elSize); //TODO:
            }
            buffer.Free();
            GraphicsDevice.CheckError();
            //GL.BufferData<T> (BufferTarget.ElementArrayBuffer, new IntPtr (elementCount * Marshal.SizeOf(T)), data[startIndex], BufferUsageHint.StaticDraw);//TODO:
            //Buffer.BlockCopy (data, startIndex, buffer, 0, elementCount);
        }

        public void SetData<T>(T[] data) where T : struct
        {
            if (data.Length == 0)
                return;

            using (Execute.OnUiContext)
            {
                Bind();
                GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, new IntPtr(data.Length * Marshal.SizeOf(default(T))), data); //TODO:
            }
            GraphicsDevice.CheckError();
            //Buffer.BlockCopy (data, 0, buffer, 0, data.Length);
        }
        public void Resize(int indexCount, bool keepData = false)
        {
            
            int tempIBO;
            using (Execute.OnUiContext)
            {
                tempIBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, tempIBO);
                GL.BufferData(
                    BufferTarget.ElementArrayBuffer,
                    new IntPtr(indexCount * _elementSize),
                    IntPtr.Zero,
                    (OpenTK.Graphics.OpenGL4.BufferUsageHint) BufferUsage);


                GL.DeleteBuffer(_ibo);
                _ibo = tempIBO;

                IndexCount = indexCount;
            }

            if (keepData)
            {
                //TODO:
                throw new NotImplementedException();
            }

        }


        internal void Bind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ibo);
            GraphicsDevice.CheckError();
        }

        public override void Dispose()
        {
            using (Execute.OnUiContext)
            {
                GL.DeleteBuffer(_ibo);
            }
            base.Dispose();
        }
    }
}

