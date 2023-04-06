// author:KIPKIPS
// date:2023.02.08 08:46
// describe:行为树
using System;
using System.Collections.Generic;
using UnityEngine;
using State = Framework.AI.BehaviorTree.Node.State;
using UnityEditor;

namespace Framework.AI.BehaviorTree {
    // ReSharper disable MemberCanBeMadeStatic.Global
    [CreateAssetMenu]
    public class BehaviorTree : ScriptableObject {
        internal const string LOGTag = "BehaviorTree";
        [HideInInspector] public Node rootNode;
        [HideInInspector] public State treeState = State.Running;
        [HideInInspector] public List<Node> nodes = new();
        public State Update() {
            if (rootNode && rootNode.state == State.Running) {
                treeState = rootNode.Update();
            }
            return treeState;
        }
#if UNITY_EDITOR
        public Node CreateNode(Type type) {
            var node = CreateInstance(type) as Node;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();
            Undo.RecordObject(this, "Behavior Tree (CreateNode)");
            nodes.Add(node);
            if (!Application.isPlaying) {
                AssetDatabase.AddObjectToAsset(node, this);
            }
            Undo.RegisterCreatedObjectUndo(node, "Behavior Tree (CreateNode)");
            AssetDatabase.SaveAssets();
            return node;
        }

        public void DeleteNode(Node node) {
            Undo.RecordObject(this, "Behavior Tree (DeleteNode)");
            nodes.Remove(node);
            // AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);
            // EditorUtility.SetDirty(node);
            AssetDatabase.SaveAssets();
        }

        public void AddChild(Node parent, Node child) {
            var decorator = parent as DecoratorNode;
            if (decorator) {
                Undo.RecordObject(decorator, "Behavior Tree (AddChild)");
                decorator.child = child;
                EditorUtility.SetDirty(decorator);
            }
            var composite = parent as CompositeNode;
            if (composite) {
                Undo.RecordObject(composite, "Behavior Tree (AddChild)");
                composite.children.Add(child);
                EditorUtility.SetDirty(composite);
            }
            var root = parent as RootNode;
            if (root) {
                Undo.RecordObject(root, "Behavior Tree (AddChild)");
                root.child = child;
                EditorUtility.SetDirty(root);
            }
        }
        public void RemoveChild(Node parent, Node child) {
            var decorator = parent as DecoratorNode;
            if (decorator) {
                Undo.RecordObject(decorator, "Behavior Tree (AddChild)");
                decorator.child = null;
                EditorUtility.SetDirty(decorator);
            }
            var composite = parent as CompositeNode;
            if (composite) {
                Undo.RecordObject(composite, "Behavior Tree (AddChild)");
                composite.children.Remove(child);
                EditorUtility.SetDirty(composite);
            }
            var root = parent as RootNode;
            if (root) {
                Undo.RecordObject(root, "Behavior Tree (AddChild)");
                root.child = null;
                EditorUtility.SetDirty(root);
            }
        }
#endif
        public List<Node> GetChildren(Node parent) {
            var composite = parent as CompositeNode;
            if (composite) {
                return composite.children;
            }
            var children = new List<Node>();
            var decorator = parent as DecoratorNode;
            if (decorator && decorator.child != null) {
                children.Add(decorator.child);
            }
            var root = parent as RootNode;
            if (root && root.child != null) {
                children.Add(root.child);
            }
            return children;
        }


        private void Traverse(Node node, Action<Node> callback) {
            if (!node) return;
            callback.Invoke(node);
            var children = GetChildren(node);
            if (children == null) return;
            foreach (var c in children) {
                Traverse(c, callback);
            }
        }

        public BehaviorTree Clone() {
            var tree = Instantiate(this);
            tree.rootNode = tree.rootNode.Clone();
            tree.nodes = new List<Node>();
            Traverse(tree.rootNode, n => { tree.nodes.Add(n); });
            return tree;
        }
    }
}