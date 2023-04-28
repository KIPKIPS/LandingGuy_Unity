// author:KIPKIPS
// date:2023.04.13 17:30
// describe:玩家创角界面
using Framework;
using Framework.UI;
using UnityEngine;
using UnityEngine.Events;

namespace GamePlay.UI {
    public class CreateRolePage : BasePage {
        protected override void Values() {
            DOBind("profession1");
            DOBind("profession2");
            DOBind("profession3");
            DOBind("profession4");
            DOBind("modelPath");
        }
        protected override void Methods() {
            DOBind("closeBtn", () => {
                LUI.Close("CreateRolePage");
                LUI.Open("LoginPage");
            });
            DOBind("onLast",() => LUtil.Log("onLast"));
            DOBind("onNext",() => LUtil.Log("onNext"));
            DOBind("onDragBegin",pos => LUtil.Log("onDragBegin",pos));
            DOBind("onDrag",pos => LUtil.Log("onDrag",pos));
            DOBind("onDragEnd",pos => LUtil.Log("onDragEnd",pos));
        }
        public override void OnEnter() {
            LUtil.Log("CreateRolePage");
            Bind("profession1", "战士");
            Bind("profession2", "刺客");
            Bind("profession3", "坦克");
            Bind("profession4", "射手");
            
            Bind("modelPath","Assets/ResourcesAssets/Character/pre_liam.prefab");
        }
    }
}