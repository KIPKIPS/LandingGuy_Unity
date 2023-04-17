// author:KIPKIPS
// date:2023.04.13 17:30
// describe:玩家创角界面
using System;
using Framework.UI;
using UnityEngine;

namespace GamePlay.UI {
    public class CreateRolePage:BasePage {
        private Bindable<string> desc;
        private Bindable<string> image;

        protected override void OnEnter() {
            desc.Value = "wkp";
        }
    }
}