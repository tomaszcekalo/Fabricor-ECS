using System.Runtime.InteropServices;
using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace Fabricor.ECS
{
    public unsafe class EntityHeap
    {
        public readonly byte* heapStart;
        public byte* heapIndex;

        public readonly List<EntityMetadata> EntityMetadata = new List<EntityMetadata>();
        private object HeapIndexLock = new object();

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

        public SystemWorkload[] GetSystemWorkloads(int count)
        {
            byte* maxIndex;
            lock (HeapIndexLock)
            {
                maxIndex = heapIndex;
            }
            int steps;
            SystemWorkload[] workloads = new SystemWorkload[count];
            lock (EntityMetadata)
            {
                steps = (int)Math.Floor(((double)EntityMetadata.Count) / count);
                for (int i = 0; i < count; i++)
                {
                    workloads[i].startAddress = EntityMetadata[i * steps].heapAddress;
                }
            }
            for (int i = 0; i < count - 1; i++)
            {
                workloads[i].endAddress = workloads[i + 1].startAddress;
            }
            workloads[count-1].endAddress=maxIndex-heapStart;
            return workloads;
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
            lock (HeapIndexLock)
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
                    addresses[j] = address;
                }
                EntityMetadata[] meta = new EntityMetadata[count];
                ulong[] ids = EntityManager.GetNewEntityID(count);
                for (int i = 0; i < count; i++)
                {
                    meta[i] = new EntityMetadata(ids[i], components);
                    meta[i].heapAddress = addresses[i];
                }
                EntityMetadata.AddRange(meta);
                return meta;
            }
        }

        public void Free()
        {
            Marshal.FreeHGlobal((IntPtr)heapStart);
        }
    }
}