using System;
using System.Collections.Generic;
using System.Linq;

namespace _15pl04.Ucc.CommunicationServer.Collections
{
    public class LexicographicList<TKey, TVal>
    {
        private readonly Dictionary<TKey, LinkedList<TVal>> _dictionary;

        public LexicographicList()
        {
            _dictionary = new Dictionary<TKey, LinkedList<TVal>>();
        }

        public LexicographicList(int capacity)
        {
            _dictionary = new Dictionary<TKey, LinkedList<TVal>>(capacity);
        }

        public ICollection<TVal> this[TKey key]
        {
            get { return _dictionary[key]; }
            set { _dictionary[key] = new LinkedList<TVal>(value); }
        }

        public bool TryGetFirst(TKey key, Predicate<TVal> predicate, out TVal value)
        {
            value = default(TVal);

            if (!_dictionary.ContainsKey(key) || _dictionary[key].Count == 0)
                return false;

            var node = _dictionary[key].First;
            while (node != null)
            {
                if (predicate(node.Value))
                {
                    value = node.Value;
                    break;
                }
                node = node.Next;
            }

            return node != null;
        }

        public int GetCountByKey(TKey key)
        {
            if (!_dictionary.ContainsKey(key))
                return 0;

            return _dictionary[key].Count;
        }

        public void AddLast(TKey key, TVal value)
        {
            if (!_dictionary.ContainsKey(key))
                _dictionary.Add(key, new LinkedList<TVal>());

            _dictionary[key].AddLast(value);
        }

        public void AddLast(TKey key, ICollection<TVal> values)
        {
            if (!_dictionary.ContainsKey(key))
                _dictionary[key] = new LinkedList<TVal>(values);
            else
                _dictionary[key] = (LinkedList<TVal>) _dictionary[key].Concat(values);
        }

        public bool TryRemoveFirst(TKey key, out TVal value)
        {
            if (!_dictionary.ContainsKey(key) || _dictionary[key].Count == 0)
            {
                value = default(TVal);
                return false;
            }

            value = _dictionary[key].First.Value;
            _dictionary[key].RemoveFirst();

            return true;
        }

        public bool TryRemoveFirst(TKey key, Predicate<TVal> predicate, out TVal value)
        {
            value = default(TVal);

            if (!_dictionary.ContainsKey(key) || _dictionary[key].Count == 0)
                return false;

            var node = _dictionary[key].First;
            while (node != null)
            {
                if (predicate(node.Value))
                {
                    value = node.Value;
                    _dictionary[key].Remove(node);
                    break;
                }
                node = node.Next;
            }

            return node != null;
        }

        public bool TryRemove(TKey key, TVal value)
        {
            if (!_dictionary.ContainsKey(key) || _dictionary[key].Count == 0)
                return false;

            return _dictionary[key].Remove(value);
        }

        public ICollection<TVal> RemoveAllByKey(TKey key)
        {
            if (_dictionary.ContainsKey(key))
            {
                var temp = _dictionary[key];
                _dictionary.Remove(key);
                return temp;
            }
            return null;
        }
    }
}