// author:KIPKIPS
// date:2023.04.23 18:48
// describe:RectTransform绑定类
// ReSharper disable InconsistentNaming
using UnityEngine;

namespace Framework.UI {
    [BinderComponent(typeof(RectTransform))]
    public class LRectTransformBinder : BaseBinder {
        [BinderField(typeof(RectTransform))]
        public enum AttributeType {
            anchorMin = 10000 + LinkerType.Vector2,
            anchorMax = 20000 + LinkerType.Vector2,
            anchoredPosition = 30000 + LinkerType.Vector2,
            sizeDelta = 40000 + LinkerType.Vector2,
            pivot = 50000 + LinkerType.Vector2,
            anchoredPosition3D = 60000 + LinkerType.Vector3,
            offsetMin = 70000 + LinkerType.Vector2,
            offsetMax = 80000 + LinkerType.Vector2,
            position = 90000 + LinkerType.Vector3,
            localPosition = 100000 + LinkerType.Vector3,
            eulerAngles = 110000 + LinkerType.Vector3,
            localEulerAngles = 120000 + LinkerType.Vector3,
            right = 130000 + LinkerType.Vector3,
            up = 140000 + LinkerType.Vector3,
            forward = 150000 + LinkerType.Vector3,
            rotation = 160000 + LinkerType.Quaternion,
            localRotation = 170000 + LinkerType.Quaternion,
            localScale = 180000 + LinkerType.Vector3,
        }

        public override void SetVector2(Object mono, int linkerType, Vector2 value) {
            if (mono == null) return;
            var target = mono as RectTransform;
            switch ((AttributeType)linkerType) {
                case AttributeType.anchorMin:
                    target.anchorMin = value;
                    break;
                case AttributeType.anchorMax:
                    target.anchorMax = value;
                    break;
                case AttributeType.anchoredPosition:
                    target.anchoredPosition = value;
                    break;
                case AttributeType.sizeDelta:
                    target.sizeDelta = value;
                    break;
                case AttributeType.pivot:
                    target.pivot = value;
                    break;
                case AttributeType.offsetMin:
                    target.offsetMin = value;
                    break;
                case AttributeType.offsetMax:
                    target.offsetMax = value;
                    break;
            }
        }
        public override void SetVector3(Object mono, int linkerType, Vector3 value) {
            if (mono == null) return;
            var target = mono as RectTransform;
            switch ((AttributeType)linkerType) {
                case AttributeType.anchoredPosition3D:
                    target.anchoredPosition3D = value;
                    break;
                case AttributeType.position:
                    target.position = value;
                    break;
                case AttributeType.localPosition:
                    target.localPosition = value;
                    break;
                case AttributeType.eulerAngles:
                    target.eulerAngles = value;
                    break;
                case AttributeType.localEulerAngles:
                    target.localEulerAngles = value;
                    break;
                case AttributeType.right:
                    target.right = value;
                    break;
                case AttributeType.up:
                    target.up = value;
                    break;
                case AttributeType.forward:
                    target.forward = value;
                    break;
                case AttributeType.localScale:
                    target.localScale = value;
                    break;
                default:
                    break;
            }
        }
        public override void SetQuaternion(Object mono, int linkerType, Quaternion value) {
            if (mono == null) return;
            var target = mono as RectTransform;
            switch ((AttributeType)linkerType) {
                case AttributeType.rotation:
                    target.rotation = value;
                    break;
                case AttributeType.localRotation:
                    target.localRotation = value;
                    break;
            }
        }
    }
}