﻿using System;
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

        public T Get(int index)
        {
            if (0 > index || index >= _size)
            {
                throw new IndexOutOfRangeException();
            }

            return _inner[index]; 
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
        private const int SegmentSize = 20;
        private int _currentSegmentIndex = 0;

        private List<DataArray<T>> _freeArray = new List<DataArray<T>>();

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

        public T Get(int seg, int i)
        {
            if (_store.Count <= seg)
            {
                throw new IndexOutOfRangeException("");
            }

            return _store[seg].Get(i);
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