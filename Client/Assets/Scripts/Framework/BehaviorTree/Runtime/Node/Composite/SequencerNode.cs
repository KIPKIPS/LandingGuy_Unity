// author:KIPKIPS
// date:2023.02.08 20:05
// describe:顺序节点
namespace Framework.AI.BehaviorTree {
    public class SequencerNode:CompositeNode {
        private int current;
        protected override void OnStart() {
            current = 0;
        }
        protected override void OnStop() {
            
        }
        protected override State OnUpdate() {
            var child = children[current];
            switch (child.Update()) {
                case State.Running:
                    return State.Running;
                case State.Failure:
                    return State.Failure;
                case State.Success:
                    current++;
                    break;
            }
            return current == children.Count ? State.Success : State.Running;
        }
    }
}