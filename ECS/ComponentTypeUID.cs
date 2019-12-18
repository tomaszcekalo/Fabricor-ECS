using System;
using System.Collections.Generic;
namespace Fabricor.ECS
{
    public static class ComponentType
    {
        private static Dictionary<Type,uint> lookup=new Dictionary<Type, uint>();
        private static uint head=0;
        public static uint GetUID(Type component){
            if(lookup.ContainsKey(component)){
                return lookup[component];
            }
            lock(lookup){
                lookup.Add(component,head);
                head++;
            }
            return GetUID(component);
        }
    }
}