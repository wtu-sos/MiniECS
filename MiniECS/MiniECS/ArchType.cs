using System;
using System.Collections.Generic;

namespace ECS
{
    public class ArchTypeStorage
    {
        private List<ArchType> _ats = new List<ArchType>();

        public List<ArchType> View(Type t)
        {
            var r = new List<ArchType>();
            foreach (var tp in _ats)
            {
                if (tp.match(t))
                {
                    r.Add(tp);
                }
            }
            return r;
        }

        public List<ArchType> View(Type t1, Type t2)
        {
            var r = new List<ArchType>();
            foreach (var tp in _ats)
            {
                if (tp.match(t1, t2))
                {
                    r.Add(tp);
                }
            }
            return r;
        }

        public List<ArchType> View(Type t1, Type t2, Type t3)
        {
            var r = new List<ArchType>();
            foreach (var tp in _ats)
            {
                if (tp.match(t1, t2, t3))
                {
                    r.Add(tp);
                }
            }
            return r;
        }

        public List<ArchType> View(params Type[] t)
        {
            var r = new List<ArchType>();
            foreach (var tp in _ats)
            {
                if (tp.match(t))
                {
                    r.Add(tp);
                }
            }
            return r;
        }

        public ArchType GetArchType(Type t1)
        {
            foreach (var tp in _ats)
            {
                if (tp.Kind() != 1)
                {
                    continue;
                }
                if (tp.match(t1))
                {
                    return tp;
                }
            }

            ArchType newAt = new ArchType();
            newAt.Add(t1);
            _ats.Add(newAt);
            return newAt;
        }

        public ArchType GetArchType(Type t1, Type t2)
        {
            foreach (var tp in _ats)
            {
                if (tp.Kind() != 2)
                {
                    continue;
                }
                if (tp.match(t1, t2))
                {
                    return tp;
                }
            }

            ArchType newAt = new ArchType();
            newAt.Add(t1);
            newAt.Add(t2);
            _ats.Add(newAt);
            return newAt;
        }

        public ArchType GetArchType(Type t1, Type t2, Type t3)
        {
            foreach (var tp in _ats)
            {
                if (tp.Kind() != 3)
                {
                    continue;
                }
                if (tp.match(t1, t2, t3))
                {
                    return tp;
                }
            }

            ArchType newAt = new ArchType();
            newAt.Add(t1);
            newAt.Add(t2);
            newAt.Add(t3);
            _ats.Add(newAt);
            return newAt;
        }

        public ArchType GetArchType(params Type[] t)
        {
            foreach (var tp in _ats)
            {
                if (tp.Kind() != t.Length)
                {
                    continue;
                }
                if (tp.match(t))
                {
                    return tp;
                }
            }

            ArchType newAt = new ArchType();
            foreach(var tp in t)
            {
                newAt.Add(tp);
            }
            _ats.Add(newAt);
            return newAt;
        }
    }

    public class ArchType
    {
        private HashSet<Type> _types = new HashSet<Type>();

        public void Add(Type t)
        {
            _types.Add(t);
        }

        public int Kind()
        {
            return _types.Count;
        }

        public HashSet<Type> Get()
        {
            return _types;
        }

        public bool match(Type t)
        {
            return _types.Contains(t);
        }

        public bool match(Type t1, Type t2)
        {
            return _types.Contains(t1) && _types.Contains(t2);
        }

        public bool match(Type t1, Type t2, Type t3)
        {
            return _types.Contains(t1) && _types.Contains(t2) && _types.Contains(t3);
        }

        public bool match(params Type[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                if (!_types.Contains(types[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public ArchType Clone()
        {
            var arch = new ArchType();
            foreach (var t in _types)
                arch.Add(t);
            return arch;
        }
    }
}