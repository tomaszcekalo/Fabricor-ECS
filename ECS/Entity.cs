using System.Runtime.InteropServices;
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
        public readonly ulong ID;
        public ulong heapAddress;
        public ComponentMetadata[] components;

        public EntityMetadata(ulong ID, Type[] components)
        {
            this.ID = ID;
            this.components = new ComponentMetadata[components.Length];
            ushort currentOffset=0;
            for (int i = 0; i < components.Length; i++)
            {
                this.components[i].type=components[i];
                this.components[i].componentOffset=currentOffset;
                currentOffset+=(ushort)Marshal.SizeOf(components[i]);
            }
        }
        public bool FitsQuery(EntityQuery q)
        {
            for (int i = 0; i < q.components.Length; i++)
            {
                for (int j = 0; j < components.Length; j++)
                {
                    if (q.components[i] == components[j].type)
                        goto next;
                }
                return false;

            next: continue;
            }
            return true;
        }
    }

    public struct EntityQuery
    {
        public Type[] components;
    }

    public struct ComponentMetadata
    {
        public Type type;
        public ushort componentOffset;
    }

    public interface IComponent
    {

    }
}