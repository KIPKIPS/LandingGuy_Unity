﻿// author:KIPKIPS
// date:2023.02.08 16:57
// describe:节点视图
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using Framework.AI.BehaviorTree;
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
    }
    public sealed override string title {
        get => base.title;
        set => base.title = value;
    }
    private void CreateInputPorts() {
        switch (Node) {
            case ActionNode:
                Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                break;
            case CompositeNode:
                Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                break;
            case DecoratorNode:
                Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
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
        switch (Node) {
            case ActionNode:
                break;
            case CompositeNode:
                Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
                break;
            case DecoratorNode:
                Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
                break;
            case RootNode:
                Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
                break;
        }
        if (Output is null) return;
        Output.portName = "";
        Output.style.flexDirection = FlexDirection.ColumnReverse;
        outputContainer.Add(Output);
    }
    public override void SetPosition(Rect newPos) {
        base.SetPosition(newPos);
        Node.position.x = newPos.xMin;
        Node.position.y = newPos.yMin;
    }

    public override void OnSelected() {
        base.OnSelected();
        OnNodeSelected?.Invoke(this);
    }
}
