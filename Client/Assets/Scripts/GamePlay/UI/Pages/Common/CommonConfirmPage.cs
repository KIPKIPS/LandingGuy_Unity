// author:KIPKIPS
// date:2023.04.25 16:11
// describe:玩家创角界面
using System;
using Framework;
using Framework.UI;
using UnityEngine;
using UnityEngine.Events;

namespace GamePlay.UI {
    public class CommonConfirmPage:BasePage {
        private Action _confirmCallback;
        protected override void Values() {
            DOBind("desc");
        }
        protected override void Methods() {
            DOBind("cancelBtn",OnCancel);
            DOBind("confirmBtn",OnConfirm);
        }
        private void OnCancel() {
            LUI.Close("CommonConfirmPage");
        }
        private void OnConfirm() {
            LUI.Close("CommonConfirmPage");
            _confirmCallback?.Invoke();
        }
        public override void OnEnter(dynamic options) {
            Bind("desc", options?["desc"]);
            _confirmCallback = options?["confirmCallback"];
        }
    }
}