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
        private Dictionary<System.Type, object> _defaultData = new();

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

        public void ViewReset<T>(ArchType t) where T: struct
        {
            if (_everything.TryGetValue(typeof(T), out var store))
            {
                ((Storage<T>)store).ViewReset(t);
            }
        }

        public ref T ViewRef<T>(ArchType t, out bool o) where T: struct
        {
            o = false;
            if (_everything.TryGetValue(typeof(T), out var store))
            {
                return ref ((Storage<T>)store).ViewRef(t, out o);
            }
            if (!_defaultData.TryGetValue(typeof(T), out var d))
            {
                d = new T();
            }

            throw new InvalidOperationException("");
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

        public delegate void ActionRef<T1, T2, T3>(ref T1 item1, ref T2 item2, ref T3 item3);

        public IEnumerable<bool> Zip<T1, T2, T3>(ArchType t, ActionRef<T1, T2, T3> action) 
            where T1 : struct
            where T2 : struct
            where T3 : struct
        {

            ViewReset<T1>(t);
            ViewReset<T2>(t);
            ViewReset<T3>(t);

            while (true)
            {
                //T1 t1 = ViewRef<T1>(t, out bool r1);
                //T2 t2 = ViewRef<T2>(t, out bool r2);
                //T3 t3 = ViewRef<T3>(t, out bool r3);
                //if (r1 && r2 && r3) 
                //{
                    action(ref ViewRef<T1>(t, out bool r1), ref ViewRef<T2>(t, out bool r2), ref ViewRef<T3>(t, out bool r3));
                    yield return r1 && r2 && r3;
                //}

                //break;
            }

            yield return false;
        }
    }
}