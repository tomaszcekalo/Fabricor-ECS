using System;
using NUnit.Framework;
using Fabricor.ECS;

namespace ECS.Tests
{
    public class HeapTests{

        [Test]
        [TestCase(1024L*1024)]
        public void CreationAndDeletion(long size){
            EntityHeap heap=new EntityHeap(size);

            heap.Free();
        }

        [Test]
        public void Appending(){
            EntityHeap heap=new EntityHeap(32);

            heap.AppendEntities(new Type[0],4);
            Assert.AreEqual(8,heap.HeapLength,0.1);

            heap.Free();
        }

        
    }
}