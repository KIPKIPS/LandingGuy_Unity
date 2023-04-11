// --[[
//     author:{wkp}
//     time:20:45
// ]]
using Framework;
using Framework.UI;
using UnityEngine.UI;

namespace GamePlay.UI {
    public class TempPage:BasePage {
        public override void OnEnter() {
            Utils.Log("enter");
            Bind(Find<Button>("_CONTENT_/Button"),()=> {
                base.OnExit();
                Utils.Log("close");
            });
        }
    }
}