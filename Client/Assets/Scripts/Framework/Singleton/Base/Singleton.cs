// author:KIPKIPS
// date:2023.02.02 22:22
// describe:普通单例类
namespace Framework.Singleton {
    // ReSharper disable MemberCanBeMadeStatic.Global
    public abstract class Singleton<T> : ISingleton where T : Singleton<T> {
        /// <summary>
        /// 静态实例
        /// </summary>
        private static T _instance;
        // 标签锁：确保当一个线程位于代码的临界区时，另一个线程不进入临界区。
        // 如果其他线程试图进入锁定的代码，则它将一直等待（即被阻止），直到该对象被释放
        // ReSharper disable once StaticMemberInGenericType
        private static readonly object Lock = new ();

        /// <summary>
        /// 静态单例对象
        /// </summary>
        public static T Instance {
            get {
                lock (Lock) {
                    _instance ??= SingletonCreator.CreateSingleton<T>();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 资源释放
        /// </summary>
        public virtual void Dispose() => _instance = null;

        /// <summary>
        /// 单例初始化方法
        /// </summary>
        public virtual void Initialize() {
        }
    }
}