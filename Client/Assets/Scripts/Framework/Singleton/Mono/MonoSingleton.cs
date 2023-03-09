// author:KIPKIPS
// date:2023.02.02 22:19
// describe:mono单例类
using UnityEngine;

namespace Framework.Singleton {
    /// <summary>
    /// 静态类：MonoBehaviour类的单例
    /// 泛型类：Where约束表示T类型必须继承MonoSingleton
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviour, ISingleton where T : MonoSingleton<T> {
        /// <summary>
        /// 静态实例
        /// </summary>
        private static T _instance;

        /// <summary>
        /// 静态属性：封装相关实例对象
        /// </summary>
        public static T Instance {
            get {
                if (_instance == null && !_onApplicationQuit) {
                    _instance = SingletonCreator.CreateMonoSingleton<T>();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 实现接口的单例初始化
        /// </summary>
        public virtual void Initialize() {
        }

        /// <summary>
        /// 资源释放
        /// </summary>
        public virtual void Dispose() {
            Destroy(gameObject);
        }

        /// <summary>
        /// 当前应用程序是否结束 标签
        /// </summary>
        // ReSharper disable once StaticMemberInGenericType
        private static bool _onApplicationQuit;

        /// <summary>
        /// 应用程序退出：释放当前对象并销毁相关GameObject
        /// </summary>
        protected virtual void OnApplicationQuit() {
            _onApplicationQuit = true;
            if (_instance == null) return;
            Destroy(_instance.gameObject);
            _instance = null;
        }

        /// <summary>
        /// 释放当前对象
        /// </summary>
        protected virtual void OnDestroy() => _instance = null;
    }
}