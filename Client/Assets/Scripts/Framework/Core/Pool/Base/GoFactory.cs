// author:KIPKIPS
// date:2023.02.03 15:48
// describe:GameObject工厂
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace Framework.Core.Pool {
    public class GoFactory<T> : IFactory<T> where T : Object {
        private T _prefab;
        private Transform _root;
        public T Create() {
            var g = GameObject.Instantiate(_prefab, _root);
            Transform t = g.GameObject().transform;
            t.localScale = Vector3.one;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            return g;
        }
        public GoFactory(T prefab,Transform root) {
            _prefab = prefab;
            _root = root;
        }
    }
}