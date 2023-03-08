// author:KIPKIPS
// date:2023.02.02 22:21
// describe:mono单例属性
using UnityEngine;

namespace Framework.Singleton {
    /// <summary>
    /// 继承Mono的属性单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class MonoSingletonProperty<T> where T : MonoBehaviour, ISingleton {
        private static T _instance;
        public static T Instance => _instance ??= SingletonCreator.CreateMonoSingleton<T>();
        public static void Dispose() {
            Object.Destroy(_instance.gameObject);
            _instance = null;
        }
    }
}