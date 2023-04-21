// author:KIPKIPS
// date:2023.04.13 17:30
// describe:玩家创角界面
using System;
using Framework;
using Framework.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GamePlay.UI {
    public class CreateRolePage : BasePage {
        //todo:简化
        private Bindable<string> text;
        private Bindable<bool> textEnable;
        private Bindable<int> textFont;
        private Bindable<Color> imageColor;
        private Bindable<Color> textColor;
        private Bindable<UnityAction> button;
        public override void OnBind() {
            text = Bind<string>("text");
            textEnable = Bind<bool>("textEnable");
            textFont = Bind<int>("textFont");
            imageColor = Bind<Color>("imageColor");
            textColor = Bind<Color>("textColor");
            button = Bind<UnityAction>("button");
        }
        protected override void OnEnter() {
            text.Value = "wkp";
            textEnable.Value = true;
            textFont.Value = 130;
            imageColor.Value = Color.red;
            textColor.Value = Color.blue;
            button.Value = () => { Utils.Log("???"); };
        }
    }
}