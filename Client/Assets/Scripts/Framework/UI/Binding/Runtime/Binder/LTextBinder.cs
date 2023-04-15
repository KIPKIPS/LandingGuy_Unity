// --[[
//     author:{wkp}
//     time:19:49
// ]]
namespace Framework.UI {
    [Binder(typeof(LText))]
    public class LTextBinder : BaseBinder {
        [BinderParams(typeof(LText))]
        public enum AttributeType {
            text = LinkerType.String,
            duration = LinkerType.Single,
            resizeTextForBestFit = LinkerType.Boolean,
            resizeTextMinSize = LinkerType.Int32,
            resizeTextMaxSize = LinkerType.Int32,
            alignByGeometry = LinkerType.Boolean,
            fontSize = LinkerType.Int32,
            lineSpacing = LinkerType.Single,
            maskable = LinkerType.Boolean,
            isMaskingGraphic = LinkerType.Boolean,
            color = LinkerType.Color,
            raycastTarget = LinkerType.Boolean,
            enabled = LinkerType.Boolean,
            alignment = LinkerType.Int32
        }
        public override void SetString(UnityEngine.Object mono, int linkerType, string value) {
            if (mono == null) return;
            var target = mono as LText;
            switch ((AttributeType)linkerType) {
                case AttributeType.text:
                    target.text = value;
                    break;
            }
        }
    }
}