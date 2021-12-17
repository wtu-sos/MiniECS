using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ECS
{
    public class DataArray<T> where T: struct
    {
        public DataArray(int index, int Cap = 10)
        {
            _inner = new T[Cap];
            _capacity = Cap;
            Index = index;
        }

        public int Index;
        private T[] _inner = null;
        private readonly int _capacity = 0;
        private int _size = 0;
        private T defaultData = new T(); 


        public bool HasValue(int idx)
        {
            return idx < _size;
        }

        public int Add(T t)
        {
            if (_size == _capacity)
            {
                return -1;
            }

            var index = _size;
            _inner[index] = t;
            _size += 1;
            return index;
        }

        public bool Remove(int index)
        {
            if (0 > index)
            {
                return false;
            }

            if (index >= _size)
            {
                return false;
            }

            var last = _size - 1;
            if (index != last)
            {
                _inner[index] = _inner[last];
            }
            _size -= 1;
            
            return true;
        }

        public ref T Get(int index)
        {
            if (0 > index || index >= _size)
            {
                throw new IndexOutOfRangeException();
            }

            return ref _inner[index]; 
        }

        public ref T Get(int index, out bool o)
        {
            if (0 > index || index >= _size)
            {
                //throw new IndexOutOfRangeException();
                o = false;
                return ref defaultData;
            }

            o = true;
            return ref _inner[index]; 
        }

        public IEnumerable<T> View()
        {
            for (int idx = 0; idx < _size; idx++)
            {
                yield return _inner[idx];   
            }
        }

        public bool IsFull()
        {
            return _size == _capacity;
        }

        public string ArrayInfo()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < _size; i++)
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
        private const int SegmentSize = 40;
        private int _currentSegmentIndex = 0;
        private T defautData = new T();

        private List<DataArray<T>> _freeArray = new List<DataArray<T>>();

        private class Cursor
        {
            public Cursor() { }
            public void Clear() {
                Seg = 0;
                Idx = -1;
            }

            public int Seg = 0;
            public int Idx = -1;
        }

        private Cursor _cursor = new();

        public (int, int) Add(T t)
        {
            if (_freeArray.Count == 0)
            {
                NewArray();
            }

            var ar = _freeArray[0];
            var ar_idx = ar.Add(t);
            if (ar_idx < 0)
            {
                return (-1, -1);
            }

            if (ar.IsFull())
            {
                _freeArray.RemoveAt(0);
            }

            return (ar.Index, ar_idx);
        }

        public bool Remove(int seg, int i)
        {
            if (_store.Count <= seg)
            {
                return false;
            }

            var r = _store[seg].Remove(i);
            if (r)
            {
                _freeArray.Add(_store[seg]);
            }
            return r;
        }

        public ref T Get(int seg, int i, out bool o)
        {
            if (_store.Count <= seg)
            {
                throw new IndexOutOfRangeException("");
            }

            //o = true;
            return ref _store[seg].Get(i, out o);
        }

        public ref T Get(int seg, int i)
        {
            if (_store.Count <= seg)
            {
                throw new IndexOutOfRangeException("");
            }

            if (_store[seg].HasValue(i))
            {
                return ref defautData;
            }

            return ref _store[seg].Get(i);
        }

        public ArchData<T> ViewReset()
        {
            _cursor.Clear();
            return this;
        }

        public ref T ViewRef(out bool o)
        {
            ++_cursor.Idx;
            if (_cursor.Idx >= SegmentSize)
            {
                _cursor.Idx = 0;
                ++_cursor.Seg;
            }
            if (_cursor.Seg >= _store.Count)
            {
                o = false;
                return ref defautData;
            }

            return ref Get(_cursor.Seg, _cursor.Idx, out o);
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
            var na = new DataArray<T>(_currentSegmentIndex, SegmentSize);
            ++_currentSegmentIndex;
            _store.Add(na);
            _freeArray.Add(na);
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