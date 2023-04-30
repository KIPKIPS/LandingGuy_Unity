// author:KIPKIPS
// date:2023.02.03 10:05
// describe:单例对象池
using Framework.Singleton;
namespace Framework.Pool {
    
    /// <summary>
    /// 数量限制池对象容器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExclusivePool<T> : BasePool<T>, ISingleton where T : IPoolAble, new() {
        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize() {
        }
        /// <summary>
        /// 构造器
        /// </summary>
        public ExclusivePool() => Factory = new BaseFactory<T>();
        /// <summary>
        /// 单例池容器
        /// </summary>
        public static ExclusivePool<T> Instance => SingletonProperty<ExclusivePool<T>>.Instance;
        
        /// <summary>
        /// 析构函数
        /// </summary>
        public void Dispose() => SingletonProperty<ExclusivePool<T>>.Dispose();
        
        /// <summary>
        /// 分配实例
        /// </summary>
        /// <returns>实例对象</returns>
        public override T Allocate() {
            var result = base.Allocate();
            result.IsRecycled = false;
            return result;
        }

        /// <summary>
        /// 回收实例
        /// </summary>
        /// <param name="obj">回收对象</param>
        /// <returns>是否回收成功</returns>
        public override bool Recycle(T obj) {
            if (obj == null || obj.IsRecycled) return false;
            obj.IsRecycled = true;
            obj.OnRecycled();
            CacheStack.Push(obj);
            return true;
        }
    }
}