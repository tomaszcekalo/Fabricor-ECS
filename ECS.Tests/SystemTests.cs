using System.Threading.Tasks;
using System.Threading;
using System;
using System.Diagnostics;
using Fabricor.ECS;
using NUnit.Framework;

namespace ECS.Tests
{
    public class SystemTests
    {
        [Test]
        public void SingleThreadSystem()
        {
            EntityHeap heap = new EntityHeap(1024L * 1024L);
            heap.AppendEntities(new Type[] { typeof(FloatComponent), typeof(IntComponent) }, 100);
            SystemWorkload[] workloads = heap.GetSystemWorkloads(6);

            RandomFloatKernel random = new RandomFloatKernel();
            PrintOutKernel<FloatComponent> print = new PrintOutKernel<FloatComponent>();
            PrintOutKernel<IntComponent> intprint = new PrintOutKernel<IntComponent>();

            Task t1 = heap.ExecuteSystem(new ECSLinearStepSystem<FloatComponent, RandomFloatKernel>(random), workloads);
            Task t2 = heap.ExecuteSystem(new ECSLinearStepSystem<FloatComponent, PrintOutKernel<FloatComponent>>(print), workloads, false);
            Task t3 = heap.ExecuteSystem(new ECSLinearStepSystem<IntComponent, PrintOutKernel<IntComponent>>(intprint), workloads, false);

            if (!Task.WaitAll(new Task[] { t1, t2, t3 }, 10000))
            {
                throw new TimeoutException("The kernels did not complete within the timout.");
            }

            heap.Free();
        }
    }

    public unsafe struct PrintOutKernel<T> : ILinearStepKernel<T> where T : unmanaged
    {
        public void Kernel(T* component)
        {
            Console.WriteLine((*component).ToString());
        }
    }

    public unsafe struct RandomFloatKernel : ILinearStepKernel<FloatComponent>
    {
        public void Kernel(FloatComponent* component)
        {
            FloatComponent c = (*component);
            c.f = (float)new Random().NextDouble();
            *component = c;
        }
    }

    public struct FloatComponent
    {
        private ComponentHeader header;
        public float f;

        public override string ToString()
        {
            return $"{f}";
        }
    }

    public struct IntComponent
    {
        private ComponentHeader header;
        public float i;

        public override string ToString()
        {
            return $"{i}";
        }
    }
}