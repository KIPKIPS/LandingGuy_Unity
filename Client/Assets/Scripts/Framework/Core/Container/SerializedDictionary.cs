// author:KIPKIPS
// date:2023.02.03 22:08
// describe:可序列化字典实现
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework.Core.Container {
    [Serializable]
    public class SerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
        [SerializeField] List<TKey> keys = new List<TKey>();
        [SerializeField] List<TValue> values = new List<TValue>();

        // OnBeforeSerialize implementation.
        public void OnBeforeSerialize() {
            keys.Clear();
            values.Clear();
            foreach (var kvp in this) {
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }
        }

        // OnAfterDeserialize implementation
        public void OnAfterDeserialize() {
            for (int i = 0; i < keys.Count; i++) {
                Add(keys[i], values[i]);
            }
            keys.Clear();
            values.Clear();
        }
    }
}