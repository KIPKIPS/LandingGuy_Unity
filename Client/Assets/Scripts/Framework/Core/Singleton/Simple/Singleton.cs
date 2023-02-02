// author:KIPKIPS
// date:2023.02.02 22:22
// describe:普通单例类
namespace Framework.Core.Singleton {
    public abstract class Singleton<T> : ISingleton where T : Singleton<T> {
        // 静态实例
        protected static T _instance;

        // 标签锁：确保当一个线程位于代码的临界区时，另一个线程不进入临界区。
        // 如果其他线程试图进入锁定的代码，则它将一直等待（即被阻止），直到该对象被释放
        static object _lock = new object();

        // 静态属性
        public static T Instance {
            get {
                lock (_lock) {
                    _instance = _instance ?? SingletonCreator.CreateSingleton<T>();
                }
                return _instance;
            }
        }

        // 资源释放
        public virtual void Dispose() {
            _instance = null;
        }

        // 单例初始化方法
        public virtual void Initialize() {
        }
    }
}