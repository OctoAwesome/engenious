using System;
using System.Reflection.Emit;

namespace engenious.Helper
{
    public class MemoryHelper
    {
        static MemoryHelper()
        {
            CopyBulk = CompileBulkCopy();
        }
        private static CopyBulkDelegate CompileBulkCopy() {
            var dynamicMethod = new DynamicMethod(
                "copybulk",
                null,
                new[] { typeof(IntPtr),typeof( IntPtr),typeof(uint) });

            var gen = dynamicMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Ldarg_2);
            gen.Emit(OpCodes.Cpblk);
            gen.Emit(OpCodes.Ret);
            return (CopyBulkDelegate)dynamicMethod.CreateDelegate(typeof(CopyBulkDelegate));
        }
        public delegate void CopyBulkDelegate(IntPtr src, IntPtr dst, uint size);
        public static CopyBulkDelegate CopyBulk;
    }
}

