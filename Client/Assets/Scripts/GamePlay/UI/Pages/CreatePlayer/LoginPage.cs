// author:KIPKIPS
// date:2023.04.25 10:45
// describe:玩家创角界面
using Framework;
using Framework.UI;
using UnityEngine;

namespace GamePlay.UI {
    public class LoginPage:BasePage {
        protected override void Values() {
            DOBind<string>("gameName");
        }
        protected override void Methods() {
            DOBind("onNewBtn",() => {
                LUI.Close("LoginPage");
                LUI.Open("CreateRolePage");
            });
            DOBind("onContinueBtn",() => LUtil.Log("onContinueBtn"));
            DOBind("onSettingBtn",() => LUtil.Log("onSettingBtn"));
            DOBind("onExitBtn",() => LUI.Close("LoginPage"));
        }
        protected override void OnEnter() {
            Bind("gameName","Landing Guy");
        }
    }
}