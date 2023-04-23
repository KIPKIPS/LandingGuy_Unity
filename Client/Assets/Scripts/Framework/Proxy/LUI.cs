// author:KIPKIPS
// date:2023.03.25 11:27
// describe:UI代理
using Framework.UI;

namespace Framework {
    // ReSharper disable InconsistentNaming
    public static class LUI {
        public static void Open(string pageName,dynamic options = null) {
            UIManager.Instance.Open(pageName,options);
        } 
        public static void Close(string pageName) {
            UIManager.Instance.Close(pageName);
        } 
    }
}