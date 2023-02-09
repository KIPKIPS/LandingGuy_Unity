// author:KIPKIPS
// date:2023.02.02 22:21
// describe:可替换单例类,可被新创建单例替换
using UnityEngine;

namespace Framework {
    /// <summary>
    /// 如果跳转到新的场景里已经有了实例,则删除已有示例,再创建新的实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReplaceableMonoSingleton<T> : MonoBehaviour where T : Component {
        private static T _instance;
        public float initializationTime;

        /// <summary>
        /// Singleton design pattern
        /// </summary>
        public static T Instance {
            get {
                if (_instance == null) {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null) {
                        GameObject obj = new GameObject();
                        obj.hideFlags = HideFlags.HideAndDontSave;
                        _instance = obj.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }
        
        protected virtual void Awake() {
            if (!Application.isPlaying) return;
            initializationTime = Time.time;
            DontDestroyOnLoad(gameObject);
            T[] check = FindObjectsOfType<T>();
            foreach (T searched in check) {
                if (searched != this) {
                    if (searched.GetComponent<ReplaceableMonoSingleton<T>>().initializationTime < initializationTime) {
                        Destroy(searched.gameObject);
                    }
                }
            }
            _instance ??= this as T;
        }
    }
}