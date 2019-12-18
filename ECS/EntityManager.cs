using System;
using System.Runtime.CompilerServices;

namespace Fabricor.ECS
{
    public class EntityManager
    {
        private EntityHeap heap;

        public EntityManager(long startSize = 104857600)//100mb
        {
            heap = new EntityHeap(startSize);
        }

        private static readonly Object lockobj = new Object();
        private static ulong NextID = 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetNewEntityID()
        {
            return GetNewEntityID(1)[0];
        }

        public static ulong[] GetNewEntityID(int count)
        {
            lock (lockobj)
            {
                ulong[] ids=new ulong[count];
                for (int i = 0; i < count; i++)
                {
                    ids[i]=NextID++;
                }
                return ids;
            }
        }
    }
}