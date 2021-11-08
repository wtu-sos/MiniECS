using System.Collections.Generic;

namespace ECS
{
    public class Storage<T> where T: struct
    {
        private Dictionary<ArchType, ArchData<T>> _storages = new();

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

        public bool Remove(ArchType at, int seg, int idx)
        {
            if (_storages.TryGetValue(at, out var storage))
            {
                return storage.Remove(seg, idx);
            }
            
            return true;
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
    }
}