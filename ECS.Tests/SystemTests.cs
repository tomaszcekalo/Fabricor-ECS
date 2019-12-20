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
            EntityHeap heap = new EntityHeap(1024L * 1024L * 1024L*8L);
            for (int i = 0; i < 8; i++)
                heap.AppendEntities(new Type[] { typeof(FloatComponent), typeof(IntComponent) }, 10000000);
            SystemWorkload[] workloads = heap.GetSystemWorkloads(6);

            RandomFloatKernel random = new RandomFloatKernel();
            PrintOutKernel<FloatComponent> print = new PrintOutKernel<FloatComponent>();
            PrintOutKernel<IntComponent> intprint = new PrintOutKernel<IntComponent>();
            Thread.Sleep(5000);
            Stopwatch stopwatch = Stopwatch.StartNew();
            heap.ExecuteSystem(new ECSSystem<FloatComponent,RandomFloatKernel>(random), workloads);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            //heap.ExecuteSystem(print,workloads,false);
            //heap.ExecuteSystem(intprint,workloads);

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
            c.f = MathF.Sin(0.45f);
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