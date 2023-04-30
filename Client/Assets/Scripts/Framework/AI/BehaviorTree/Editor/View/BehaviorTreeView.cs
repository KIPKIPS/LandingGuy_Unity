// author:KIPKIPS
// date:2023.02.08 08:46
// describe:树视图
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using Framework.AI;
using Node = Framework.AI.Node;
public class BehaviorTreeView : GraphView {
    public Action<NodeView> OnNodeSelected;
    public new class UxmlFactory : UxmlFactory<BehaviorTreeView, UxmlTraits> {
    }

    private BehaviorTree _tree;
    public BehaviorTreeView() {
        Insert(0, new GridBackground());
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Framework/AI/BehaviorTree/Editor/StyleSheet/BehaviorTree.uss");
        styleSheets.Add(styleSheet);

        Undo.undoRedoPerformed += OnUndoRecord;
    }

    private void OnUndoRecord() {
        PopulateView(_tree);
        AssetDatabase.SaveAssets();
    }
    public void PopulateView(BehaviorTree bt) {
        _tree = bt;
        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        if (bt.rootNode == null) {
            bt.rootNode = _tree.CreateNode(typeof(RootNode)) as RootNode;
            EditorUtility.SetDirty(bt);
            AssetDatabase.SaveAssets();
        }
        //create node view
        bt.nodes.ForEach(CreateNodeView);
        
        //create edge
        bt.nodes.ForEach(n => {
            var children = bt.GetChildren(n);
            children.ForEach(c => {
                var parentView = FindNodeView(n);
                var childView = FindNodeView(c);
                var edge = parentView.Output.ConnectTo(childView.Input);
                AddElement(edge);
            });
        });
    }
    private NodeView FindNodeView(Node node) {
        return GetNodeByGuid(node.guid) as NodeView;
    }
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) {
        return ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
    }
    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange) {
        graphViewChange.elementsToRemove?.ForEach(elem => {
            switch (elem) {
                case NodeView nodeView:
                    _tree.DeleteNode(nodeView.Node);
                    break;
                case Edge edge: {
                    var parentView = edge.output.node as NodeView;
                    var childView = edge.input.node as NodeView;
                    _tree.RemoveChild(parentView.Node,childView.Node);
                    break;
                }
            }
        });
        graphViewChange.edgesToCreate?.ForEach(edge => {
            var parentView = edge.output.node as NodeView;
            var childView = edge.input.node as NodeView;
            _tree.AddChild(parentView.Node,childView.Node);
        });
        if (graphViewChange.movedElements != null) {
            graphViewChange.movedElements.ForEach(n => {
                NodeView view = n as NodeView;
                view.SortChildren();
            });
        }
        return graphViewChange;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt) {
        foreach (var type in TypeCache.GetTypesDerivedFrom<ActionNode>()) {
            evt.menu.AppendAction($"{type.BaseType.Name}/{type.Name}", _ => CreateNode(type));
        }
        foreach (var type in TypeCache.GetTypesDerivedFrom<CompositeNode>()) {
            evt.menu.AppendAction($"{type.BaseType.Name}/{type.Name}", _ => CreateNode(type));
        }
        foreach (var type in TypeCache.GetTypesDerivedFrom<DecoratorNode>()) {
            evt.menu.AppendAction($"{type.BaseType.Name}/{type.Name}", _ => CreateNode(type));
        }
    }
    private void CreateNode(Type type) {
        var node = _tree.CreateNode(type);
        CreateNodeView(node);
    }
    private void CreateNodeView(Node node) {
        var nodeView = new NodeView(node);
        nodeView.OnNodeSelected += OnNodeSelected; 
        AddElement(nodeView);
    }

    public void UpdateNodeStates() {
        nodes.ForEach(n => {
            NodeView v = n as NodeView;
            v.UpdateState();
        });
    }
}
