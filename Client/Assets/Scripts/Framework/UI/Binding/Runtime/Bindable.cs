// author:KIPKIPS
// date:2023.04.17 23:27
// describe:
using System;
using System.Diagnostics;

namespace Framework.UI {
    public class Bindable<T> {
        //保存真正的值
        private T _value;
        //get时返回真正的值，set时顺便调用值改变事件
        public T Value {
            get => _value;
            set {
                if (_value is not null && Equals(value, _value)) return;
                _value = value;
                OnValueChanged?.Invoke(value);
            }
        }
        //用event存储值改变的事件
        public event Action<T> OnValueChanged;
        //初始化
        public Bindable(int pageId,string key,T value = default) {
            BindEvent(pageId,key);
            _value = value;
        }
        private void BindEvent(int pageId,string key) {
            OnValueChanged = v=> UIManager.Instance.UpdateData<T>(pageId,key,_value);
        }
    }
}