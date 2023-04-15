// author:KIPKIPS
// date:2023.04.15 04:20
// describe:
namespace Framework.UI {
    [Binder(typeof(LImage))]
    public class LImageBinder : BaseBinder {
        [BinderParams(typeof(LImage))]
        public enum AttributeType {
            sprite = LinkerType.String,
            size = LinkerType.String,
        }
        public override void SetString(UnityEngine.Object mono, int linkerType, string value) {
            if (mono == null) return;
            var target = mono as LText;
            switch ((AttributeType)linkerType) {
                case AttributeType.sprite:
                    target.text = value;
                    break;
            }
        }
    }
}