// author:KIPKIPS
// date:2023.04.13 17:30
// describe:玩家创角界面
using System;
using Framework.UI;
using UnityEngine;

namespace GamePlay.UI {
    public class CreateRolePage:BasePage {
        [Binder(typeof(LText)),HideInInspector] public string desc;
        [Binder(typeof(LImage)),HideInInspector] public string image;

        protected override void OnEnter() {
            
        }
    }
}