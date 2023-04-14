// --[[
//     author:{wkp}
//     time:19:49
// ]]
namespace Framework.UI {
    [Binder(typeof(LText))]
    public class LTextBinder:BaseBinder {
        [BinderParams(typeof(LText))]
        public enum AttributeType : int {
            text = 10000 + LinkerType.String,
            numberMode = 20000 + LinkerType.Boolean,
            duration = 30000 + LinkerType.Single,
            onTweenComplete = 40000 + LinkerType.UnityAction,
            number = 50000 + LinkerType.Single,
            supportRichText = 60000 + LinkerType.Boolean,
            resizeTextForBestFit = 70000 + LinkerType.Boolean,
            resizeTextMinSize = 80000 + LinkerType.Int32,
            resizeTextMaxSize = 90000 + LinkerType.Int32,
            alignByGeometry = 100000 + LinkerType.Boolean,
            fontSize = 110000 + LinkerType.Int32,
            lineSpacing = 120000 + LinkerType.Single,
            // onCullStateChanged = 130000 + LinkerType.UnityAction_Boolean,
            maskable = 140000 + LinkerType.Boolean,
            isMaskingGraphic = 150000 + LinkerType.Boolean,
            color = 160000 + LinkerType.Color,
            raycastTarget = 170000 + LinkerType.Boolean,
            enabled = 180000 + LinkerType.Boolean,
            format = 190000 + LinkerType.String,
            alignment = 200000 + LinkerType.Int32,
        }

        public override void SetString(UnityEngine.Object mono, int linkerType, string value) {
            if (mono == null) return;
            var target = mono as LText;
            switch ((AttributeType)linkerType) {
                case AttributeType.text:
                    target.text = value;
                    break;
                default:
                    break;
            }
        }
    }
}