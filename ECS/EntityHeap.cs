using System.Runtime.InteropServices;
using System;
using System.Runtime.CompilerServices;

namespace Fabricor.ECS
{
    public unsafe class EntityHeap
    {
        public readonly byte* heapStart;
        public byte* heapIndex;

        public long HeapLength { get { return heapIndex - heapStart; } }

        public EntityHeap(long size)
        {
            heapStart = (byte*)Marshal.AllocHGlobal(new IntPtr(size));
            byte* ptr = heapStart;
            heapIndex = heapStart;
            for (int i = 0; i < size; i++)
            {
                *ptr = 0;
                ptr++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Append<T>(T component, int size) where T : unmanaged
        {
            T* ptr = (T*)heapIndex;
            *ptr = component;
            heapIndex += size;
        }

        public EntityMetadata[] AppendEntities(Type[] components, int count)
        {
            long[] addresses = new long[count];
            for (int j = 0; j < count; j++)
            {
                long address = HeapLength;
                ushort size = 0;
                for (int i = 0; i < components.Length; i++)
                {
                    size += (ushort)components[i].MarshalSize();
                }
                *((ushort*)heapIndex) = size;
                heapIndex += sizeof(ushort);
                for (int i = 0; i < components.Length; i++)
                {
                    Component c = new Component { componentID = ComponentType.GetUID(components[i]), size = (ushort)components[i].MarshalSize() };
                    Append(c, components[i].MarshalSize());
                }
                addresses[j]=address;
            }
            EntityMetadata[] meta = new EntityMetadata[count];
            ulong[] ids=EntityManager.GetNewEntityID(count);
            for (int i = 0; i < count; i++)
            {
                meta[i]=new EntityMetadata(ids[i], components);
                meta[i].heapAddress=addresses[i];
            }
            return meta;
        }

        public void Free()
        {
            Marshal.FreeHGlobal((IntPtr)heapStart);
        }
    }
}