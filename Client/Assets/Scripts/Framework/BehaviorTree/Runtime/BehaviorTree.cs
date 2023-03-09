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
        [HideInInspector]public Node rootNode;
        [HideInInspector]public State treeState = State.Running;
        [HideInInspector]public List<Node> nodes = new ();
        public State Update() {
            if (rootNode.state == State.Running) {
                treeState = rootNode.Update();
            }
            return treeState;
        }

        public Node CreateNode(Type type) {
            var node = CreateInstance(type) as Node;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();
            nodes.Add(node);
            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();
            return node;
        }

        public void DeleteNode(Node node) {
            nodes.Remove(node);
            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
        }

        public void AddChild(Node parent,Node child) {
            var decorator = parent as DecoratorNode;
            if (decorator) {
                decorator.child = child;
            }
            var composite = parent as CompositeNode;
            if (composite) {
                composite.children.Add(child);
            }
            
            var root = parent as RootNode;
            if (root) {
                root.child = child;
            }
        }
        public void RemoveChild(Node parent,Node child) {
            var decorator = parent as DecoratorNode;
            if (decorator) {
                decorator.child = null;
            }
            var composite = parent as CompositeNode;
            if (composite) {
                composite.children.Remove(child);
            }
            var root = parent as RootNode;
            if (root) {
                root.child = null;
            }
        }
        
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

        public BehaviorTree Clone() {
            var tree = Instantiate(this);
            tree.rootNode = tree.rootNode.Clone();
            return tree;
        }
    }
}