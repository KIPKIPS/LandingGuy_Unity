﻿// author:KIPKIPS
// date:2023.04.17 23:27
// describe:可绑定处理
using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.UI {
    public class Bindable<T> {
        //保存真正的值
        private T _value;
        private bool _isInit;
        //get时返回真正的值，set时顺便调用值改变事件
        public T Value {
            get => _value;
            set {
                if (!_isInit && _value is not null && Equals(value, _value)) return;
                _value = value;
                _isInit = false;
                OnValueChanged?.Invoke(value);
            }
        }
        //用event存储值改变的事件
        public event Action<T> OnValueChanged;
        private readonly string _key;
        private readonly UIBinding _uiBinding;
        //初始化
        public Bindable(UIBinding uiBinding,string key,T value = default) {
            OnValueChanged = UpdateBind;
            _value = value;
            _isInit = true;
            _key = key;
            _uiBinding = uiBinding;
        }
        public Bindable(UIBinding uiBinding,string key,UnityAction unityAction) {
            if (!uiBinding.BinderDataDict.TryGetValue(key, out var data)) return;
            var baseBinder = UIBinding.GetBaseBinder(data.bindComponent.GetType().ToString());
            baseBinder.SetAction(data.bindComponent, data.bindFieldId,unityAction );
        }
        private void UpdateBind(T value) {
            if (!_uiBinding.BinderDataDict.TryGetValue(_key, out var data)) return;
            var baseBinder = UIBinding.GetBaseBinder(data.bindComponent.GetType().ToString());
            switch (typeof(T).Name) {
                case "String":
                    if (value is string stringValue) baseBinder.SetString(data.bindComponent, data.bindFieldId, stringValue);
                    break;
                case "Int32":
                    if (value is int intValue) baseBinder.SetInt32(data.bindComponent, data.bindFieldId,intValue);
                    break;
                case "Boolean":
                    if (value is bool boolValue) baseBinder.SetBoolean(data.bindComponent, data.bindFieldId,boolValue );
                    break;
                case "Color":
                    if (value is Color colorValue) baseBinder.SetColor(data.bindComponent, data.bindFieldId,colorValue );
                    break;
                default:
                    LUtil.LogWarning("Failure Binding",$"Unregistered binding type : {typeof(T).Name}");
                    break;
            }
        }
    }
}