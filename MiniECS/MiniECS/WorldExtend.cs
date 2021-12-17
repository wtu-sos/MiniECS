using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    partial class World
    {
        public delegate void ActionRef<T1>(ref T1 item1);
        public delegate void ActionRef<T1, T2>(ref T1 item1, ref T2 item2);
        public delegate void ActionRef<T1, T2, T3>(ref T1 item1, ref T2 item2, ref T3 item3);
        public delegate void ActionRef<T1, T2, T3, T4>(ref T1 item1, ref T2 item2, ref T3 item3, ref T4 item4);
        public delegate void ActionRef<T1, T2, T3, T4, T5>(ref T1 item1, ref T2 item2, ref T3 item3, ref T4 item4, ref T5 item5);

        private bool ZipInner<T1>(ArchData<T1> s1, ActionRef<T1> action)
            where T1 : struct
        {
            ref T1 t1 = ref s1.ViewRef(out bool r1);
            if (r1) 
            {
                action(ref t1);
                return r1;
            }

            return false;
        }
        private bool ZipInner<T1, T2>(ArchData<T1> s1, ArchData<T2> s2, ActionRef<T1, T2> action)
            where T1 : struct
            where T2 : struct
        {
            ref T1 t1 = ref s1.ViewRef(out bool r1);
            ref T2 t2 = ref s2.ViewRef(out bool r2);
            if (r1 && r2) 
            {
                action(ref t1, ref t2);
                return r1 && r2;
            }

            return false;
        }
        private bool ZipInner<T1, T2, T3>(ArchData<T1> s1, ArchData<T2> s2, ArchData<T3> s3, ActionRef<T1, T2, T3> action)
            where T1 : struct
            where T2 : struct
            where T3 : struct
        {
            ref T1 t1 = ref s1.ViewRef(out bool r1);
            ref T2 t2 = ref s2.ViewRef(out bool r2);
            ref T3 t3 = ref s3.ViewRef(out bool r3);
            if (r1 && r2 && r3) 
            {
                action(ref t1, ref t2, ref t3);
                return r1 && r2 && r3;
            }

            return false;
        }


        public IEnumerable<bool> Zip<T1>(ArchType t, ActionRef<T1> action) 
            where T1 : struct
        {
            ViewReset<T1>(t);
            var s1 = GetStorage<T1>(t);

            if (null == s1)
            {
                yield return false;
            }

            while (ZipInner(s1, action))
            {
                yield return true;
            }

            yield return false;
        }
        public IEnumerable<bool> Zip<T1, T2>(ArchType t, ActionRef<T1, T2> action) 
            where T1 : struct
            where T2 : struct
        {
            ViewReset<T1>(t);
            ViewReset<T2>(t);
            var s1 = GetStorage<T1>(t);
            var s2 = GetStorage<T2>(t);

            if (null == s1 || null == s2)
            {
                yield return false;
            }

            while (ZipInner(s1, s2, action))
            {
                yield return true;
            }

            yield return false;
        }
        public IEnumerable<bool> Zip<T1, T2, T3>(ArchType t, ActionRef<T1, T2, T3> action) 
            where T1 : struct
            where T2 : struct
            where T3 : struct
        {
            ViewReset<T1>(t);
            ViewReset<T2>(t);
            ViewReset<T3>(t);
            var s1 = GetStorage<T1>(t);
            var s2 = GetStorage<T2>(t);
            var s3 = GetStorage<T3>(t);

            if (null == s1 || null == s2 || null == s3)
            {
                yield return false;
            }

            while (ZipInner(s1, s2, s3, action))
            {
                yield return true;
            }

            yield return false;
        }
    }
}
