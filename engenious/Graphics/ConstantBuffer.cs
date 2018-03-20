using System;
using OpenTK.Graphics.OpenGL;
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
                GL.BufferData(BufferTarget.UniformBuffer, new IntPtr(size), IntPtr.Zero, OpenTK.Graphics.OpenGL.BufferUsageHint.DynamicDraw);
            }
        }

        public unsafe void Update(IntPtr data, uint size)
        {
            using (Execute.OnUiContext)
            {
                GL.BindBuffer(BufferTarget.UniformBuffer, Ubo);
                var ptr = GL.MapBuffer(BufferTarget.UniformBuffer, BufferAccess.WriteOnly);

                System.Runtime.CompilerServices.Unsafe.CopyBlock((void*) ptr, (void*) data, size);
                //Buffer.BlockCopy(
                GL.UnmapBuffer(BufferTarget.UniformBuffer);
            }
        }

        public unsafe void Update<T>(T data) where T : struct
        {
            using (Execute.OnUiContext)
            {
                GL.BindBuffer(BufferTarget.UniformBuffer, Ubo);
                var ptr = GL.MapBuffer(BufferTarget.UniformBuffer, BufferAccess.WriteOnly);

                System.Runtime.CompilerServices.Unsafe.Write((void*) ptr, data);

                GL.UnmapBuffer(BufferTarget.UniformBuffer);
            }
        }

        public unsafe void Update<T>(T[] data) where T : struct
        {
            using (Execute.OnUiContext)
            {
                GL.BindBuffer(BufferTarget.UniformBuffer, Ubo);
                var ptr = GL.MapBuffer(BufferTarget.UniformBuffer, BufferAccess.WriteOnly);

                System.Runtime.CompilerServices.Unsafe.Write((void*) ptr, data);

                GL.UnmapBuffer(BufferTarget.UniformBuffer);
            }
        }
    }
}

