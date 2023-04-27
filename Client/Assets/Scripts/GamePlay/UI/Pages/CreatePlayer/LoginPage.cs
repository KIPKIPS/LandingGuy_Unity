// author:KIPKIPS
// date:2023.04.25 10:45
// describe:玩家创角界面
using System;
using System.Dynamic;
using Framework;
using Framework.Container;
using Framework.UI;
using UnityEngine;
using UnityEngine.Events;

namespace GamePlay.UI {
    public class LoginPage:BasePage {
        protected override void Values() {
            DOBind<string>("gameName");
        }
        private void Confirm() {
            LUI.Close("LoginPage");
            LUI.Open("CreateRolePage");
        }
        protected override void Methods() {
            DOBind<UnityAction>("onNewBtn",() => {
                LUI.Open("CommonConfirmPage",new Table {
                    ["desc"] = "确定创建新游戏吗?",
                    ["confirmCallback"] = (Action)Confirm
                });
            });
            DOBind<UnityAction>("onContinueBtn",() => LUtil.Log("onContinueBtn"));
            DOBind<UnityAction>("onSettingBtn",() => LUtil.Log("onSettingBtn"));
            DOBind<UnityAction>("onExitBtn",() => LUI.Close("LoginPage"));
        }
        public override void OnEnter() {
            Bind("gameName","Landing Guy");
        }
    }
}