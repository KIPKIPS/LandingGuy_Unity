// author:KIPKIPS
// date:2023.02.11 21:34
// describe:本地化管理器
using Framework.Singleton;
namespace Framework.Manager {
    public class LocalizationManager: Singleton<LocalizationManager> {
        private readonly string _logTag = "LocalizationManager";
        public void Launch() {
            Utils.Log(_logTag,"localization manager is start");
        }
    }
}