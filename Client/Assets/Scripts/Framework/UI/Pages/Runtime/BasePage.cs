// author:KIPKIPS
// date:2023.04.10 18:06
// describe:BasePage UI面板的基类
// ReSharper disable InconsistentNaming
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
namespace Framework.UI {
    public class BasePage {
        // public List<BinderData>
        public PageConfig Config { get; set; }
        private Canvas _canvas;
        public Canvas Canvas {
            get => _canvas != null ? _canvas : null;
            set => _canvas = value;
        }
        private bool _isShow;
        public bool IsShow {
            get => _isShow;
            set => _isShow = value;
        }
        private static UIBinding _uiBinding;
        public UIBinding UIBinding {
            get => _uiBinding;
            set => _uiBinding = value;
        }
        private static readonly Dictionary<string, Bindable> _bindDict = new();

        /// <summary>
        /// 更新绑定字段的值
        /// </summary>
        /// <param name="key">绑定字段</param>
        /// <param name="value">绑定的值</param>
        protected static void Bind(string key, dynamic value = null) {
            _bindDict[key].Value = value;
        }
        /// <summary>
        /// 字段值绑定
        /// </summary>
        /// <param name="key">绑定字段</param>
        /// <param name="value">绑定值</param>
        protected static void DOBind(string key,dynamic value) {
            if (_bindDict.TryAdd(key,new Bindable(_uiBinding, key, value))) {
                _bindDict[key].Value = value;
            }
        }
        protected static void DOBind(string key) {
            if (_bindDict.TryAdd(key,new Bindable(_uiBinding, key))) {
                _bindDict[key].Value = default;
            }
        }
        protected static void DOBind(string key, UnityAction value) {
            if (_bindDict.TryAdd(key,new Bindable(_uiBinding, key, value))) {
                _bindDict[key].Value = value;
            }
        }
        protected static void DOBind(string key, UnityAction<Vector2> value) {
            if (_bindDict.TryAdd(key,new Bindable(_uiBinding, key, value))) {
                _bindDict[key].Value = value;
            }
        }
        
        #region Page Life Cycle

        public void OnBind() {
            Values();
            Methods();
        }
        protected virtual void Values() {
        }
        protected virtual void Methods() {
        }
        /// <summary>
        /// 进入界面
        /// </summary>
        /// <param name="options">参数传递</param>
        public virtual void OnEnter(dynamic options) {
            OnEnter();
        }
        /// <summary>
        /// 进入界面
        /// </summary>
        public virtual void OnEnter() {
            _bindDict.Clear();
        }
        /// <summary>
        /// 暂停界面
        /// </summary>
        public virtual void OnPause() {
        }
        /// <summary>
        /// 恢复界面
        /// </summary>
        public virtual void OnResume() {
        }
        /// <summary>
        /// 关闭界面
        /// </summary>
        public virtual void OnExit() {
            _bindDict.Clear();
        }

        #endregion
    }
}