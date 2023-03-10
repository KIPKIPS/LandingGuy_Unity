// author:KIPKIPS
// date:2023.02.03 15:41
// describe:对象池类
using System.Collections.Generic;

namespace Framework.Pool {
    public abstract class GoPool<T> : IPool<T> {
        protected IFactory<T> Factory; //定义实现接口的类对象
        protected readonly Stack<T> CacheStack = new ();
        /// <summary>
        /// 请求分配对象
        /// </summary>
        /// <returns>实例对象</returns>
        public virtual T Allocate() => CacheStack.Count == 0 ? Factory.Create() : CacheStack.Pop();
        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="o">被回收的对象</param>
        /// <returns>是否回收成功</returns>
        public abstract bool Recycle(T o);
    }
}