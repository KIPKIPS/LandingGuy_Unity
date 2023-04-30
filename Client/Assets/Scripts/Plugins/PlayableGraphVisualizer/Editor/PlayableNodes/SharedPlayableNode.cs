using System;

namespace GraphVisualizer {
    public class SharedPlayableNode : Node {
        public SharedPlayableNode(object content, float weight = 1.0f, bool active = false) : base(content, weight, active) {
        }
        protected static string InfoString(string key, double value) {
            if (Math.Abs(value) < 100000.0)
                return $"<b>{key}:</b> {value:#.###}";
            if (value == double.MaxValue)
                return $"<b>{key}:</b> +Inf";
            if (value == double.MinValue)
                return $"<b>{key}:</b> -Inf";
            return $"<b>{key}:</b> {value:E4}";
        }
        protected static string InfoString(string key, int value) {
            return $"<b>{key}:</b> {value:D}";
        }
        protected static string InfoString(string key, object value) {
            return "<b>" + key + ":</b> " + (value ?? "(none)");
        }
        protected static string RemoveFromEnd(string str, string suffix) {
            return str.EndsWith(suffix) ? str.Substring(0, str.Length - suffix.Length) : str;
        }
    }
}