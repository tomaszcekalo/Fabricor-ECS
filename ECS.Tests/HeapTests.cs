using System;
using NUnit.Framework;
using Fabricor.ECS;

namespace ECS.Tests
{
    public class HeapTests{

        [Test]
        [TestCase(1024L*1024*1024)]
        public void CreationAndDeletion(long size){
            EntityHeap heap=new EntityHeap(size);

            heap.Free();
        }
    }
}