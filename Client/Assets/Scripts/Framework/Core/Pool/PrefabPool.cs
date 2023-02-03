// author:KIPKIPS
// date:2023.02.03 14:19
// describe:针对GameObject的对象池
using Framework.Core.Singleton;
using UnityEngine;
using UnityEngine.Rendering;

namespace Framework.Core.Pool {
    public class PrefabPool<T> : GoPool<T> where T : UnityEngine.Object,IPoolAble {
        public void Initialize() {
        }
        public PrefabPool(T prefab) {
            _factory = new GoFactory<T>(prefab);
        }
        public override T Allocate() {
            T result = base.Allocate();
            result.IsRecycled = false;
            return result;
        }
        public override bool Recycle(T obj) {
            if (obj == null || obj.IsRecycled) {
                return false;
            }
            obj.IsRecycled = true;
            obj.OnRecycled();
            _cacheStack.Push(obj);
            return true;
        }
    }
}