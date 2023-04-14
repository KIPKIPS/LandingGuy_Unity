// author:KIPKIPS
// date:2023.04.14 17:25
// describe:ui绑定器检视面板
using UnityEditor;

namespace Framework.UI {
    [CustomEditor(typeof(UIBinding))]
    public class UIBindingInspector : Editor {
        private SerializedProperty _pageProperty;
        public override void OnInspectorGUI() {
            EditorGUILayout.PropertyField(_pageProperty);
            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable() {
            _pageProperty = serializedObject.FindProperty("_page");
        }
    }
}