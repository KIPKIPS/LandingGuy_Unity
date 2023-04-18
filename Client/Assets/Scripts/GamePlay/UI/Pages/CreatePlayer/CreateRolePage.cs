// author:KIPKIPS
// date:2023.04.13 17:30
// describe:玩家创角界面
using System;
using Framework.UI;
using UnityEngine;

namespace GamePlay.UI {
    public class CreateRolePage:BasePage {
        //todo:简化
        private Bindable<string> desc = new(0,"desc");
        private Bindable<string> imageSize = new(0,"imageSize");

        protected override void OnEnter() {
            desc.Value = "wkp";
        }
    }
}