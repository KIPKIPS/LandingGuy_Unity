// author:KIPKIPS
// date:2023.02.08 16:57
// describe:节点视图
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using Framework.AI.BehaviorTree;
using Node = Framework.AI.BehaviorTree.Node;
public class NodeView : UnityEditor.Experimental.GraphView.Node {

    public Action<NodeView> OnNodeSelected;
    public readonly Node node;
    public Port input;
    public Port output;
    
    public NodeView(Node node) {
        this.node = node;
        title = node.name;
        viewDataKey = node.guid;
        style.left = node.position.x;
        style.top = node.position.y;

        CreateInputPorts();
        CreateOutputPorts();
    }
    public sealed override string title {
        get => base.title;
        set => base.title = value;
    }
    private void CreateInputPorts() {
        switch (node) {
            case ActionNode:
                input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
                break;
            case CompositeNode:
                input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
                break;
            case DecoratorNode:
                input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
                break;
            case RootNode:
                break;
        }
        if (!(input is null)) {
            input.portName = "";
            inputContainer.Add(input);
        }
    }
    private void CreateOutputPorts() {
        switch (node) {
            case ActionNode:
                break;
            case CompositeNode:
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
                break;
            case DecoratorNode:
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                break;
            case RootNode:
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                break;
        }
        if (!(output is null)) {
            output.portName = "";
            outputContainer.Add(output);
        }
    }
    public override void SetPosition(Rect newPos) {
        base.SetPosition(newPos);
        node.position.x = newPos.xMin;
        node.position.y = newPos.yMin;
    }

    public override void OnSelected() {
        base.OnSelected();
        OnNodeSelected?.Invoke(this);
    }
}
