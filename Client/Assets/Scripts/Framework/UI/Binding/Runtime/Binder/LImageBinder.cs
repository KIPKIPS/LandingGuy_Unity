// author:KIPKIPS
// date:2023.04.15 04:20
// describe:Image绑定类
using UnityEngine;

namespace Framework.UI {
    [BinderComponent(typeof(LImage))]
    public class LImageBinder : BaseBinder {
        [BinderField(typeof(LImage))]
        public enum AttributeType {
            sprite = 10000 + LinkerType.String,
            color = 20000 + LinkerType.Color,
        }
        public override void SetString(Object mono, int linkerType, string value) {
            if (mono == null) return;
            var target = mono as LImage;
            switch ((AttributeType)linkerType) {
                case AttributeType.sprite:
                    // target.sprite = value;
                    break;
                
            }
        }
        public override void SetColor(Object mono, int linkerType, Color value) {
            if (mono == null) return;
            var target = mono as LImage;
            switch ((AttributeType)linkerType) {
                case AttributeType.color:
                    target.color = value;
                    break;
            }
        }
    }
}