﻿// author:KIPKIPS
// date:2023.02.08 08:46
// describe:装饰节点
using UnityEngine;

namespace Framework.AI {
    public abstract class DecoratorNode : Node {
        [HideInInspector]public Node child;
        
        public override Node Clone() {
            var node = Instantiate(this);
            node.child = child.Clone();
            return node;
        }
    }
}
