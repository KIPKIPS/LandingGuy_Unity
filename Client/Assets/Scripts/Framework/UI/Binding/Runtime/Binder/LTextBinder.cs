// author:KIPKIPS
// date:2023.04.15 19:49
// describe:Text绑定类
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
using UnityEngine;

namespace Framework.UI {
    [BinderComponent(typeof(LText))]
    public class LTextBinder : BaseBinder {
        [BinderField(typeof(LText))]
        public enum AttributeType {
            text = 10000 + LinkerType.String,
            fontSize = 20000 + LinkerType.Int32,
            color = 30000 + LinkerType.Color,
            raycastTarget = 40000 + LinkerType.Boolean,
            enabled = 50000 + LinkerType.Boolean
        }
        public override void SetString(Object mono, int linkerType, string value) {
            if (mono == null) return;
            var target = mono as LText;
            switch ((AttributeType)linkerType) {
                case AttributeType.text:
                    target.text = value;
                    break;
            }
        }
        public override void SetInt32(Object mono, int linkerType, int value) {
            if (mono == null) return;
            var target = mono as LText;
            switch ((AttributeType)linkerType) {
                case AttributeType.fontSize:
                    target.fontSize = value;
                    break;
            }
        }
        public override void SetColor(Object mono, int linkerType, Color value) {
            if (mono == null) return;
            var target = mono as LText;
            switch ((AttributeType)linkerType) {
                case AttributeType.color:
                    target.color = value;
                    break;
            }
        }
        public override void SetBoolean(Object mono, int linkerType, bool value) {
            if (mono == null) return;
            var target = mono as LText;
            switch ((AttributeType)linkerType) {
                case AttributeType.raycastTarget:
                    target.raycastTarget = value;
                    break;
                case AttributeType.enabled:
                    target.enabled = value;
                    break;
            }
        }
    }
}