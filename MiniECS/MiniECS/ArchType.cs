using System.Collections.Generic;
using System;

namespace ECS
{
    public class ArchTypeStorage
    {
        private List<ArchType> _ats;

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
    }
    
    public class ArchType
    {
        private HashSet<Type> _types = new HashSet<Type>();

        public void Add(Type t)
        {
            _types.Add(t);
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

        public ArchType Clone()
        {
            var arch = new ArchType();
            foreach(var t in _types)
                arch.Add(t);
            return arch;
        }
    }
}