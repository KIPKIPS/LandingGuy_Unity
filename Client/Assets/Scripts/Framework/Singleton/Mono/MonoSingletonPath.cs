// author:KIPKIPS
// date:2023.02.02 22:20
// describe:mono单例路径
using System;
namespace Framework {
    /// <summary>
    /// MonoSingleton路径
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)] //这个特性只能标记在Class上
    public class MonoSingletonPath : Attribute {
        private readonly string _pathInHierarchy;
        public MonoSingletonPath(string pathInHierarchy) => _pathInHierarchy = pathInHierarchy;
        public string PathInHierarchy => _pathInHierarchy;
    }
}