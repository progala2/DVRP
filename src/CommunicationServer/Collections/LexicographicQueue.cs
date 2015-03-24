using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _15pl04.Ucc.CommunicationServer.Collections
{
    public class LexicographicQueue<TKey, TVal>
    {
        private Dictionary<TKey, Queue<TVal>> _dictionary;

        public LexicographicQueue()
        {
            _dictionary = new Dictionary<TKey, Queue<TVal>>();
        }

        public void Enqueue(TKey key, TVal value)
        {
            if (!_dictionary.ContainsKey(key))
                _dictionary.Add(key, new Queue<TVal>());

            _dictionary[key].Enqueue(value);
        }

        public bool TryDequeue(TKey key, out TVal value)
        {
            value = default(TVal);

            if (!_dictionary.ContainsKey(key) || _dictionary[key].Count == 0)
                return false;

            value = _dictionary[key].Dequeue();
            return true;
        }

        public int GetCount(TKey key)
        {
            if (!_dictionary.ContainsKey(key))
                return 0;

            return _dictionary[key].Count;
        }
    }
}
