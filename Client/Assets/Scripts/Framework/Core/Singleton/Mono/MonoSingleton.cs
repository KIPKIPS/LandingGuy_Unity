// author:KIPKIPS
// date:2023.02.02 22:19
// describe:mono单例类
using UnityEngine;

namespace Framework.Core.Singleton {
    // 静态类：MonoBehaviour类的单例
    // 泛型类：Where约束表示T类型必须继承MonoSingleton<T>
    public abstract class MonoSingleton<T> : MonoBehaviour, ISingleton where T : MonoSingleton<T> {
        // 静态实例
        protected static T _instance;

        // 静态属性：封装相关实例对象
        public static T Instance {
            get {
                if (_instance == null && !_onApplicationQuit) {
                    _instance = SingletonCreator.CreateMonoSingleton<T>();
                }
                return _instance;
            }
        }

        // 实现接口的单例初始化
        public virtual void Initialize() {
        }

        // 资源释放
        public virtual void Dispose() {
            if (SingletonCreator.IsUnitTestMode) {
                var curTrans = transform;
                do {
                    var parent = curTrans.parent;
                    DestroyImmediate(curTrans.gameObject);
                    curTrans = parent;
                } while (curTrans != null);
                _instance = null;
            } else {
                Destroy(gameObject);
            }
        }

        // 当前应用程序是否结束 标签
        protected static bool _onApplicationQuit = false;

        // 应用程序退出：释放当前对象并销毁相关GameObject
        protected virtual void OnApplicationQuit() {
            _onApplicationQuit = true;
            if (_instance == null) return;
            Destroy(_instance.gameObject);
            _instance = null;
        }

        // 释放当前对象
        protected virtual void OnDestroy() {
            _instance = null;
        }

        // 判断当前应用程序是否退出
        public static bool IsApplicationQuit {
            get => _onApplicationQuit;
        }
    }
}