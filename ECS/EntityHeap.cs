using System.Runtime.InteropServices;
using System;

namespace Fabricor.ECS
{
    public unsafe class EntityHeap{
        public readonly byte* heapStart;
        
        public EntityHeap(long size){
            heapStart=(byte*)Marshal.AllocHGlobal(new IntPtr(size));
        }

        public void Free(){
            Marshal.FreeHGlobal((IntPtr)heapStart);
        }
    }
}