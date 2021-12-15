namespace ECS
{
    public class EntityKey
    {
        public EntityKey(ArchType t, int seg, int idx)
        {
            _type = t;
            segment = seg;
            index = idx;
        }
        
        public ArchType _type;
        public int segment;
        public int index;
    }
    
    public class Entity
    {
        public Entity(EntityKey _key)
        {
            key = _key; 
        }

        private EntityKey key;
    }
}