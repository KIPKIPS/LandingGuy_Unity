// --[[
//     author:{wkp}
//     time:11:27
// ]]
using Framework.UI;

namespace Framework {
    public static class LUI {
        public static void Open(string pageName,dynamic options = null) {
            UIManager.Instance.Open(pageName,options);
        } 
        public static void Close(string pageName) {
            UIManager.Instance.Close(pageName);
        } 
    }
}