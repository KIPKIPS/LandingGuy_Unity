// author:KIPKIPS
// date:2023.04.10 18:06
// describe:BasePage UI面板的基类
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Button = UnityEngine.UI.Button;
using UnityEngine.Events;

namespace Framework.UI {
    public class BasePage : MonoBehaviour {
        public PageConfig Config{ get; set; }
        public bool IsShow { get; private set; }
        private Transform _content;
        private Transform _bg;
        protected Transform Content => _content ??= Find<Transform>("_CONTENT_", transform);
        protected Transform Bg => _bg ??= Find<Transform>("_BG_", transform);
        private Canvas _canvas;
        public Canvas Canvas {
            get {
                _canvas ??= GetComponent<Canvas>();
                _canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.Tangent | AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.Normal;
                return _canvas;
            }
        }

        #region Find接口列表

        public T Find<T>(string namePath) {
            return Utils.Find<T>(transform, namePath);
        }
        public T Find<T>(string namePath, Action func) where T : Button {
            return Find<T>(namePath, transform, func);
        }
        public T Find<T>(string namePath, UnityAction<BaseEventData> mouseEnter, UnityAction<BaseEventData> mouseExit) where T : Button {
            return Find<T>(namePath, transform, mouseEnter, mouseExit);
        }
        public T Find<T>(string namePath, Transform trs) {
            return Utils.Find<T>((trs ? trs : transform), namePath);
        }
        public T Find<T>(string namePath, Transform trs, Action func) where T : Button {
            var resObj = Utils.Find<T>((trs ? trs : transform), namePath);
            if (typeof(T) == typeof(Button)) { //button支持绑定函数方法
                resObj.onClick.AddListener(() => { func(); });
            }
            return resObj;
        }
        public T Find<T>(string namePath, Transform trs, UnityAction<BaseEventData> mouseEnter, UnityAction<BaseEventData> mouseExit) where T : Button {
            var resObj = Utils.Find<T>(trs ? trs : transform, namePath);
            if (typeof(T) == typeof(Button)) { //button支持绑定函数方法
                BindBtn(resObj, mouseEnter, mouseExit);
            }
            return resObj;
        }

        #endregion

        #region Bind接口列表

        public void Bind(Button btn, Action click) {
            btn.onClick.AddListener(() => { click(); });
        }
        public void Bind(Button btn, Action click, UnityAction<BaseEventData> mouseEnter, UnityAction<BaseEventData> mouseExit = null) {
            btn.onClick.AddListener(() => { click(); });
            BindBtn(btn, mouseEnter, mouseExit);
        }
        public void Bind(Button btn, UnityAction<BaseEventData> mouseEnter, UnityAction<BaseEventData> mouseExit) {
            BindBtn(btn, mouseEnter, mouseExit);
        }
        private void BindBtn(Button btn, UnityAction<BaseEventData> mouseEnter, UnityAction<BaseEventData> mouseExit = null) {
            var trigger = btn.gameObject.GetComponent<EventTrigger>();
            trigger = trigger ? trigger : btn.gameObject.AddComponent<EventTrigger>();
            // 实例化delegates(trigger.trigger是注册在EventTrigger组件上的所有功能)  
            trigger.triggers = new List<EventTrigger.Entry>();
            // 在EventSystem委托列表中进行登记 
            if (mouseEnter != null) {
                var enterEntry = new EventTrigger.Entry {
                    // 设置 事件类型  
                    eventID = EventTriggerType.PointerEnter,
                    // 实例化回调函数  
                    callback = new EventTrigger.TriggerEvent()
                };
                //UnityAction 本质上是delegate,且有数个泛型版本(参数最多是四个),一个UnityAction可以添加多个函数(多播委托)  
                //将方法绑定在回调上(给回调方法添加监听)  
                enterEntry.callback.AddListener(mouseEnter);
                // 添加事件触发记录到GameObject的事件触发组件  
                trigger.triggers.Add(enterEntry);
            }
            if (mouseExit == null) return;
            var exitEntry = new EventTrigger.Entry {
                // 设置 事件类型  
                eventID = EventTriggerType.PointerExit,
                // 实例化回调函数  
                callback = new EventTrigger.TriggerEvent()
            };
            //将方法绑定在回调上(给回调方法添加监听)  
            exitEntry.callback.AddListener(mouseExit);
            // 添加事件触发记录到GameObject的事件触发组件  
            trigger.triggers.Add(exitEntry);
        }

        #endregion

        #region page life cycle

        //界面生命周期流程,这里只提供虚方法,具体的逻辑由各个业务界面进行重写
        //进入界面
        public virtual void OnEnter(dynamic options = null) {
            if (options == null) {
                OnEnter();
                return;
            }
            IsShow = true;
            gameObject.SetActive(true);
        }
        public virtual void OnEnter() {
            IsShow = true;
            gameObject.SetActive(true);
        }

        //暂停界面
        public void OnPause() {
            IsShow = false;
        }

        //恢复界面
        public void OnResume() {
            IsShow = true;
        }
        //关闭界面
        public virtual void OnExit() {
            UIManager.Instance.Close(Config.pageName);
            IsShow = false;
        }

        #endregion
    }
}