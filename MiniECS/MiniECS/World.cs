using System.Collections.Generic;
using System;

namespace ECS
{
    public interface ISystem
    {
        public void Exec(World world);
    }

    public class World
    {
        private Dictionary<System.Type, object> _everything = new();
        private ArchTypeStorage arches = new ArchTypeStorage();
        public ArchTypeStorage Arches { get => arches; set => arches = value; }

        private List<ISystem> systems = new List<ISystem>();

        public void AddSystem(ISystem sys)
        {
            systems.Add(sys);
        }

        public void Exec()
        {
            foreach (var system in systems)
            {
                system.Exec(this);
            }
        }

        public IEnumerable<T> View<T>(ArchType t) where T: struct
        {
            if (_everything.TryGetValue(typeof(T), out var store))
            {
                foreach (var data in ((Storage<T>)store).View(t))
                {
                    yield return data;
                }
            }
        }

        public EntityKey Add<T>(ArchType t, T data) where T : struct
        {
            if (_everything.TryGetValue(typeof(T), out var store))
            {
                var r = ((Storage<T>)store).Add(t, data);
                return new EntityKey(t, r.Item1, r.Item2);
            }
            else
            {
                var store_ = new Storage<T>();
                var r = store_.Add(t, data);
                _everything.Add(typeof(T), store_);
                return new EntityKey(t, r.Item1, r.Item2);
            }
        }

        public bool Add<T>(EntityKey key, T data) where T : struct
        {

            if (_everything.TryGetValue(typeof(T), out var store))
            {
                var r = ((Storage<T>)store).Add(key._type, data);
                if (r.Item1 != key.segment || r.Item2 != key.index)
                {
                    ((Storage<T>)store).Remove(key._type, r.Item1, r.Item2);
                    return false;
                }
                return true;
            }
            else
            {
                var store_ = new Storage<T>();
                var r = store_.Add(key._type, data);

                if (r.Item1 != key.segment || r.Item2 != key.index)
                {
                    store_.Remove(key._type, r.Item1, r.Item2);
                    return false;
                }

                _everything.Add(typeof(T), store_);
                return true;
            }
            return false;
        }

        public Entity CreateEntity(ArchType at)
        {
            var ts = at.Get();
            EntityKey key = null;
            foreach(var kv in ts)
            {
                if (_everything.TryGetValue(kv, out object store))
                {
                    var (seg, idx) = ((int, int))(store.GetType().GetMethod("Add").Invoke(store, new object[] { at, default }));
                    if (null == key)
                    {
                        key = new EntityKey(at, seg, idx);
                    }
                }
                else
                {
                    Type s0 = typeof(Storage<>);
                    Type s1 = typeof(Dictionary<,>);
                    Type storeType = s0.MakeGenericType(kv);
                    var _store = Activator.CreateInstance(storeType);
                    var (seg, idx) = ((int, int))(_store.GetType().GetMethod("Add").Invoke(_store, new object[] { at, default }));
                    if (null == key)
                    {
                        key = new EntityKey(at, seg, idx);
                    }
                    _everything.Add(kv, _store);
                }
            }

            return new Entity(key);
        }

        public void DebugInfo()
        {
            foreach(var (k, store) in _everything)
            {
                store.GetType().GetMethod("DebugInfo").Invoke(store, new object[] { });
            }
        }

        private bool Remove<T>(EntityKey key) where T: struct
        {
            if (_everything.TryGetValue(typeof(T), out var store))
            {
                return ((Storage<T>)store).Remove(key);
            }

            return true;
        }

        public bool MoveEntity(Entity entity, ArchType t)
        {
            return false;
        }
    }
}