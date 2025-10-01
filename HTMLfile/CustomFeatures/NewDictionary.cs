using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLfile.CustomFeatures
{
    public class NewDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IEqualityComparer<TKey> _equalityComparer;
        private readonly List<KeyValuePair<TKey, TValue>>[] _items;
        private NewDictionary<string, string> attributes;
        private const int DefaultCapacity = 16;

        public int Count { get; private set; }

        public NewDictionary() : this(DefaultCapacity, EqualityComparer<TKey>.Default) { }

        public NewDictionary(int capacity, EqualityComparer<TKey> equalityComparer)
        {
            _equalityComparer = equalityComparer ?? throw new ArgumentNullException(nameof(equalityComparer));
            _items = new List<KeyValuePair<TKey, TValue>>[capacity];
            for (var i = 0; i < capacity; i++)
            {
                _items[i] = [];
            }

            Keys = new List<TKey>();
            Values = new List<TValue>();
        }

        public NewDictionary(NewDictionary<string, string> attributes)
        {
            this.attributes = attributes;
        }

        private int GetItemIndex(TKey key)
        {
            if (key is null) throw new KeyNotFoundException();

            var hash = key.GetHashCode();
            hash = Math.Abs(hash);
            return hash % _items.Length;
        }

        public TValue this[TKey key]
        {
            get
            {
                var index = GetItemIndex(key);
                var list = _items[index];

                if (list.Count == 0)
                    throw new KeyNotFoundException();
                foreach (var item in list)
                {
                    if (_equalityComparer.Equals(item.Key, key))
                        return item.Value;
                }

                throw new KeyNotFoundException();
            }
            set
            {
                var index = GetItemIndex(key);
                var list = _items[index];

                for (var i = 0; i < list.Count; i++)
                {
                    if (!_equalityComparer.Equals(list[i].Key, key)) continue;
                    list[i] = new KeyValuePair<TKey, TValue>(key, value);
                    return;
                }

                list.Add(new KeyValuePair<TKey, TValue>(key, value));
                ((List<TKey>)Keys).Add(key);
                ((List<TValue>)Values).Add(value);
                Count++;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var list in _items)
            {
                foreach (var item in list)
                {
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this[item.Key] = item.Value;
        }

        public void Clear()
        {
            foreach (var list in _items)
            {
                list.Clear();
            }

            ((List<TKey>)Keys).Clear();
            ((List<TValue>)Values).Clear();
            Count = 0;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return TryGetValue(item.Key, out var value) && EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ArgumentNullException.ThrowIfNull(array);
            ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex);
            if (array.Length - arrayIndex < Count)
                throw new ArgumentException("The array is too small");

            foreach (var list in _items)
            {
                foreach (var item in list)
                {
                    array[arrayIndex++] = item;
                }
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            this[key] = value;
        }

        public bool ContainsKey(TKey key)
        {
            return TryGetValue(key, out _);
        }

        public bool Remove(TKey key)
        {
            var index = GetItemIndex(key);
            var list = _items[index];

            for (var i = 0; i < list.Count; i++)
            {
                if (!_equalityComparer.Equals(list[i].Key, key)) continue;

                list.RemoveAt(i);
                ((List<TKey>)Keys).Remove(key);
                ((List<TValue>)Values).Remove(list[i].Value);
                Count--;
                return true;
            }

            return false;
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            var index = GetItemIndex(key);
            var list = _items[index];

            foreach (var item in list)
            {
                if (!_equalityComparer.Equals(item.Key, key)) continue;
                value = item.Value;
                return true;
            }

            value = default;
            return false;
        }

        public ICollection<TKey> Keys { get; }
        public ICollection<TValue> Values { get; }
    }

}

