// author:KIPKIPS
// date:2023.04.30 23:46
// describe:
using System.Collections.Generic;
using Framework.Pool;
using Framework.Singleton;

namespace Framework.Manager {
    public class PoolManager : Singleton<PoolManager> {
        private readonly Dictionary<string, dynamic> _poolDict = new();
        public T Allocate<T>() where T : IPoolAble, new() {
            var key = typeof(T).FullName ?? typeof(T).Name;
            if (!_poolDict.ContainsKey(key)) {
                _poolDict.Add(key,ExclusivePool<T>.Instance);
            }
            return _poolDict[key].Allocate();
        }
        
        public bool Recycle(object obj) {
            var key = obj.GetType().FullName ?? obj.GetType().Name;
            return _poolDict.ContainsKey(key) && (bool)_poolDict[key].Recycle();
        }
    }
}