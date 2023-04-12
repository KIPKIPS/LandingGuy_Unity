// author:KIPKIPS
// date:2023.04.10 18:06
// describe:BasePage UI面板的基类
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using Button = UnityEngine.UI.Button;
using UnityEngine.Events;

namespace Framework.UI {
    public class BasePage : MonoBehaviour {
        public PageConfig Config{ get; set; }
        public bool IsShow { get; private set; }
        private Canvas _canvas;
        private Canvas Canvas {
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

        /// <summary>
        /// 按钮绑定
        /// </summary>
        /// <param name="btn">按钮</param>
        /// <param name="callback">点击事件</param>
        public void Bind(Button btn, Action callback) {
            btn.onClick.AddListener(() => { callback(); });
        }
        public void Bind(Button btn, Action callback, UnityAction<BaseEventData> mouseEnter, UnityAction<BaseEventData> mouseExit = null) {
            btn.onClick.AddListener(() => { callback(); });
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

        /// <summary>
        /// 进入界面
        /// </summary>
        /// <param name="options">参数传递</param>
        public virtual void OnEnter([CanBeNull] dynamic options) {
            OnEnter();
        }
        /// <summary>
        /// 进入界面
        /// </summary>
        protected virtual void OnEnter() {
            Canvas.worldCamera = CameraProxy.GetCamera(CameraType.UI);
            IsShow = true;
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 暂停界面
        /// </summary>
        public virtual void OnPause() {
            IsShow = false;
        }

        /// <summary>
        /// 恢复界面
        /// </summary>
        public virtual void OnResume() {
            IsShow = true;
        }
        /// <summary>
        /// 关闭界面
        /// </summary>
        public virtual void OnExit() {
            IsShow = false;
        }

        #endregion
    }
}