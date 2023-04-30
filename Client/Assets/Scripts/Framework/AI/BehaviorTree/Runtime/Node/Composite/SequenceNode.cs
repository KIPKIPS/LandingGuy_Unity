// author:KIPKIPS
// date:2023.02.08 20:05
// describe:顺序节点
using System;

namespace Framework.AI {
    public class SequenceNode : CompositeNode {
        private int _current;
        protected override void OnStart() {
            _current = 0;
        }
        protected override void OnStop() {
        }
        protected override State OnUpdate() {
            var child = children[_current];
            switch (child.Update()) {
                case State.Running:
                    return State.Running;
                case State.Failure:
                    return State.Failure;
                case State.Success:
                    _current++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return _current == children.Count ? State.Success : State.Running;
        }
    }
}