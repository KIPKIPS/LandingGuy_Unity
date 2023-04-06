// author:KIPKIPS
// date:2023.02.08 09:46
// describe:检视面板视图
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class InspectorView : VisualElement {
    public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> {
    }
    private Editor _editor;
    public InspectorView() {
    }
    public void UpdateSelection(NodeView nodeView) {
        Clear();
        Object.DestroyImmediate(_editor);
        _editor = Editor.CreateEditor(nodeView.Node);
        var container = new IMGUIContainer(() => {
            if (_editor.target) {
                _editor.OnInspectorGUI();
            }
        });
        Add(container);
    }
}