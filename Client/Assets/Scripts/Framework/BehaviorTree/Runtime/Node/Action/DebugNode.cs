// author:KIPKIPS
// date:2023.02.08 08:46
// describe:日志节点

namespace Framework.AI.BehaviorTree {
    public class DebugNode : ActionNode {
        public string message;
        protected override void OnStart() {
            Utils.Log(BehaviorTree.LOGTag,message);
        }
        protected override State OnUpdate() {
            return State.Success;
        }
        protected override void OnStop() {
        }
    }
}
