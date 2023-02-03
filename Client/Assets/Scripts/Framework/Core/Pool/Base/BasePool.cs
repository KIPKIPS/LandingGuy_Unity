// author:KIPKIPS
// date:2023.02.03 13:49
// describe:对象池类
using System.Collections.Generic;

namespace Framework.Core.Pool {
    public abstract class BasePool<T> : IPool<T> {
        // Gets the current count.
        protected int CurCount =>_cacheStack.Count;
        protected IFactory<T> _factory; //定义实现接口的类对象
        protected readonly Stack<T> _cacheStack = new Stack<T>();

        // default is 5
        protected int MaxCount = 5;
        public virtual T Allocate() {
            return _cacheStack.Count == 0 ? _factory.Create() : _cacheStack.Pop();
        }
        public abstract bool Recycle(T obj);
    }
}