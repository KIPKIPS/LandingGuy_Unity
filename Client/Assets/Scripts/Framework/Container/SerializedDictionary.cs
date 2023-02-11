// author:KIPKIPS
// date:2023.02.03 22:08
// describe:可序列化字典实现
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework.Container {
    [Serializable]
    public class SerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
        [SerializeField] List<TKey> keys = new ();
        [SerializeField] List<TValue> values = new ();

        /// <summary>
        /// OnBeforeSerialize implementation.
        /// </summary>
        public void OnBeforeSerialize() {
            keys.Clear();
            values.Clear();
            foreach (var kvp in this) {
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }
        }

        /// <summary>
        /// OnAfterDeserialize implementation
        /// </summary>
        public void OnAfterDeserialize() {
            for (int i = 0; i < keys.Count; i++) {
                Add(keys[i], values[i]);
            }
            keys.Clear();
            values.Clear();
        }
    }
}