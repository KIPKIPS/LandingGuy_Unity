// author:KIPKIPS
// date:2023.04.21 23:26
// describe:Button绑定类
// ReSharper disable InconsistentNaming
using UnityEngine;
using UnityEngine.Events;

namespace Framework.UI {
    [BinderComponent(typeof(LButton))]
    public class LButtonBinder : BaseBinder {
        [BinderField(typeof(LButton))]
        public enum AttributeType {
            onClick = 10000 + LinkerType.UnityAction,
            enabled = 20000 + LinkerType.Boolean,
        }
        public override void SetAction(Object mono, int linkerType, UnityAction value) {
            if (mono == null) return;
            var target = mono as LButton;
            switch ((AttributeType)linkerType) {
                case AttributeType.onClick:
                    target.onClick.AddListener(value);
                    break;
            }
        }
        public override void SetBoolean(Object mono, int linkerType, bool value) {
            if (mono == null) return;
            var target = mono as LButton;
            switch ((AttributeType)linkerType) {
                case AttributeType.enabled:
                    target.enabled = value;
                    break;
            }
        }
        // public override void RemoveAction(Object mono, int linkerType, UnityAction value) {
        //     if (mono == null) return;
        //     var target = mono as LButton;
        //     switch ((AttributeType)linkerType) {
        //         case AttributeType.onClick:
        //             target.onClick.RemoveListener(value);
        //             break;
        //         
        //     }
        // }
    }
}