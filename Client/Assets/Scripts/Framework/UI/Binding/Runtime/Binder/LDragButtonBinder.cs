// author:KIPKIPS
// date:2023.04.27 20:21
// describe:拖拽按钮Binder
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
using UnityEngine;
using UnityEngine.Events;

namespace Framework.UI {
    [BinderComponent(typeof(LDragButton))]
    public class LDragButtonBinder : BaseBinder {
        [BinderField(typeof(LDragButton))]
        public enum AttributeType {
            onClick = 10000 + LinkerType.UnityAction,
            enabled = 20000 + LinkerType.Boolean,
            onEnter = 30000 + LinkerType.UnityActionVector2,
            onExit = 40000 + LinkerType.UnityActionVector2,
            onDragBegin = 50000 + LinkerType.UnityActionVector2,
            onDrag = 60000 + LinkerType.UnityActionVector2,
            onDragEnd = 70000 + LinkerType.UnityActionVector2,
        }
        public override void SetActionVector2(Object mono, int linkerType, UnityAction<Vector2> value) {
            if (mono == null) return;
            var target = mono as LDragButton;
            switch ((AttributeType)linkerType) {
                case AttributeType.onEnter:
                    target.onPointerEnter = value;
                    break;
                case AttributeType.onExit:
                    target.onPointerExit = value;
                    break;
                case AttributeType.onDragBegin:
                    target.onDragBegin = value;
                    break;
                case AttributeType.onDrag:
                    target.onDrag = value;
                    break;
                case AttributeType.onDragEnd:
                    target.onDragEnd = value;
                    break;
            }
        }
        public override void SetAction(Object mono, int linkerType, UnityAction value) {
            if (mono == null) return;
            var target = mono as LDragButton;
            switch ((AttributeType)linkerType) {
                case AttributeType.onClick:
                    target.onClick.AddListener(value);
                    break;
            }
        }
        public override void SetBoolean(Object mono, int linkerType, bool value) {
            if (mono == null) return;
            var target = mono as LDragButton;
            switch ((AttributeType)linkerType) {
                case AttributeType.enabled:
                    target.enabled = value;
                    break;
            }
        }
        // public override void RemoveAction(Object mono, int linkerType, UnityAction value) {
        //     if (mono == null) return;
        //     var target = mono as LDragButton;
        //     switch ((AttributeType)linkerType) {
        //         case AttributeType.onClick:
        //             target.onClick.RemoveListener(value);
        //             break;
        //         
        //     }
        // }
    }
}