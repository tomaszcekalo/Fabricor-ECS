using System.Runtime.CompilerServices;
using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace ECS.Benchmark
{
    public unsafe class Program
    {
        static void Main(string[] args)
        {
            var result=BenchmarkRunner.Run<Program>();
        }

        [Benchmark]
        public void Virt()
        {
            IStruct s = new Struc();

            for (int i = 0; i < 1000; i++)
            {
                s.Method(&i);
            }
        }

        [Benchmark]
        public void Call()
        {
            Struc s = new Struc();

            for (int i = 0; i < 1000; i++)
            {
                s.Method(&i);
            }
        }

        [Benchmark]
        public void Virt2()
        {
            Struc s = new Struc();
            Virt2Other(s);
        }

        private void Virt2Other<T>(T s) where T : struct, IStruct
        {
            for (int i = 0; i < 1000; i++)
            {
                s.Method(&i);
            }
        }

        [Benchmark]
        public void Del()
        {
            Action<IntPtr> action = (ptr) => { int* i = (int*)ptr.ToPointer(); };

            for (int i = 0; i < 1000; i++)
            {
                action.Invoke(new IntPtr(&i));
            }
        }

        [Benchmark]
        public void Del2()
        {
            Intdel del = (ptr) => { };

            for (int i = 0; i < 1000; i++)
            {
                del(&i);
            }
        }

        public delegate void Intdel(int* ptr);
    }

    public unsafe interface IStruct
    {
        void Method(int* ptr);
    }

    public unsafe struct Struc : IStruct
    {
        public void Method(int* ptr)
        {

        }
    }
}
