// author:KIPKIPS
// date:2023.02.03 22:08
// describe:可序列化字典实现
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework.Container {
    [Serializable]
    public class SerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
        [SerializeField] private List<TKey> keys = new ();
        [SerializeField] private List<TValue> values = new ();

        /// <summary>
        /// OnBeforeSerialize implementation.
        /// </summary>
        public void OnBeforeSerialize() {
            keys.Clear();
            values.Clear();
            foreach (var (key, value) in this) {
                keys.Add(key);
                values.Add(value);
            }
        }

        /// <summary>
        /// OnAfterDeserialize implementation
        /// </summary>
        public void OnAfterDeserialize() {
            for (var i = 0; i < keys.Count; i++) {
                Add(keys[i], values[i]);
            }
            keys.Clear();
            values.Clear();
        }
    }
}