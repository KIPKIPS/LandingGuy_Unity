// author:KIPKIPS
// date:2023.02.03 15:41
// describe:对象池类
using System.Collections.Generic;

namespace Framework.Core.Pool {
    public abstract class GoPool<T> : IPool<T> {
        protected IFactory<T> _factory; //定义实现接口的类对象
        protected Stack<T> _cacheStack = new Stack<T>();
        
        public virtual T Allocate() {
            return _cacheStack.Count == 0 ? _factory.Create() : _cacheStack.Pop();
        }
        public abstract bool Recycle(T o);
    }
}