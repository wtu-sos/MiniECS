using System;
using System.Collections.Generic;

namespace ECS
{
    public class EntityBuilder
    {
        public static EntityBuilder Default = new EntityBuilder();

        private List<System.ValueType> comps = new List<ValueType>();

        public EntityBuilder AddComponent<T>(T t) where T: struct
        {
            comps.Add(t);
            return this;
        }

        public void Build(World world)
        {
            
        }
    }
}