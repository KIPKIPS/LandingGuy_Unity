// author:KIPKIPS
// date:2023.04.13 17:30
// describe:玩家创角界面
using Framework;
using Framework.UI;
using UnityEngine;
using UnityEngine.Profiling;

namespace GamePlay.UI {
    public class CreateRolePage : BasePage {
        protected override void Values() {
            Profiler.BeginSample("BindField");
            DOBind<string>("text");
            DOBind<bool>("textEnable");
            DOBind<int>("textFont");
            DOBind<Color>("imageColor");
            DOBind<Color>("textColor");
            DOBind<Vector2>("pos");
            DOBind<bool>("active");
            Profiler.EndSample();
        }
        protected override void Methods() {
            Profiler.BeginSample("BindMethods");
            DOBind("closeBtn",() => LUI.Close("CreateRolePage"));
            DOBind("onLast",() => LUtil.Log("onLast"));
            DOBind("onNext",() => LUtil.Log("onNext"));
            Profiler.EndSample();
        }
        protected override void OnEnter() {
            Profiler.BeginSample("BindUpdate");
            Bind("text", "wkp");
            Bind("textEnable", true);
            Bind("textFont", 130);
            Bind("imageColor", Color.red);
            Bind("textColor", Color.blue);
            
            Bind("pos", Vector2.one * 100);
            Bind("active", false);
            Profiler.EndSample();
        }
    }
}