﻿// author:KIPKIPS
// date:2023.04.10 18:06
// describe:BasePage UI面板的基类
using System;
using System.Collections.Generic;
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
        public bool IsShow { get; set; }
        private static UIBinding _uiBinding;
        public UIBinding UIBinding {
            get => _uiBinding;
            set => _uiBinding = value;
        }
        private static readonly Dictionary<string, dynamic> _bindDict = new();

        /// <summary>
        /// 绑定字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        protected static void Bind<T>(string key, T value = default) {
            _bindDict[key].Value = value;
        }
        protected static void DOBind<T>(string key, T value = default) {
            _bindDict.Add(key,new Bindable<T>(_uiBinding, key, value));
        }
        protected static void DOBind(string key, UnityAction action) {
            _bindDict.Add(key,new Bindable<UnityAction>(_uiBinding, key, action));
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
        protected virtual void OnEnter() {
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