// author:KIPKIPS
// date:2023.02.08 08:46
// describe:树视图
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using Framework.AI.BehaviorTree;
using Node = Framework.AI.BehaviorTree.Node;
public class BehaviorTreeView : GraphView {
    public Action<NodeView> OnNodeSelected;
    public new class UxmlFactory : UxmlFactory<BehaviorTreeView, UxmlTraits> {
    }

    private BehaviorTree tree;
    public BehaviorTreeView() {
        Insert(0, new GridBackground());
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Framework/BehaviorTree/Editor/BehaviorTreeEditor.uss");
        styleSheets.Add(styleSheet);
    }
    public void PopulateView(BehaviorTree bt) {
        this.tree = bt;
        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        if (bt.rootNode == null) {
            bt.rootNode = tree.CreateNode(typeof(RootNode)) as RootNode;
            EditorUtility.SetDirty(bt);
            AssetDatabase.SaveAssets();
        }
        //create node view
        bt.nodes.ForEach(CreateNodeView);
        
        //create edge
        bt.nodes.ForEach(n => {
            var children = bt.GetChildren(n);
            children.ForEach(c => {
                NodeView parentView = FindNodeView(n);
                NodeView childView = FindNodeView(c);
                Edge edge = parentView.output.ConnectTo(childView.input);
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
        if (graphViewChange.elementsToRemove != null) {
            graphViewChange.elementsToRemove.ForEach(elem => {
                if (elem is NodeView nodeView) {
                    tree.DeleteNode(nodeView.node);
                }
                if (elem is Edge edge) {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    tree.RemoveChild(parentView.node,childView.node);
                }
            });
        }
        if (graphViewChange.edgesToCreate != null) {
            graphViewChange.edgesToCreate.ForEach(edge => {
                NodeView parentView = edge.output.node as NodeView;
                NodeView childView = edge.input.node as NodeView;
                tree.AddChild(parentView.node,childView.node);
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
        Node node = tree.CreateNode(type);
        CreateNodeView(node);
    }
    private void CreateNodeView(Node node) {
        NodeView nodeView = new NodeView(node);
        nodeView.OnNodeSelected += OnNodeSelected; 
        AddElement(nodeView);
    }
}
