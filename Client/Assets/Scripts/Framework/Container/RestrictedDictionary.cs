// author:KIPKIPS
// date:2023.02.03 22:04
// describe:只读字典,开启读权限
using System.Collections.Generic;

namespace Framework.Container {
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
        /// <summary>
        /// 添加键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Add(TKey key, TValue value) {
            if (_writable) {
                _dictionary.Add(key, value);
            }
        }
        /// <summary>
        /// 是否包含键对应值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);
        public ICollection<TKey> Keys => _dictionary.Keys;
        /// <summary>
        /// 移除键对应的值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>被移除的值</returns>
        public bool Remove(TKey key) => _writable && _dictionary.Remove(key);
        /// <summary>
        /// 尝试获取键对应的值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>是否可以获取到</returns>
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
        /// <summary>
        /// 添加键值对
        /// </summary>
        /// <param name="item">键值对</param>
        public void Add(KeyValuePair<TKey, TValue> item) {
            if (_writable) {
                _dictionary.Add(item);
            }
        }
        /// <summary>
        /// 清除字典
        /// </summary>
        public void Clear() {
            if (_writable) {
                _dictionary.Clear();
            }
        }
        /// <summary>
        /// 是否包含键值对
        /// </summary>
        /// <param name="item">键值对</param>
        /// <returns>是否包含改键值对</returns>
        public bool Contains(KeyValuePair<TKey, TValue> item) => _dictionary.Contains(item);
        /// <summary>
        /// 将字串字典值复制至在指定索引处的一维array执行个体
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => _dictionary.CopyTo(array, arrayIndex);
        public int Count => _dictionary.Count;
        public bool IsReadOnly => false;
        /// <summary>
        /// 移除键值对
        /// </summary>
        /// <param name="item">键值对</param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<TKey, TValue> item) => _writable && _dictionary.Remove(item);
        /// <summary>
        /// 获取键值对的迭代器
        /// </summary>
        /// <returns>遍历的迭代器</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();
    }
}