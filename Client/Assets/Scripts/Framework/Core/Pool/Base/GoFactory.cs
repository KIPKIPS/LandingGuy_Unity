// author:KIPKIPS
// describe:GameObject工厂
using UnityEngine;

namespace Framework.Core.Pool {
    public class GoFactory<T> : IFactory<T> where T : UnityEngine.Object {
        private T _prefab;
        public T Create() {
            return GameObject.Instantiate<T>(_prefab);
        }
        public GoFactory(T prefab) {
            _prefab = prefab;
        }
    }
}