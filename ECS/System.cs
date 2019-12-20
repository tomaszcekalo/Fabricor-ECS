using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Fabricor.ECS
{
    public unsafe class ECSLinearStepSystem<T,K> : ECSSystem where T : unmanaged where K : struct,ILinearStepKernel<T>
    {
        private K kernel;

        public ECSLinearStepSystem(K kernel){
            this.kernel=kernel;
        }

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
                        kernel.Kernel((T*)head);
                        head+=c.size;
                        goto case 1;
                }
                 
                
            }
        }
    }

    public unsafe interface ILinearStepKernel<T> where T : unmanaged
    {
        void Kernel(T* component);
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