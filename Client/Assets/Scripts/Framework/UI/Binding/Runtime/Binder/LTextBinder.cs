// --[[
//     author:{wkp}
//     time:19:49
// ]]
namespace Framework.UI {
    [Binder(typeof(LText))]
    public class LTextBinder : BaseBinder {
        [BinderParams(typeof(LText))]
        public enum AttributeType {
            Text = LinkerType.String,
            Duration = LinkerType.Single,
            ResizeTextForBestFit = LinkerType.Boolean,
            ResizeTextMinSize = LinkerType.Int32,
            ResizeTextMaxSize = LinkerType.Int32,
            AlignByGeometry = LinkerType.Boolean,
            FontSize = LinkerType.Int32,
            LineSpacing = LinkerType.Single,
            Maskable = LinkerType.Boolean,
            IsMaskingGraphic = LinkerType.Boolean,
            Color = LinkerType.Color,
            RaycastTarget = LinkerType.Boolean,
            Enabled = LinkerType.Boolean,
            Alignment = LinkerType.Int32
        }
        public override void SetString(UnityEngine.Object mono, int linkerType, string value) {
            if (mono == null) return;
            var target = mono as LText;
            switch ((AttributeType)linkerType) {
                case AttributeType.Text:
                    target.text = value;
                    break;
            }
        }
    }
}