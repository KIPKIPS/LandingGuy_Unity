// author:KIPKIPS
// date:2023.04.07 00:59
// describe:
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class PortWrapperView : VisualElement {
    public new class UxmlFactory : UxmlFactory<PortWrapperView, UxmlTraits> {
    }
    private PortView portView;
    public PortWrapperView() {
        
    }
    public PortView CreatePortView(Orientation o, Direction d, Port.Capacity c, System.Type t) {
        portView = PortView.Create<Edge>(o, d, c, t);
        return portView;
    }
}
