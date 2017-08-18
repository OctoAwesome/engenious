using System;
using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;
using engenious.Helper;

namespace engenious.Graphics
{
    public class ConstantBuffer
    {
        internal int Ubo;
        public ConstantBuffer(int size)
        {
            using (Execute.OnUiContext)
            {
                Ubo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.UniformBuffer, Ubo);
                GL.BufferData(BufferTarget.UniformBuffer, new IntPtr(size), IntPtr.Zero, OpenTK.Graphics.OpenGL4.BufferUsageHint.DynamicDraw);
            }
        }
        public void Update(IntPtr data,uint size)
        {
            using (Execute.OnUiContext)
            {
                GL.BindBuffer(BufferTarget.UniformBuffer, Ubo);
                var ptr = GL.MapBuffer(BufferTarget.UniformBuffer, BufferAccess.WriteOnly);
                MemoryHelper.CopyBulk(data, ptr, size);
                //Buffer.BlockCopy(
                GL.UnmapBuffer(BufferTarget.UniformBuffer);
            }
        }
        public void Update<T>(T data) where T : struct
        {
            var size = (uint)Marshal.SizeOf(data);
            var h = GCHandle.Alloc(data,GCHandleType.Pinned);
            Update(h.AddrOfPinnedObject(),size);
            h.Free();
        }
        public void Update<T>(T[] data) where T : struct
        {
            var size = (uint)(Marshal.SizeOf(typeof(T))*data.Length);
            var h = GCHandle.Alloc(data,GCHandleType.Pinned);
            Update(h.AddrOfPinnedObject(),size);
            h.Free();
        }
    }
}

