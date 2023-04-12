﻿// --[[
//     author:{wkp}
//     time:20:45
// ]]
using System;
using System.Collections;
using Framework;
using Framework.UI;
using UnityEngine.UI;

namespace GamePlay.UI {
    public class TempPage:BasePage {
        protected override void OnEnter() {
            Bind(Find<Button>("_CONTENT_/Button"),()=> {
                UIProxy.Close(Config.pageName);
                void CloseCallback() {
                    Utils.Log("close callback");
                }
                UIProxy.Open("TempPage2",(Action)CloseCallback);
            });
        }
    }
}