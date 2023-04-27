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
            DOBind<string>("profession1");
            DOBind<string>("profession2");
            DOBind<string>("profession3");
            DOBind<string>("profession4");
        }
        protected override void Methods() {
            DOBind<UnityAction>("closeBtn", () => {
                LUI.Close("CreateRolePage");
                LUI.Open("LoginPage");
            });
            DOBind<UnityAction>("onLast",() => LUtil.Log("onLast"));
            DOBind<UnityAction>("onNext",() => LUtil.Log("onNext"));
            DOBind<UnityAction<Vector2>>("onDragBegin",pos => LUtil.Log("onDragBegin",pos));
            DOBind<UnityAction<Vector2>>("onDrag",pos => LUtil.Log("onDrag",pos));
            DOBind<UnityAction<Vector2>>("onDragEnd",pos => LUtil.Log("onDragEnd",pos));
        }
        public override void OnEnter() {
            LUtil.Log("CreateRolePage");
            Bind("profession1", "战士");
            Bind("profession2", "刺客");
            Bind("profession3", "坦克");
            Bind("profession4", "射手");
        }
    }
}