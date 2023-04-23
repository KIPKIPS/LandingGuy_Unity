// --[[
//     author:{wkp}
//     time:14:33
// ]]
using System;
using UnityEditor;
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
        private static readonly Color DefaultBlackColor = new(0f, 0f, 0f, 1f);
        private static readonly Color DefaultWhiteColor = new(1f, 1f, 1f, 1f);

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
            var obj = CreateUIElementRoot("Text", ThickElementSize, typeof(LText));
            var lbl = obj.GetComponent<LText>();
            SetDefaultTextValues(lbl);
            return obj;
        }
        public static GameObject CreateImage(Resources resources) {
            var obj = CreateUIElementRoot("Image", ImageElementSize, typeof(LImage));
            var lbl = obj.GetComponent<LImage>();
            SetDefaultImageValues(lbl);
            return obj;
        }
        private static GameObject CreateUIObject(string name, GameObject parent, params Type[] components) {
            var go = Factory.CreateGameObject(name, components);
            SetParentAndAlign(go, parent);
            return go;
        }
        private static void SetParentAndAlign(GameObject child, GameObject parent) {
            if (parent == null) return;
#if UNITY_EDITOR
            Undo.SetTransformParent(child.transform, parent.transform, "");
#else
            child.transform.SetParent(parent.transform, false);
#endif
            SetLayerRecursively(child, parent.layer);
        }
        private static void SetLayerRecursively(GameObject go, int layer) {
            go.layer = layer;
            var t = go.transform;
            for (var i = 0; i < t.childCount; i++) {
                SetLayerRecursively(t.GetChild(i).gameObject, layer);
            }
        }
        public static GameObject CreateButton(Resources resources) {
            var buttonRoot = CreateUIElementRoot("Button", ThickElementSize, typeof(LImage), typeof(LButton));
            var childText = CreateUIObject("Text", buttonRoot, typeof(LText));
            var image = buttonRoot.GetComponent<LImage>();
            // image.sprite = resources.standard;
            image.sprite = null;
            image.type = Image.Type.Sliced;
            image.color = DefaultSelectableColor;
            var bt = buttonRoot.GetComponent<LButton>();
            SetDefaultColorTransitionValues(bt);
            var text = childText.GetComponent<LText>();
            SetDefaultTextValues(text, "Button");
            text.color = DefaultBlackColor;
            var rect = text.GetComponent<RectTransform>();
            rect.anchoredPosition = Vector2.zero;
            // rect.
            var textRectTransform = childText.GetComponent<RectTransform>();
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.sizeDelta = Vector2.zero;
            return buttonRoot;
        }
        private static void SetDefaultColorTransitionValues(Selectable selectable) {
            var colors = selectable.colors;
            colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
            colors.pressedColor = new Color(0.698f, 0.698f, 0.698f);
            colors.disabledColor = new Color(0.521f, 0.521f, 0.521f);
        }

        private static void SetDefaultImageValues(LImage lbl) {
            lbl.raycastTarget = false;
        }
        private static void SetDefaultTextValues(LText lbl, string text = "") {
            lbl.text = string.IsNullOrEmpty(text) ? "New Text" : text;
            lbl.raycastTarget = false;
            lbl.alignment = TextAnchor.MiddleCenter;
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