// author:KIPKIPS
// date:2023.02.02 22:23
// describe:普通单例属性
namespace Framework.Core.Singleton {
    // 属性单例类
    public static class SingletonProperty<T> where T : class, ISingleton {
        // 静态实例
        private static T _instance;

        // 标签锁
        private static readonly object _lock = new object();

        // 静态属性
        public static T Instance {
            get {
                lock (_lock) {
                    _instance = _instance ?? SingletonCreator.CreateSingleton<T>();
                }
                return _instance;
            }
        }
        //资源释放
        public static void Dispose() {
            _instance = null;
        }
    }
}