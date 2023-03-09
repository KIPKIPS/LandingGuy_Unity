// author:KIPKIPS
// date:2023.02.02 22:20
// describe:mono单例路径
using System;
namespace Framework.Singleton {
    /// <summary>
    /// MonoSingleton路径
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)] //这个特性只能标记在Class上
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MonoSingletonPath : Attribute {
        public MonoSingletonPath(string pathInHierarchy) => PathInHierarchy = pathInHierarchy;
        public string PathInHierarchy { get; }
    }
}