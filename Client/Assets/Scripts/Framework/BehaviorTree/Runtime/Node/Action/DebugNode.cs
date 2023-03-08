// author:KIPKIPS
// date:2023.02.08 08:46
// describe:日志节点
using UnityEngine;

namespace Framework.AI.BehaviorTree {
    public class DebugNode : ActionNode {
        public string message;
        protected override void OnStart() {
            Debug.Log($"OnStart{message}");
        }
        protected override State OnUpdate() {
            Debug.Log($"OnUpdate{message}");
            return State.Success;
        }
        protected override void OnStop() {
            Debug.Log($"OnStop{message}");
        }
    }
}
