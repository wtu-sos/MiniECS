using System.Collections.Generic;

namespace ECS
{
    public class World
    {
        private Dictionary<System.Type, object> _everything = new();

        public IEnumerable<T> View<T >(ArchType t) where T: struct
        {
            if (_everything.TryGetValue(typeof(T), out var store))
            {
                foreach (var data in ((Storage<T>)store).View(t))
                {
                    yield return data;
                }
            }
        }

        public (ArchType, int, int) Add<T>(ArchType t, T data) where T : struct
        {

            if (_everything.TryGetValue(typeof(T), out var store))
            {
                var r = ((Storage<T>)store).Add(t, data);
                return (t, r.Item1, r.Item2);
            }
            else
            {
                var store_ = new Storage<T>();
                var r = store_.Add(t, data);
                _everything.Add(typeof(T), store_);
                return (t, r.Item1, r.Item2);
            }
        }

        public bool Remove<T>(ArchType t, int seg, int idx) where T: struct
        {
            if (_everything.TryGetValue(typeof(T), out var store))
            {
                return ((Storage<T>)store).Remove(t, seg, idx);
            }

            return true;
        }
    }
}