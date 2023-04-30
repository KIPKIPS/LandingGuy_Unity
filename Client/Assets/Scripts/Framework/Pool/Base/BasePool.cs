// author:KIPKIPS
// date:2023.02.03 13:49
// describe:对象池类
using System;
using System.Collections.Generic;

namespace Framework.Pool {
    public abstract class BasePool<T> : IPool<T> {
        // Gets the current count.
        protected int CurCount => CacheStack.Count;
        protected IFactory<T> Factory; //定义实现接口的类对象
        protected readonly Stack<T> CacheStack = new();

        // default is 5
        protected int MaxCount = 5;
        /// <summary>
        /// 请求分配对象
        /// </summary>
        /// <returns></returns>
        public virtual T Allocate() => CacheStack.Count == 0 ? Factory.Create() : CacheStack.Pop();
        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="obj">放入池中的对象</param>
        /// <returns>是否回收成功</returns>
        public abstract bool Recycle(T obj);
    }
}