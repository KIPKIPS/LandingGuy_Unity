// author:KIPKIPS
// date:2023.02.03 10:05
// describe:单例对象池
using System;
namespace Framework.Core {
    
    /// <summary>
    /// 数量限制池对象容器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservePool<T> : BasePool<T>, ISingleton where T : IPoolAble, new() {
        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize() {
        }
        /// <summary>
        /// 构造器
        /// </summary>
        protected ObservePool() {
            _factory = new BaseFactory<T>();
        }
        /// <summary>
        /// 单例池容器
        /// </summary>
        public static ObservePool<T> Instance => SingletonProperty<ObservePool<T>>.Instance;
        
        /// <summary>
        /// 析构函数
        /// </summary>
        public void Dispose() {
            SingletonProperty<ObservePool<T>>.Dispose();
        }
        
        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <param name="maxCount">最大数量</param>
        /// <param name="initCount">初始化数量</param>
        public void Init(int maxCount, int initCount) {
            if (maxCount > 0) {
                initCount = Math.Min(maxCount, initCount);
                MaxCount = maxCount;
            }
            if (CurCount < initCount) {
                for (int i = CurCount; i < initCount; ++i) {
                    Recycle(_factory.Create());
                }
            }
        }
        
        /// <summary>
        /// 最大缓存数量
        /// </summary>
        public int MaxCacheCount {
            get => MaxCount;
            set {
                MaxCount = value;
                if (_cacheStack != null && MaxCount > 0 && MaxCount < _cacheStack.Count) {
                    int removeCount = MaxCount - _cacheStack.Count;
                    while (removeCount > 0) {
                        _cacheStack.Pop();
                        --removeCount;
                    }
                }
            }
        }
        
        /// <summary>
        /// 分配实例
        /// </summary>
        /// <returns>实例对象</returns>
        public override T Allocate() {
            T result = base.Allocate();
            result.IsRecycled = false;
            return result;
        }

        /// <summary>
        /// 回收实例
        /// </summary>
        /// <param name="obj">回收对象</param>
        /// <returns>是否回收成功</returns>
        public override bool Recycle(T obj) {
            if (obj == null || obj.IsRecycled) {
                return false;
            }
            if (MaxCount > 0 && _cacheStack.Count >= MaxCount) {
                obj.OnRecycled();
                return false;
            }
            obj.IsRecycled = true;
            obj.OnRecycled();
            _cacheStack.Push(obj);
            return true;
        }
    }
}