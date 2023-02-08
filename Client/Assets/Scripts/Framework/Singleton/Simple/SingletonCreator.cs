// author:KIPKIPS
// date:2023.02.02 22:22
// describe:单例创建器
using UnityEngine;
using System.Reflection;
using System;

namespace Framework {
    // 普通单例创建类
    internal static class SingletonCreator {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <returns>创建的实例对象</returns>
        /// <exception cref="Exception"></exception>
        static T CreateNonPublicConstructorObject<T>() where T : class {
            var type = typeof(T);
            //获取私有构造函数
            var constructorInfos = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            // 获取无参构造函数
            var ctor = Array.Find(constructorInfos, c => c.GetParameters().Length == 0);
            if (ctor == null) {
                throw new Exception("Non-Public Constructor() not found! in " + type);
            }
            return ctor.Invoke(null) as T;
        }
        /// <summary>
        /// 创建单例对象
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <returns>创建的实例对象</returns>
        public static T CreateSingleton<T>() where T : class, ISingleton {
            var type = typeof(T);
            var monoBehaviourType = typeof(MonoBehaviour);
            if (monoBehaviourType.IsAssignableFrom(type)) {
                return CreateMonoSingleton<T>();
            } else {
                var instance = CreateNonPublicConstructorObject<T>();
                instance.Initialize();
                return instance;
            }
        }

        /// <summary>
        /// 单元测试模式 标签
        /// </summary>
        public static bool IsUnitTestMode { get; set; }

        /// <summary>
        /// 查找Obj（一个嵌套查找Obj的过程）
        /// </summary>
        /// <param name="root"></param>
        /// <param name="subPath"></param>
        /// <param name="index"></param>
        /// <param name="build"></param>
        /// <param name="dontDestroy"></param>
        /// <returns></returns>
        private static GameObject FindGameObject(GameObject root, string[] subPath, int index, bool build, bool dontDestroy) {
            GameObject client = null;
            if (root == null) {
                client = GameObject.Find(subPath[index]);
            } else {
                var child = root.transform.Find(subPath[index]);
                if (child != null) {
                    client = child.gameObject;
                }
            }
            if (client == null) {
                if (build) {
                    client = new GameObject(subPath[index]);
                    if (root != null) {
                        client.transform.SetParent(root.transform);
                    }
                    if (dontDestroy && index == 0 && !IsUnitTestMode) {
                        GameObject.DontDestroyOnLoad(client);
                    }
                }
            }
            if (client == null) {
                return null;
            }
            return ++index == subPath.Length ? client : FindGameObject(client, subPath, index, build, dontDestroy);
        }

        /// <summary>
        /// 泛型方法：创建MonoBehaviour单例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateMonoSingleton<T>() where T : class, ISingleton {
            T instance = null;
            var type = typeof(T);

            //判断T实例存在的条件是否满足
            if (!IsUnitTestMode && !Application.isPlaying)
                return instance;

            //判断当前场景中是否存在T实例
            instance = UnityEngine.Object.FindObjectOfType(type) as T;
            if (instance != null) {
                instance.Initialize();
                return instance;
            }

            //MemberInfo：获取有关成员属性的信息并提供对成员元数据的访问
            MemberInfo info = typeof(T);
            //获取T类型 自定义属性，并找到相关路径属性，利用该属性创建T实例
            var attributes = info.GetCustomAttributes(true);
            foreach (var attribute in attributes) {
                var defineAttr = attribute as MonoSingletonPath;
                if (defineAttr == null) {
                    continue;
                }
                instance = CreateComponentOnGameObject<T>(defineAttr.PathInHierarchy, true);
                break;
            }

            //如果还是无法找到instance  则主动去创建同名Obj 并挂载相关脚本 组件
            if (instance == null) {
                var obj = new GameObject(typeof(T).Name);
                if (!IsUnitTestMode)
                    UnityEngine.Object.DontDestroyOnLoad(obj);
                instance = obj.AddComponent(typeof(T)) as T;
            }
            instance.Initialize();
            return instance;
        }

        /// <summary>
        /// 在GameObject上创建T组件（脚本）
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dontDestroy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static T CreateComponentOnGameObject<T>(string path, bool dontDestroy) where T : class {
            var obj = FindGameObject(path, true, dontDestroy);
            if (obj == null) {
                obj = new GameObject("Singleton of " + typeof(T).Name);
                if (dontDestroy && !IsUnitTestMode) {
                    UnityEngine.Object.DontDestroyOnLoad(obj);
                }
            }
            return obj.AddComponent(typeof(T)) as T;
        }

        /// <summary>
        /// 查找Obj（对于路径 进行拆分）
        /// </summary>
        /// <param name="path"></param>
        /// <param name="build"></param>
        /// <param name="dontDestroy"></param>
        /// <returns></returns>
        private static GameObject FindGameObject(string path, bool build, bool dontDestroy) {
            if (string.IsNullOrEmpty(path)) {
                return null;
            }
            var subPath = path.Split('/');
            if (subPath == null || subPath.Length == 0) {
                return null;
            }
            return FindGameObject(null, subPath, 0, build, dontDestroy);
        }
    }
}