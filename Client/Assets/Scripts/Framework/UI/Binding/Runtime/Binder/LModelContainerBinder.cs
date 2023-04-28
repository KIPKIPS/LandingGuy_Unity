// --[[
//     author:{wkp}
//     time:14:02
// ]]
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
using UnityEngine;

namespace Framework.UI {
    [BinderComponent(typeof(LModelContainer))]
    public class LModelContainerBinder : BaseBinder {
        [BinderField(typeof(LModelContainer))]
        public enum AttributeType {
            modelPath = 10000 + LinkerType.String,
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
    }
}