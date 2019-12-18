using System;
using NUnit.Framework;
using Fabricor.ECS;


namespace ECS.Tests
{
    public class EntityTests
    {
        [Test]
        public void EntityQuery()
        {
            EntityMetadata metadata=new EntityMetadata(10,new Type[]{typeof(TestComponentA),typeof(TestComponentB)});
            
            EntityQuery query1=new EntityQuery{components=new Type[]{typeof(TestComponentA),typeof(TestComponentB)}};
            EntityQuery query2=new EntityQuery{components=new Type[]{typeof(TestComponentA)}};
            EntityQuery query3=new EntityQuery{components=new Type[]{typeof(TestComponentB)}};
            EntityQuery query4=new EntityQuery{components=new Type[]{typeof(TestComponentB),typeof(TestComponentA)}};

            EntityQuery query5=new EntityQuery{components=new Type[]{typeof(TestComponentC),typeof(TestComponentB)}};
            EntityQuery query6=new EntityQuery{components=new Type[]{typeof(TestComponentC)}};
            EntityQuery query7=new EntityQuery{components=new Type[]{typeof(TestComponentC),typeof(TestComponentA)}};
            EntityQuery query8=new EntityQuery{components=new Type[]{typeof(TestComponentB),typeof(TestComponentC)}};

            Assert.IsTrue(metadata.FitsQuery(query1));
            Assert.IsTrue(metadata.FitsQuery(query2));
            Assert.IsTrue(metadata.FitsQuery(query3));
            Assert.IsTrue(metadata.FitsQuery(query4));

            Assert.IsFalse(metadata.FitsQuery(query5));
            Assert.IsFalse(metadata.FitsQuery(query6));
            Assert.IsFalse(metadata.FitsQuery(query7));
            Assert.IsFalse(metadata.FitsQuery(query8));
        }
    }

    struct TestComponentA{
        byte b;
    }
    struct TestComponentB{
        byte b;
    }
    struct TestComponentC{
        byte b;
    }
}