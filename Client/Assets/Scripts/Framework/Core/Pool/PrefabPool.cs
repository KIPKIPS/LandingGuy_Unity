// author:KIPKIPS
// date:2023.02.03 14:19
// describe:针对GameObject的对象池
using Unity.VisualScripting;
using UnityEngine;

namespace Framework.Core.Pool {
    public class PrefabPool<T> : GoPool<T> where T : Object,IPoolAble {
        public PrefabPool(T prefab,Transform root) {
            _factory = new GoFactory<T>(prefab,root);
        }
        public override T Allocate() {
            T result = base.Allocate();
            result.GameObject().SetActive(true);
            result.IsRecycled = false;
            return result;
        }
        public override bool Recycle(T obj) {
            if (obj == null || obj.IsRecycled) {
                return false;
            }
            obj.GameObject().SetActive(false);
            obj.IsRecycled = true;
            obj.OnRecycled();
            _cacheStack.Push(obj);
            return true;
        }
    }
}