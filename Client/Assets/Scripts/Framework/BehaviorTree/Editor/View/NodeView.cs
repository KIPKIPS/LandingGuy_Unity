// author:KIPKIPS
// date:2023.02.08 16:57
// describe:节点视图
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using Framework.AI.BehaviorTree;
using UnityEditor;
using UnityEngine.UIElements;
using Node = Framework.AI.BehaviorTree.Node;
public class NodeView : UnityEditor.Experimental.GraphView.Node {

    public Action<NodeView> OnNodeSelected;
    public readonly Node Node;
    public Port Input;
    public Port Output;
    public NodeView(Node node):base("Assets/Scripts/Framework/BehaviorTree/Editor/VisualTree/NodeView.uxml") {
        Node = node;
        title = node.name;
        viewDataKey = node.guid;
        style.left = node.position.x;
        style.top = node.position.y;

        CreateInputPorts();
        CreateOutputPorts();
        SetupClasses();
    }

    private void SetupClasses() {
        switch (Node) {
            case ActionNode:
                AddToClassList("action");
                break;
            case CompositeNode:
                AddToClassList("composite");
                break;
            case DecoratorNode:
                AddToClassList("decorator");
                break;
            case RootNode:
                AddToClassList("root");
                break;
        }
    }
    public sealed override string title {
        get => base.title;
        set => base.title = value;
    }
    private void CreateInputPorts() {
        PortWrapperView v = new PortWrapperView();
        switch (Node) {
            case ActionNode:
                // Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                Input = PortView.Create<Edge>(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                break;
            case CompositeNode:
                // Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                Input = PortView.Create<Edge>(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                break;
            case DecoratorNode:
                // Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                Input = PortView.Create<Edge>(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                break;
            case RootNode:
                break;
        }
        if (Input is null) return;
        Input.portName = "";
        Input.style.flexDirection = FlexDirection.Column;
        inputContainer.Add(Input);
    }
    private void CreateOutputPorts() {
        PortWrapperView v = new PortWrapperView();
        switch (Node) {
            case ActionNode:
                break;
            case CompositeNode:
                Output = PortView.Create<Edge>(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
                // Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
                break;
            case DecoratorNode:
                // Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
                Output = PortView.Create<Edge>(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
                break;
            case RootNode:
                // Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
                Output = PortView.Create<Edge>(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
                break;
        }
        if (Output is null) return;
        Output.portName = "";
        Output.style.flexDirection = FlexDirection.ColumnReverse;
        outputContainer.Add(Output);
    }
    public override void SetPosition(Rect newPos) {
        base.SetPosition(newPos);
        Undo.RecordObject(Node,"Behavior Tree (Set Position)");
        Node.position.x = newPos.xMin;
        Node.position.y = newPos.yMin;
        EditorUtility.SetDirty(Node);
    }

    public override void OnSelected() {
        base.OnSelected();
        OnNodeSelected?.Invoke(this);
    }

    public void SortChildren() {
        var composite = Node as CompositeNode;
        if (composite) {
            composite.children.Sort(SortByHorizontalPosition);
        }
    }
    int SortByHorizontalPosition(Node left,Node right) {
        return left.position.x < right.position.x ? -1 : 1;
    }

    public void UpdateState() {
        RemoveFromClassList("running");
        RemoveFromClassList("failure");
        RemoveFromClassList("success");
        if (!Application.isPlaying) return;
        switch (Node.state) {
            case Node.State.Running:
                if (Node.started) {
                    AddToClassList("running");
                }
                break;
            case Node.State.Failure:
                AddToClassList("failure");
                break;
            case Node.State.Success:
                AddToClassList("success");
                break;
        }
    }
}
