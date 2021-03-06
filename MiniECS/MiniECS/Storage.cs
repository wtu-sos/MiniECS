using System.Collections.Generic;

namespace ECS
{
    public class Storage<T> where T: struct
    {
        private Dictionary<ArchType, ArchData<T>> _storages = new();
        private T DefaultData = new T();

        public (int, int) Add(ArchType at, T t)
        {
            if (_storages.TryGetValue(at, out var storage))
            {
                return storage.Add(t);
            }
            else
            {
                var dt = new ArchData<T>();
                var r = dt.Add(t);
                _storages.Add(at, dt);
                return r;
            }
        }

        public bool Remove(EntityKey key)
        {
            if (_storages.TryGetValue(key._type, out var storage))
            {
                return storage.Remove(key.segment, key.index);
            }
            
            return true;
        }

        public bool Remove(ArchType t, int seg, int idx)
        {
            if (_storages.TryGetValue(t, out var storage))
            {
                return storage.Remove(seg, idx);
            }
            
            return true;
        }

        public ref T Get(EntityKey key)
        {
            if (_storages.TryGetValue(key._type, out var storage))
            {
                return ref storage.Get(key.segment, key.index);
            }

            throw new System.Exception("miss data");
        }

        public void ViewReset(ArchType t)
        {
            if (_storages.TryGetValue(t, out var storage))
            {
                storage.ViewReset();
            }
        }

        public ArchData<T> GetStorage(ArchType t)
        {
            if (_storages.TryGetValue(t, out var storage))
            {
                return storage;
            }

            return null;
        }

        public ref T ViewRef(ArchType t, out bool o)
        {
            o = false;
            if (_storages.TryGetValue(t, out var storage))
            {
                return ref storage.ViewRef(out o);
            }
            return ref DefaultData;
        }

        public IEnumerable<T> View(ArchType t)
        {
            if (_storages.TryGetValue(t, out var storage))
            {
                foreach (var d in storage.View())
                {
                    yield return d;
                }
            }
        }

        public void DebugInfo()
        {
            System.Console.WriteLine($"{ _storages.ToString()}  => {_storages.Count}");
            foreach(var s in _storages)
            {
                string info = s.Value.DebugInfo();
                System.Console.WriteLine(info);
            }
        }
    }
}