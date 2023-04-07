// author:KIPKIPS
// date:2023.02.08 12:08
// describe:行为树编辑器
using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Framework.AI.BehaviorTree {
    public class BehaviorTreeEditor : EditorWindow {
        private BehaviorTreeView _treeView;
        private InspectorView _inspectorView;
        private IMGUIContainer _blackboardView;

        private SerializedObject _treeObject;
        private SerializedProperty _blackboardProperty;
        [MenuItem("Tools/AI/BehaviorTreeEditor")]
        public static void OpenWindow() {
            var wnd = GetWindow<BehaviorTreeEditor>();
            wnd.titleContent = new GUIContent("BehaviorTreeEditor");
        }
        
        [OnOpenAsset(1)]
        public static bool Open(int instanceID,int line) {
            var open = Selection.activeObject is BehaviorTree;
            if (open) {
                OpenWindow();
            }
            return open;
        }

        public void CreateGUI() {
            // Each editor window contains a root VisualElement object
            var root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Framework/BehaviorTree/Editor/VisualTree/BehaviorTree.uxml");
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Framework/BehaviorTree/Editor/StyleSheet/BehaviorTree.uss");
            root.styleSheets.Add(styleSheet);

            _treeView = root.Q<BehaviorTreeView>();
            _inspectorView = root.Q<InspectorView>();
            _blackboardView = root.Q<IMGUIContainer>();
            _blackboardView.onGUIHandler = () => {
                if (_treeObject!=null && _blackboardProperty != null) {
                    _treeObject.Update();
                    EditorGUILayout.PropertyField(_blackboardProperty);
                    _treeObject.ApplyModifiedProperties();
                }
            };
            _treeView.OnNodeSelected = OnNodeSelectionChanged;
            OnSelectionChange();
        }
        private void OnEnable() {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }
        private void OnDisable() {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }
        private void OnPlayModeStateChanged(PlayModeStateChange obj) {
            switch (obj) {
                case PlayModeStateChange.EnteredEditMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }
        private void OnSelectionChange() {
            var tree = Selection.activeObject as BehaviorTree;
            if (!tree) {
                if (Selection.activeGameObject) {
                    var runner = Selection.activeGameObject.GetComponent<BehaviorTreeRunner>();
                    if (runner) {
                        tree = runner.behaviorTree;
                    }
                }
            }
            if (_treeView != null && tree) {
                if (Application.isPlaying) {
                    _treeView.PopulateView(tree);
                } else {
                    if (AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID())) {
                        _treeView.PopulateView(tree);
                    }
                }
            }
            if (tree!= null) {
                _treeObject = new SerializedObject(tree);
                _blackboardProperty = _treeObject.FindProperty("blackboard");
            }
        }

        private void OnNodeSelectionChanged(NodeView nodeView) {
            _inspectorView?.UpdateSelection(nodeView);
        }

        private void OnInspectorUpdate() {
            _treeView?.UpdateNodeStates();
        }
    }
}
