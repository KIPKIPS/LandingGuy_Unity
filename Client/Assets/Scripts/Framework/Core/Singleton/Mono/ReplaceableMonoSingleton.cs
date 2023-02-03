// author:KIPKIPS
// date:2023.02.02 22:21
// describe:可替换单例类,可被新创建单例替换
using UnityEngine;

namespace Framework.Core.Singleton {
    /// <summary>
    /// 如果跳转到新的场景里已经有了实例,则删除已有示例,再创建新的实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReplaceableMonoSingleton<T> : MonoBehaviour where T : Component {
        protected static T _instance;
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

        // On awake, we check if there's already a copy of the object in the scene. If there's one, we destroy it.
        protected virtual void Awake() {
            if (!Application.isPlaying) return;
            initializationTime = Time.time;
            DontDestroyOnLoad(gameObject);
            // we check for existing objects of the same type
            T[] check = FindObjectsOfType<T>();
            foreach (T searched in check) {
                if (searched != this) {
                    // if we find another object of the same type (not this), and if it's older than our current object, we destroy it.
                    if (searched.GetComponent<ReplaceableMonoSingleton<T>>().initializationTime < initializationTime) {
                        Destroy(searched.gameObject);
                    }
                }
            }
            _instance ??= this as T;
        }
    }
}