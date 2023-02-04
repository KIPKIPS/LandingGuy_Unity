// author:KIPKIPS
// date:2023.02.03 14:19
// describe:针对GameObject的对象池
using Unity.VisualScripting;
using UnityEngine;

namespace Framework.Core {
    /// <summary>
    /// 预制体对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PrefabPool<T> : GoPool<T> where T : Object,IPoolAble {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="prefab">实例化的预制体</param>
        /// <param name="root">创建实例的根节点</param>
        public PrefabPool(T prefab,Transform root) {
            _factory = new GoFactory<T>(prefab,root);
        }
        /// <summary>
        /// 分配函数
        /// </summary>
        /// <returns>分配的对象</returns>
        public override T Allocate() {
            T result = base.Allocate();
            result.GameObject().SetActive(true);
            result.IsRecycled = false;
            return result;
        }
        /// <summary>
        /// 回收函数
        /// </summary>
        /// <param name="obj">回收的对象</param>
        /// <returns>是否回收成功</returns>
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