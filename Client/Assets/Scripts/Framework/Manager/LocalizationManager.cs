// author:KIPKIPS
// date:2023.02.11 21:34
// describe:本地化管理器
using Framework.Singleton;
namespace Framework.Manager {
    // ReSharper disable ClassNeverInstantiated.Global
    // ReSharper disable MemberCanBeMadeStatic.Local
    // ReSharper disable MemberCanBeMadeStatic.Global
    public class LocalizationManager: Singleton<LocalizationManager> {
        private const string LOGTag = "LocalizationManager";
        public void Launch() {
            Utils.Log(LOGTag,"localization manager is start");
        }
    }
}