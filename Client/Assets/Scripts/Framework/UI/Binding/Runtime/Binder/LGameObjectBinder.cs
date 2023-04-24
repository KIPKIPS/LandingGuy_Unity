// author:KIPKIPS
// date:2023.04.24 11:00
// describe:GameObject绑定类
// ReSharper disable InconsistentNaming
using UnityEngine;

namespace Framework.UI {
    [BinderComponent(typeof(GameObject))]
    public class LGameObjectBinder : BaseBinder {
        [BinderField(typeof(GameObject))]
        public enum AttributeType : int {
            active = 10000 + LinkerType.Boolean,
            name = 20000 + LinkerType.String,
        }

        public override void SetBoolean(Object mono, int linkerType, bool value) {
            if (mono == null) return;
            var target = mono as GameObject;
            switch ((AttributeType)linkerType) {
                case AttributeType.active:
                    if (target.activeSelf != value) {
                        target.SetActive(value);
                    }
                    break;
            }
        }

        public override void SetString(Object mono, int linkerType, string value) {
            if (mono == null) return;
            var target = mono as GameObject;
            switch ((AttributeType)linkerType) {
                case AttributeType.name:
                    target.name = value;
                    break;
            }
        }
    }
}