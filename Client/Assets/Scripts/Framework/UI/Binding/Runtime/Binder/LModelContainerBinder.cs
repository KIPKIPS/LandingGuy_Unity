// author:KIPKIPS
// date:2023.04.28 14:05
// describe:模型绑定器
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
using UnityEngine;

namespace Framework.UI {
    [BinderComponent(typeof(LModelContainer))]
    public class LModelContainerBinder : BaseBinder {
        [BinderField(typeof(LModelContainer))]
        public enum AttributeType {
            modelPath = 10000 + LinkerType.String,
            modelLocalRotation = 20000 + LinkerType.Vector3,
            modelLocalPosition = 30000 + LinkerType.Vector3,
            modelLocalScale = 40000 + LinkerType.Vector3,
        }

        public override void SetString(Object mono, int linkerType, string value) {
            if (mono == null) return;
            var target = mono as LModelContainer;
            switch ((AttributeType)linkerType) {
                case AttributeType.modelPath:
                    target.LoadModel(value);
                    break;
            }
        }
        public override void SetVector3(Object mono, int linkerType, Vector3 value) {
            if (mono == null) return;
            var target = mono as LModelContainer;
            switch ((AttributeType)linkerType) {
                case AttributeType.modelLocalRotation:
                    target.SetModelLocalRotation(value);
                    break;
                case AttributeType.modelLocalPosition:
                    target.SetModelLocalPosition(value);
                    break;
                case AttributeType.modelLocalScale:
                    target.SetModelLocalScale(value);
                    break;
            }
        }
    }
}