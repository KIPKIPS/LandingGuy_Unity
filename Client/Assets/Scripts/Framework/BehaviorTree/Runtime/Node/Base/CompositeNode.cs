// author:KIPKIPS
// date:2023.02.08 08:46
// describe:复合节点
using System.Collections.Generic;
using UnityEngine;

namespace Framework.AI.BehaviorTree {
    public abstract class CompositeNode : Node {
        [HideInInspector]public List<Node> children = new List<Node>();
        public override Node Clone() {
            CompositeNode node = Instantiate(this);
            
            node.children = children.ConvertAll(n=>n.Clone());
            return node;
        }
    }
}
