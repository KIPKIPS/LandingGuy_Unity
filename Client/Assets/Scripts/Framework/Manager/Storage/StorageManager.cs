// author:KIPKIPS
// date:2023.02.11 00:30
// describe:本地化存储器
namespace Framework.Manager {
    [MonoSingletonPath("StorageManager")]
    public class StorageManager : PersistentMonoSingleton<StorageManager> {
        private readonly string _logTag = "StorageManager";
        public new void  Initialize() {
            Utils.Log(_logTag,"the local data is loaded");
        }
    }
}