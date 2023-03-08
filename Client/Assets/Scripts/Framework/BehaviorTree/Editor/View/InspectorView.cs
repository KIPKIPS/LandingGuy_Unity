// author:KIPKIPS
// date:2023.02.08 09:46
// describe:检视面板视图
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class InspectorView : VisualElement {
    public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> {
    }
    private Editor editor;
    public InspectorView() {
    }
    public void UpdateSelection(NodeView nodeView) {
        Clear();
        Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(nodeView.node);
        IMGUIContainer container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
        Add(container);
    }
}