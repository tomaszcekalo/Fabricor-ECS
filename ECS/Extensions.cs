using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Fabricor.ECS{
    public static class ECSExtensions{
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MarshalSize(this Type t){
            return Marshal.SizeOf(t);
        }
    }
}