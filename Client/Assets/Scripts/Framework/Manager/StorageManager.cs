// author:KIPKIPS
// date:2023.02.11 00:30
// describe:本地化存储器

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Framework.Singleton;

namespace Framework.Manager {
    public class StorageManager : Singleton<StorageManager> {
        private readonly string _logTag = "StorageManager";
        private BinaryFormatter _binaryFormatter = new ();
        public void Launch() {
            Utils.Log(_logTag, "the local data is loaded");
            var path = DEF.DataStoragePath;
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
            FileStream f = new FileStream(path,FileMode.OpenOrCreate);
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
            FileStream f = new FileStream(path,FileMode.Open);
            var obj = _binaryFormatter.Deserialize(f) as T;//二进制的方式把对象保存进文件
            f.Dispose();
            return obj;
        }

        public void SaveFileToStorage(object obj, string fileName) {
            SaveFile(obj,DEF.DataStoragePath + fileName);
        }
        
        public T LoadFileAtStorage<T>(string fileName) where T : class {
            return LoadFile<T>(DEF.DataStoragePath + fileName);
        }

        #endregion
    }
}