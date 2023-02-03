// author:KIPKIPS
// date:2023.02.03 22:04
// describe:只读字典,开启读权限
using System.Collections.Generic;

namespace Framework.Core.Container {
    public class RestrictedDictionary<TKey, TValue> : IDictionary<TKey, TValue> {
        private bool _writable;
        /// <summary>
        /// 开启写权限
        /// </summary>
        public void EnableWrite() => _writable = true;
        /// <summary>
        /// 禁用写权限
        /// </summary>
        public void ForbidWrite() => _writable = false;
        public RestrictedDictionary() => _dictionary = new Dictionary<TKey, TValue>();
        private readonly IDictionary<TKey, TValue> _dictionary;
        public void Add(TKey key, TValue value) {
            if (_writable) {
                _dictionary.Add(key, value);
            }
        }
        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);
        public ICollection<TKey> Keys => _dictionary.Keys;
        public bool Remove(TKey key) => _writable && _dictionary.Remove(key);
        public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);
        public ICollection<TValue> Values => _dictionary.Values;
        public TValue this[TKey key] {
            get => _dictionary[key];
            set {
                if (_writable) {
                    _dictionary[key] = value;
                }
            }
        }
        TValue IDictionary<TKey, TValue>.this[TKey key] {
            get => this[key];
            set {
                if (_writable) {
                    this[key] = value;
                }
            }
        }
        public void Add(KeyValuePair<TKey, TValue> item) {
            if (_writable) {
                _dictionary.Add(item);
            }
        }
        public void Clear() {
            if (_writable) {
                _dictionary.Clear();
            }
        }
        public bool Contains(KeyValuePair<TKey, TValue> item) => _dictionary.Contains(item);
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => _dictionary.CopyTo(array, arrayIndex);
        public int Count => _dictionary.Count;
        public bool IsReadOnly => false;
        public bool Remove(KeyValuePair<TKey, TValue> item) => _writable && _dictionary.Remove(item);
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();
    }
}