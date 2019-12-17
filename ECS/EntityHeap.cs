using System.Runtime.InteropServices;
using System;

namespace Fabricor.ECS
{
    public unsafe class EntityHeap{
        public readonly byte* heapStart;
        
        public EntityHeap(long size){
            heapStart=(byte*)Marshal.AllocHGlobal(new IntPtr(size));
            byte* ptr=heapStart;
            for (int i = 0; i < size; i++)
            {
                *ptr=0;
                ptr++;
            }
        }

        public void Free(){
            Marshal.FreeHGlobal((IntPtr)heapStart);
        }
    }
}