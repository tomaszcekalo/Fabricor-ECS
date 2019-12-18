using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Fabricor.ECS
{
    public unsafe abstract class ECSSystem<T> : ECSSystem where T : unmanaged
    {

        public override void Execute(byte* start, byte* end)
        {
            byte* head = start;
            uint componentType=ComponentType.GetUID(typeof(T));

            int switchCase=0;
            while (head < end)
            {
                ushort entitySize=0;
                byte* entityStart=head;
                switchCase=0;
                switch(switchCase){
                    case 0:
                        entitySize= *((ushort*)head);
                        head += sizeof(ushort);
                        goto case 1;
                    case 1:
                        if(head-entityStart>entitySize){
                            break;
                        }
                        Component c=*((Component*)head);
                        if(c.componentID!=componentType){
                            head+=c.size;
                            goto case 1;
                        }
                        Kernel((T*)head);
                        head+=c.size;
                        goto case 1;
                }
                 
                
            }
        }

        /*
        For the love of god, please seal this method. If you don't seal it, it can't be inlined.
        */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void Kernel(T* component);
    }

    public unsafe abstract class ECSSystem
    {
        public abstract void Execute(byte* start, byte* end);
    }

    public struct SystemWorkload
    {
        public long startAddress;
        public long endAddress;
    }
}