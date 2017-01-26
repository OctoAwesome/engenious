using System;
using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;

namespace engenious
{
    public class ConstantBuffer
    {
        internal int ubo;
        public ConstantBuffer(int size)
        {
            using (Execute.OnUiThread)
            {
                ubo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.UniformBuffer, ubo);
                GL.BufferData(BufferTarget.UniformBuffer, new IntPtr(size), IntPtr.Zero, BufferUsageHint.DynamicDraw);
            }
        }
        public void Update(IntPtr data,uint size)
        {
            using (Execute.OnUiThread)
            {
                GL.BindBuffer(BufferTarget.UniformBuffer, ubo);
                IntPtr ptr = GL.MapBuffer(BufferTarget.UniformBuffer, BufferAccess.WriteOnly);
                MemoryHelper.CopyBulk(data, ptr, size);
                //Buffer.BlockCopy(
                GL.UnmapBuffer(BufferTarget.UniformBuffer);
            }
        }
        public void Update<T>(T data) where T : struct
        {
            uint size = (uint)Marshal.SizeOf(data);
            GCHandle h = GCHandle.Alloc(data,GCHandleType.Pinned);
            Update(h.AddrOfPinnedObject());
            h.Free();
        }
        public void Update<T>(T[] data) where T : struct
        {
            uint size = (uint)(Marshal.SizeOf(typeof(T))*data.Length);
            GCHandle h = GCHandle.Alloc(data,GCHandleType.Pinned);
            Update(h.AddrOfPinnedObject(),size);
            h.Free();
        }
    }
}

