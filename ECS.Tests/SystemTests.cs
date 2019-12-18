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

            RandomSystem random = new RandomSystem();
            PrintOutSystem<FloatComponent> print = new PrintOutSystem<FloatComponent>();
            PrintOutSystem<IntComponent> intprint = new PrintOutSystem<IntComponent>();
            Thread.Sleep(5000);
            Stopwatch stopwatch = Stopwatch.StartNew();
            heap.ExecuteSystem(random, workloads);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            //heap.ExecuteSystem(print,workloads,false);
            //heap.ExecuteSystem(intprint,workloads);

            heap.Free();
        }
    }

    public unsafe class PrintOutSystem<T> : ECSSystem<T> where T : unmanaged
    {
        public sealed override void Kernel(T* component)
        {
            Console.WriteLine((*component).ToString());
        }
    }

    public unsafe class RandomSystem : ECSSystem<FloatComponent>
    {
        public sealed override void Kernel(FloatComponent* component)
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