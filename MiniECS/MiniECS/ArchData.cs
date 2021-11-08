using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ECS
{

    public class DataArray<T> where T: struct
    {
        public DataArray(int index, int Cap = 10)
        {
            _inner = new T[Cap];
            Capcity = Cap;
            Index = index;
        }

        public int Index;
        private T[] _inner = null;
        private int Capcity = 0;
        private int Size = 0;

        public int Add(T t)
        {
            if (Size == Capcity)
            {
                return -1;
            }

            var index = Size;
            _inner[index] = t;
            Size += 1;
            return index;
        }

        public bool Remove(int index)
        {
            if (0 > index)
            {
                return false;
            }

            if (index >= Size)
            {
                return false;
            }

            var last = Size - 1;
            if (index != last)
            {
                _inner[index] = _inner[last];
            }
            Size -= 1;
            
            return true;
        }

        public IEnumerable<T> View()
        {
            int idx = 0;
            foreach (var t in _inner)
            {
                if (idx >= Size)
                {
                    yield break;
                }

                ++idx;
                yield return t;
            }
        }

        public string ArrayInfo()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < Size; i++)
            {
                sb.Append($"{JsonSerializer.Serialize(_inner[i])},");
            }
            return sb.ToString();
        }
    }
    
    public class ArchData<T> where T: struct
    {
        private ArchType _type;
        private List<DataArray<T>> _store = new List<DataArray<T>>();
        private const int segment_size = 20;
        private int current_segment_index = 0;

        private List<DataArray<T>> _free_array = new List<DataArray<T>>();

        public (int, int) Add(T t)
        {
            if (_free_array.Count == 0)
            {
                NewArray();
            }

            var ar = _free_array[0];
            var ar_idx = ar.Add(t);
            if (ar_idx < 0)
            {
                return (-1, -1);
            }

            return (ar.Index, ar_idx);
        }

        public bool Remove(int seg, int i)
        {
            if (_store.Count <= seg)
            {
                return false;
            }

            return _store[seg].Remove(i);
        }

        public IEnumerable<T> View()
        {
            foreach (DataArray<T> array in _store)
            {
                foreach (var t in array.View())
                {
                    yield return t;
                }
            }
        }

        private void NewArray()
        {
            var na = new DataArray<T>(current_segment_index, segment_size);
            ++current_segment_index;
            _store.Add(na);
            _free_array.Add(na);
        }

        public string DebugInfo()
        {
            var sb = new StringBuilder();
            
            foreach (var array in _store)
            {
                sb.AppendLine(array.ArrayInfo());
            }
            
            return sb.ToString();
        }
    }
}