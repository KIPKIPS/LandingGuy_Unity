// author:KIPKIPS
// date:2023.04.08 20:01
// describe:UI框架管理器
using Framework.Singleton;
namespace Framework.UI {
    public class UIManager :Singleton<UIManager> {
        public void Launch() {
            LoadConfig();
        }
        private void LoadConfig() {
            
        }
    }
}