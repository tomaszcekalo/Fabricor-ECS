using System;
namespace Fabricor.ECS
{
    public struct Entity
    {
        public EntityMetadata metadata;
        public IComponent[] components;
    }

    public class EntityMetadata
    {
        public readonly long ID; 
        public Type[] components;

        public EntityMetadata(long ID,Type[] components){
            this.ID=ID;
            this.components=components;
        }
        public bool FitsQuery(EntityQuery q)
        {
            for (int i = 0; i < q.components.Length; i++)
            {
                for (int j = 0; j < components.Length; j++)
                {
                    if(q.components[i]==components[j])
                        goto next;
                }
                return false;

                next:continue;
            }
            return true;
        }
    }

    public struct EntityQuery
    {
        public Type[] components;
    }

    public interface IComponent
    {

    }
}