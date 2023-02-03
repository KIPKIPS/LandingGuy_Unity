// author:KIPKIPS
// date:2023.02.03 22:04
// describe:只读字典,开启读权限
using System.Collections.Generic;

namespace Framework.Core.Container {
    public class RestrictedDictionary<TKey, TValue> : IDictionary<TKey, TValue> {
        private bool _writable;
        public void EnableWrite() {
            _writable = true;
        }
        public void ForbidWrite() {
            _writable = false;
        }
        public RestrictedDictionary() {
            _dictionary = new Dictionary<TKey, TValue>();
        }
        private IDictionary<TKey, TValue> _dictionary;
        public void Add(TKey key, TValue value) {
            if (_writable) {
                _dictionary.Add(key, value);
            }
        }
        public bool ContainsKey(TKey key) {
            return _dictionary.ContainsKey(key);
        }
        public ICollection<TKey> Keys {
            get => _dictionary.Keys;
        }
        public bool Remove(TKey key) {
            if (_writable) {
                return _dictionary.Remove(key);
            }
            return false;
        }
        public bool TryGetValue(TKey key, out TValue value) {
            return _dictionary.TryGetValue(key, out value);
        }
        public ICollection<TValue> Values {
            get => _dictionary.Values;
        }
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
        public bool Contains(KeyValuePair<TKey, TValue> item) {
            return _dictionary.Contains(item);
        }
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            _dictionary.CopyTo(array, arrayIndex);
        }
        public int Count {
            get => _dictionary.Count;
        }
        public bool IsReadOnly {
            get => false;
        }
        public bool Remove(KeyValuePair<TKey, TValue> item) {
            if (_writable) {
                return _dictionary.Remove(item);
            }
            return false;
        }
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return _dictionary.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return ((IEnumerable<KeyValuePair<TKey, TValue>>)_dictionary).GetEnumerator();
        }
    }
}