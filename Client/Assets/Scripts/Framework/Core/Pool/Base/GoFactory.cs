// author:KIPKIPS
// date:2023.02.03 15:48
// describe:GameObject工厂
using Unity.VisualScripting;
using UnityEngine;

namespace Framework.Core {
    public class GoFactory<T> : IFactory<T> where T : Object {
        private readonly T _prefab;
        private readonly Transform _root;
        /// <summary>
        /// 创建实例方法
        /// </summary>
        /// <returns></returns>
        public T Create() {
            var g = GameObject.Instantiate(_prefab, _root);
            Transform t = g.GameObject().transform;
            t.localScale = Vector3.one;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            return g;
        }
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="prefab">需要创建的预制体</param>
        /// <param name="root">实例的根节点</param>
        public GoFactory(T prefab,Transform root) {
            _prefab = prefab;
            _root = root;
        }
    }
}