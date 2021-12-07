using System;
using System.Collections.Generic;

namespace ECS
{
    public class EntityBuilder
    {
        public static EntityBuilder Default = new EntityBuilder();

        public EntityBuilder AddComponent<T>(T t) where T: struct
        {
            return this;
        }

        public void Build(World world)
        {
        }
    }
}