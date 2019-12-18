using System.Runtime.InteropServices;
using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fabricor.ECS
{
    public unsafe class EntityHeap
    {
        public readonly byte* heapStart;
        public byte* heapIndex;

        public readonly List<EntityMetadata> EntityMetadata = new List<EntityMetadata>();
        private object HeapIndexLock = new object();

        public long HeapLength { get { return heapIndex - heapStart; } }

        public readonly long MaxSize;

        public EntityHeap(long size)
        {
            MaxSize = size;
            heapStart = (byte*)Marshal.AllocHGlobal(new IntPtr(size));
            byte* ptr = heapStart;
            heapIndex = heapStart;
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
            workloads[count - 1].endAddress = maxIndex - heapStart;
            return workloads;
        }

        public Task ExecuteSystem(ECSSystem system, SystemWorkload[] workloads, bool shouldBlock = true)
        {
            Task[] tasks = new Task[workloads.Length];
            for (int i = 0; i < workloads.Length; i++)
            {
                int index = i;
                tasks[i] = Task.Run(() =>
                {
                    system.Execute(heapStart + workloads[index].startAddress, heapStart + workloads[index].endAddress);
                });
            }
            if (!shouldBlock)
            {
                return Task.Run(() =>
                {
                    Task.WaitAll(tasks);
                });
            }
            Task.WaitAll(tasks);
            return Task.Run(() => { });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        public EntityMetadata[] AppendEntities(Type[] components, int count)
        {
            lock (HeapIndexLock)
            {

                ushort size = 0;
                uint[] componentIDs=new uint[components.Length];
                for (int i = 0; i < components.Length; i++)
                {
                    size += (ushort)components[i].MarshalSize();
                    componentIDs[i]=ComponentType.GetUID(components[i]);
                }
                int totalPerEntity = size + 2;
                long totalBytes=((long)totalPerEntity) * count;
                long availableBytes=MaxSize - HeapLength;
                if (totalBytes > availableBytes)
                    throw new OutOfMemoryException($"Can't append more Entities to that heap because there is not enough memory. {(totalBytes-availableBytes)/1024/1024}mb needed.");
                
                EntityMetadata[] meta = new EntityMetadata[count];
                ulong[] ids = EntityManager.GetNewEntityID(count);

                Parallel.For(0,count,(j)=>
                {
                    long address = HeapLength + j * totalPerEntity;
                    byte* localHead = heapIndex+ j * totalPerEntity;

                    *((ushort*)localHead) = size;
                    localHead += sizeof(ushort);
                    for (int i = 0; i < components.Length; i++)
                    {
                        Component c = new Component { componentID = componentIDs[i], size = (ushort)components[i].MarshalSize() };
                        *((Component*)localHead) = c;
                        localHead+=c.size;
                    }

                    meta[j] = new EntityMetadata(ids[j], components);
                    meta[j].heapAddress = address;
                });
                heapIndex += totalPerEntity * count;
                lock(EntityMetadata)
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