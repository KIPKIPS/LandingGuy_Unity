// --[[
//     author:{wkp}
//     time:14:28
// ]]
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework.UI {
    public class MenuOptions {
        private const string UILayerName = "UI";
        private const string StandardSpritePath = "UI/Skin/UISprite.psd";
        private const string BackgroundSpritePath = "UI/Skin/Background.psd";
        private const string InputFieldBackgroundPath = "UI/Skin/InputFieldBackground.psd";
        private const string KnobPath = "UI/Skin/Knob.psd";
        private const string CheckmarkPath = "UI/Skin/Checkmark.psd";
        private const string DropdownArrowPath = "UI/Skin/DropdownArrow.psd";
        private const string MaskPath = "UI/Skin/UIMask.psd";
        private const string MenuPrefix = "GameObject/UI/";
        
        private static  LDefaultControls.Resources _standardResources;
        
        enum MenuOptionsPriorityOrder {
            Image = -102,
            RawImage = 2002,
            Panel = 2003,
            Toggle = 2021,
            Slider = 2024,
            Scrollbar = 2025,
            ScrollView = 2026,
            Canvas = 2060,
            EventSystem = 2061,
            Text = -101,
            Button = -103,
            Dropdown = 2082,
            InputField = 2083,
        };

        [MenuItem(MenuPrefix + "Text", false, (int)MenuOptionsPriorityOrder.Text)]
        public static void AddText(MenuCommand menuCommand) {
            GameObject go;
            using (new FactorySwapToEditor()) {
                go = LDefaultControls.CreateText(GetStandardResources());
            }
            PlaceUIElementRoot(go, menuCommand);
        }
        
        [MenuItem(MenuPrefix + "Image", false, (int)MenuOptionsPriorityOrder.Image)]
        public static void AddImage(MenuCommand menuCommand) {
            GameObject go;
            using (new FactorySwapToEditor()) {
                go = LDefaultControls.CreateImage(GetStandardResources());
            }
            PlaceUIElementRoot(go, menuCommand);
        }
        
        private class FactorySwapToEditor : IDisposable
        {
            private readonly LDefaultControls.IFactoryControls _factory;
            public FactorySwapToEditor() {
                _factory = LDefaultControls.Factory;
                LDefaultControls.Factory = DefaultEditorFactory.Default;
            }

            public void Dispose() {
                LDefaultControls.Factory = _factory;
            }
        }
        private class DefaultEditorFactory : LDefaultControls.IFactoryControls {
            public static readonly DefaultEditorFactory Default = new();
            public GameObject CreateGameObject(string name, params Type[] components) {
                return ObjectFactory.CreateGameObject(name, components);
            }
        }

        private static GameObject CreateNewUI() {
            var root = ObjectFactory.CreateGameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            root.layer = LayerMask.NameToLayer(UILayerName);
            var canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            StageUtility.PlaceGameObjectInCurrentStage(root);
            var customScene = false;
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null) {
                Undo.SetTransformParent(root.transform, prefabStage.prefabContentsRoot.transform, "");
                customScene = true;
            }
            Undo.SetCurrentGroupName("Create " + root.name);
            if (!customScene) CreateEventSystem(false);
            return root;
        }
        private static void CreateEventSystem(MenuCommand menuCommand) {
            var parent = menuCommand.context as GameObject;
            CreateEventSystem(true, parent);
        }
        private static void CreateEventSystem(bool select) {
            CreateEventSystem(select, null);
        }

        private static void CreateEventSystem(bool select, GameObject parent) {
            var stage = parent == null ? StageUtility.GetCurrentStageHandle() : StageUtility.GetStageHandle(parent);
            var es = stage.FindComponentOfType<EventSystem>();
            if (es == null) {
                var eventSystem = ObjectFactory.CreateGameObject("EventSystem");
                if (parent == null)
                    StageUtility.PlaceGameObjectInCurrentStage(eventSystem);
                else
                    SetParentAndAlign(eventSystem, parent);
                es = ObjectFactory.AddComponent<EventSystem>(eventSystem);
                ObjectFactory.AddComponent<StandaloneInputModule>(eventSystem);
                Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
            }
            if (select && es != null) {
                Selection.activeGameObject = es.gameObject;
            }
        }

        private static LDefaultControls.Resources GetStandardResources() {
            if (_standardResources.standard != null) return _standardResources;
            _standardResources.standard = AssetDatabase.GetBuiltinExtraResource<Sprite>(StandardSpritePath);
            _standardResources.background = AssetDatabase.GetBuiltinExtraResource<Sprite>(BackgroundSpritePath);
            _standardResources.inputField = AssetDatabase.GetBuiltinExtraResource<Sprite>(InputFieldBackgroundPath);
            _standardResources.knob = AssetDatabase.GetBuiltinExtraResource<Sprite>(KnobPath);
            _standardResources.checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>(CheckmarkPath);
            _standardResources.dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>(DropdownArrowPath);
            _standardResources.mask = AssetDatabase.GetBuiltinExtraResource<Sprite>(MaskPath);
            return _standardResources;
        }

        private static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand) {
            var parent = menuCommand.context as GameObject;
            var explicitParentChoice = true;
            if (parent == null) {
                parent = GetOrCreateCanvasGameObject();
                explicitParentChoice = false;
                var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabStage != null && !prefabStage.IsPartOfPrefabContents(parent)) parent = prefabStage.prefabContentsRoot;
            }
            if (parent.GetComponentsInParent<Canvas>(true).Length == 0) {
                var canvas = MenuOptions.CreateNewUI();
                Undo.SetTransformParent(canvas.transform, parent.transform, "");
                parent = canvas;
            }
            GameObjectUtility.EnsureUniqueNameForSibling(element);
            SetParentAndAlign(element, parent);
            if (!explicitParentChoice) SetPositionVisibleInSceneView(parent.GetComponent<RectTransform>(), element.GetComponent<RectTransform>());
            Undo.RegisterFullObjectHierarchyUndo(parent == null ? element : parent, "");
            Undo.SetCurrentGroupName("Create " + element.name);
            Selection.activeGameObject = element;
        }
        
        private static void SetPositionVisibleInSceneView(RectTransform canvasRectTransform, RectTransform itemTransform) {
            var sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null || sceneView.camera == null) return;

            var camera = sceneView.camera;
            var position = Vector3.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, new Vector2(camera.pixelWidth / 2f, camera.pixelHeight / 2f), camera, out var localPlanePosition)) {
                localPlanePosition.x += canvasRectTransform.sizeDelta.x * canvasRectTransform.pivot.x;
                localPlanePosition.y += canvasRectTransform.sizeDelta.y * canvasRectTransform.pivot.y;

                localPlanePosition.x = Mathf.Clamp(localPlanePosition.x, 0, canvasRectTransform.sizeDelta.x);
                localPlanePosition.y = Mathf.Clamp(localPlanePosition.y, 0, canvasRectTransform.sizeDelta.y);
                
                var sizeDelta = canvasRectTransform.sizeDelta;
                var anchorMin = itemTransform.anchorMin;
                position.x = localPlanePosition.x - sizeDelta.x * anchorMin.x;
                position.y = localPlanePosition.y - sizeDelta.y * anchorMin.y;

                Vector3 minLocalPosition;
                minLocalPosition.x = sizeDelta.x * (0 - canvasRectTransform.pivot.x) + itemTransform.sizeDelta.x * itemTransform.pivot.x;
                minLocalPosition.y = canvasRectTransform.sizeDelta.y * (0 - canvasRectTransform.pivot.y) + itemTransform.sizeDelta.y * itemTransform.pivot.y;

                Vector3 maxLocalPosition;
                maxLocalPosition.x = canvasRectTransform.sizeDelta.x * (1 - canvasRectTransform.pivot.x) - itemTransform.sizeDelta.x * itemTransform.pivot.x;
                maxLocalPosition.y = canvasRectTransform.sizeDelta.y * (1 - canvasRectTransform.pivot.y) - itemTransform.sizeDelta.y * itemTransform.pivot.y;

                position.x = Mathf.Clamp(position.x, minLocalPosition.x, maxLocalPosition.x);
                position.y = Mathf.Clamp(position.y, minLocalPosition.y, maxLocalPosition.y);
            }
            itemTransform.anchoredPosition = position;
            itemTransform.localRotation = Quaternion.identity;
            itemTransform.localScale = Vector3.one;
        }
        private static void SetParentAndAlign(GameObject child, GameObject parent) {
            if (parent == null) return;
            Undo.SetTransformParent(child.transform, parent.transform, "");
            var rectTransform = child.transform as RectTransform;
            if (rectTransform) {
                rectTransform.anchoredPosition = Vector2.zero;
                var localPosition = rectTransform.localPosition;
                localPosition.z = 0;
                rectTransform.localPosition = localPosition;
            } else {
                child.transform.localPosition = Vector3.zero;
            }
            child.transform.localRotation = Quaternion.identity;
            child.transform.localScale = Vector3.one;
            SetLayerRecursively(child, parent.layer);
        }
        private static void SetLayerRecursively(GameObject go, int layer) {
            go.layer = layer;
            var t = go.transform;
            for (var i = 0; i < t.childCount; i++) SetLayerRecursively(t.GetChild(i).gameObject, layer);
        }
        private static GameObject GetOrCreateCanvasGameObject() {
            var selectedGo = Selection.activeGameObject;
            var canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
            if (IsValidCanvas(canvas)) return canvas.gameObject;

            var canvasArray = StageUtility.GetCurrentStageHandle().FindComponentsOfType<Canvas>();
            foreach (var t in canvasArray) {
                if (IsValidCanvas(t)) {
                    return t.gameObject;
                }
            }
            return CreateNewUI();
        }
        private static bool IsValidCanvas(Canvas canvas) {
            if (canvas == null || !canvas.gameObject.activeInHierarchy) return false;
            if (EditorUtility.IsPersistent(canvas) || (canvas.hideFlags & HideFlags.HideInHierarchy) != 0) return false;
            return StageUtility.GetStageHandle(canvas.gameObject) == StageUtility.GetCurrentStageHandle();
        }
    }
}