// author:KIPKIPS
// date:2023.02.08 12:08
// describe:行为树编辑器
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Framework.AI.BehaviorTree {
    public class BehaviorTreeEditor : EditorWindow {
        private BehaviorTreeView _treeView;
        private InspectorView _inspectorView;
        private static readonly string AssetPath = "Assets/Scripts/Framework/BehaviorTree/Editor/BehaviorTreeEditor";
        [MenuItem("Tools/AI/BehaviorTreeEditor")]
        public static void OpenWindow() {
            var wnd = GetWindow<BehaviorTreeEditor>();
            wnd.titleContent = new GUIContent("BehaviorTreeEditor");
        }

        public static string GetAssetPath(string suffix) => AssetPath + suffix;

        public void CreateGUI() {
            // Each editor window contains a root VisualElement object
            var root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(GetAssetPath(".uxml"));
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(GetAssetPath(".uss"));
            root.styleSheets.Add(styleSheet);

            _treeView = root.Q<BehaviorTreeView>();
            _inspectorView = root.Q<InspectorView>();
            _treeView.OnNodeSelected = OnNodeSelectionChanged;
            OnSelectionChange();
        }

        private void OnSelectionChange() {
            var tree = Selection.activeObject as BehaviorTree;
            if (tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID())) {
                _treeView.PopulateView(tree);
                
            }
        }

        private void OnNodeSelectionChanged(NodeView nodeView) {
            _inspectorView.UpdateSelection(nodeView);
        }
    }
}
