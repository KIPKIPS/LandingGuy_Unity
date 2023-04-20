// author:KIPKIPS
// date:2023.04.13 17:30
// describe:玩家创角界面
using System;
using Framework.UI;
using UnityEngine;

namespace GamePlay.UI {
    public class CreateRolePage:BasePage {
        //todo:简化
        private Bindable<string> text = new(0,"text");
        private Bindable<bool> textEnable = new(0,"textEnable");
        private Bindable<int> textFont = new(0,"textFont");
        private Bindable<Color> imageColor = new(0,"imageColor");
        private Bindable<Color> textColor = new(0,"textColor");
        protected override void OnEnter() {
            text.Value = "wkp";
            textEnable.Value = true;
            textFont.Value = 130;
            imageColor.Value = Color.red;
            textColor.Value = Color.blue;
        }
    }
}