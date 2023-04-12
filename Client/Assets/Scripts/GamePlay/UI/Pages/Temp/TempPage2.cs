// --[[
//     author:{wkp}
//     time:20:45
// ]]
using System;
using Framework;
using Framework.UI;
using UnityEditor;
using UnityEngine.UI;

namespace GamePlay.UI {
    public class TempPage2:BasePage {
        private Action closeCallback;
        public override void OnEnter(dynamic options) {
            closeCallback = options;
            Bind(Find<Button>("_CONTENT_/Button"),()=> {
                UIProxy.Close(Config.pageName);
            });
        }
        public override void OnExit() {
            closeCallback?.Invoke();
        }
    }
}