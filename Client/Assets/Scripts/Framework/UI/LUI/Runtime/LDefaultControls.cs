// --[[
//     author:{wkp}
//     time:14:33
// ]]
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI {
    public class LDefaultControls {
        private const float Width = 160f;
        private const float ThickHeight = 30f;
        private const float ThinHeight = 20f;
        private static readonly Vector2 ThickElementSize = new(Width, ThickHeight);
        private static readonly Vector2 ThinElementSize = new(Width, ThickHeight);
        private static readonly Vector2 ImageElementSize = new(100f, 100f);
        private static readonly Color DefaultSelectableColor = new(1f, 1f, 1f, 1f);
        private static readonly Color PanelColor = new(1f, 1f, 1f, 0.392f);
        private static readonly Color TextColor = new(50f / 255f, 50f / 255f, 50f / 255f, 1f);

        private static IFactoryControls _currentFactory = DefaultRuntimeFactory.Default;
        public static IFactoryControls Factory {
            get => _currentFactory;
#if UNITY_EDITOR
            set => _currentFactory = value;
#endif
        }

        public interface IFactoryControls {
            GameObject CreateGameObject(string name, params Type[] components);
        }

        private class DefaultRuntimeFactory : IFactoryControls {
            public static readonly IFactoryControls Default = new DefaultRuntimeFactory();
            public GameObject CreateGameObject(string name, params Type[] components) {
                return new GameObject(name, components);
            }
        }

        public struct Resources {
            public Sprite standard;
            public Sprite background;
            public Sprite inputField;
            public Sprite knob;
            public Sprite checkmark;
            public Sprite dropdown;
            public Sprite mask;
        }

        public static GameObject CreateText(Resources resources) {
            var obj = CreateUIElementRoot("Text", ThickElementSize,typeof(LText));
            var lbl = obj.GetComponent<LText>();
            SetDefaultTextValues(lbl);
            return obj;
        }
        public static GameObject CreateImage(Resources resources) {
            var obj = CreateUIElementRoot("Image", ImageElementSize,typeof(LImage));
            var lbl = obj.GetComponent<LImage>();
            SetDefaultImageValues(lbl);
            return obj;
        }
        
        private static void SetDefaultImageValues(LImage lbl) {
            lbl.raycastTarget = false;
        }
        private static void SetDefaultTextValues(LText lbl) {
            lbl.text = "New Text";
            lbl.raycastTarget = false;
        }
        private static GameObject CreateUIElementRoot(string name, Vector2 size, params Type[] components) {
            var child = Factory.CreateGameObject(name, components);
            var rectTransform = child.GetComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            rectTransform.anchoredPosition = Vector2.zero;
            return child;
        }
    }
}