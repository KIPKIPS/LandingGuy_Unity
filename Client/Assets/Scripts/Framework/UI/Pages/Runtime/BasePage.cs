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
        // public List<BinderData>
        public PageConfig Config{ get; set; }
        public bool IsShow { get; set; }
        private Canvas _canvas;
        private Canvas Canvas {
            get {
                _canvas ??= GetComponent<Canvas>();
                _canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.Tangent | AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.Normal;
                return _canvas;
            }
        }
        public UIBinding UIBinding { get; set; }

        protected static Bindable<T> Bind<T>(string key,T value = default) {
            return new Bindable<T>(pageID,key,value);
        }
        public static int pageID;

        #region Page Life Cycle

        public virtual void OnBind() {
            pageID = Config.pageID;
        }

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