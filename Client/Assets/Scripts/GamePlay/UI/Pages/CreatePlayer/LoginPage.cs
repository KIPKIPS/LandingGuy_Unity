// author:KIPKIPS
// date:2023.04.25 10:45
// describe:玩家创角界面
using System;
using System.Collections.Generic;
using System.Dynamic;
using Framework;
using Framework.Container;
using Framework.UI;
using UnityEngine;

namespace GamePlay.UI {
    public class LoginPage:BasePage {
        protected override void Values() {
            DOBind<string>("gameName");
        }
        protected override void Methods() {
            DOBind("onNewBtn",() => {
                // void Confirm() {
                //     LUI.Close("LoginPage");
                //     LUI.Open("CreateRolePage");
                // }
                // LUI.Open("CommonConfirmPage",new Table {
                //     ["desc"] = "确定创建新游戏吗?",
                //     ["confirmCallback"] = (Action)Confirm
                // });
            });
            DOBind("onContinueBtn",() => LUtil.Log("onContinueBtn"));
            DOBind("onSettingBtn",() => LUtil.Log("onSettingBtn"));
            DOBind("onExitBtn",() => LUI.Close("LoginPage"));

            dynamic o = new ExpandoObject();
            o.desc = "??";
            Debug.Log(o.desc);
            Debug.Log(o["desc"]);
        }
        public override void OnEnter() {
            Bind("gameName","Landing Guy");
        }
    }
}