// author:KIPKIPS
// date:2023.04.17 23:27
// describe:
using System;

namespace Framework.UI {
    public class Bindable<T> {
        //保存真正的值
        private T value;
        //get时返回真正的值，set时顺便调用值改变事件
        public T Value {
            get => value;
            set {
                if (!Equals(value, this.value)) {
                    this.value = value;
                    OnValueChanged(value);
                }
            }
        }
        //用event存储值改变的事件
        public event Action<T> OnValueChanged;
        //初始化
        public Bindable(T value) {
            this.value = value;
        }
        public Bindable() {
            this.value = default(T);
        }
    }
}