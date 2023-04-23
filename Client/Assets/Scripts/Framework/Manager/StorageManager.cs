// author:KIPKIPS
// date:2023.02.11 00:30
// describe:本地化存储器

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Framework.Singleton;

namespace Framework.Manager {
    // ReSharper disable ClassNeverInstantiated.Global
    public class StorageManager : Singleton<StorageManager> {
        private const string LOGTag = "StorageManager";
        private readonly BinaryFormatter _binaryFormatter = new ();
        // ReSharper disable once MemberCanBeMadeStatic.Global
        public void Launch() {
            LUtils.Log(LOGTag, "the local data is loaded");
            var path = LDefine.DataStoragePath;
                //不存在目录则创建
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
        }

        #region 工具函数

        /// <summary>
        /// 保存对象到指定路径
        /// </summary>
        /// <param name="obj">保存对象</param>
        /// <param name="path">保存路径</param>
        private void SaveFile(object obj,string path) {
            var f = new FileStream(path,FileMode.OpenOrCreate);
            _binaryFormatter.Serialize(f,obj);//二进制的方式把对象保存进文件
            f.Dispose();//释放文件流
            
        }
        /// <summary>
        /// 加载指定类型的文件
        /// </summary>
        /// <param name="path">加载文件路径</param>
        /// <typeparam name="T">加载的类型</typeparam>
        /// <returns>加载文件数据</returns>
        private T LoadFile<T>(string path) where T : class {
            if (!File.Exists(path)) {
                return null;
            }
            var f = new FileStream(path,FileMode.Open);
            var obj = _binaryFormatter.Deserialize(f) as T;//二进制的方式把对象保存进文件
            f.Dispose();
            return obj;
        }
        /// <summary>
        /// 存储到本地目录
        /// </summary>
        /// <param name="obj">存储的对象</param>
        /// <param name="fileName">文件名称</param>
        public void SaveFileToStorage(object obj, string fileName) => SaveFile(obj,LDefine.DataStoragePath + fileName);
        /// <summary>
        /// 从本地存储目录加载文件
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <typeparam name="T">数据类型</typeparam>
        /// <returns>文件序列化为指定类型的对象</returns>
        public T LoadFileAtStorage<T>(string fileName) where T : class => LoadFile<T>(LDefine.DataStoragePath + fileName);

        #endregion
    }
}