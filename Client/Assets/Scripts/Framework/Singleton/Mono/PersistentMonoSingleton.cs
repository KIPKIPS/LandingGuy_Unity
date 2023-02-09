// author:KIPKIPS
// date:2023.02.02 22:21
// describe:唯一单例,一经创建不再销毁
using UnityEngine;

namespace Framework {
    /// <summary>
    /// 如果跳转到新的场景里已经有了实例,则不创建新的单例(或者创建新的单例后会销毁掉新的单例)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PersistentMonoSingleton<T> : MonoBehaviour,ISingleton where T : PersistentMonoSingleton<T> {
        private static T _instance;
        private bool _enabled;
        private static bool _onApplicationQuit;
        public static T Instance {
            get {
                if (_instance == null && !_onApplicationQuit) {
                    _instance = SingletonCreator.CreateMonoSingleton<T>();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 应用程序退出：释放当前对象并销毁相关GameObject
        /// </summary>
        protected virtual void OnApplicationQuit() {
            _onApplicationQuit = true;
        }
        /// <summary>
        /// 实现接口的单例初始化
        /// </summary>
        public virtual void Initialize() {
        }
        
        public static bool IsApplicationQuit => _onApplicationQuit;

        protected virtual void Awake() {
            if (!Application.isPlaying) return;
            if (_instance == null) {
                //If I am the first instance, make me the Singleton
                _instance = this as T;
                DontDestroyOnLoad(transform.gameObject);
                _enabled = true;
            } else {
                if (this != _instance) {
                    Destroy(gameObject);
                }
            }
        }
    }
}