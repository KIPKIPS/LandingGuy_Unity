// author:KIPKIPS
// date:2023.02.22 21:16
// describe:延时节点
using UnityEngine;

namespace Framework.AI.BehaviorTree {
    public class WaitNode:ActionNode {
        public float duration = 1;
        private float startTime;
        protected override void OnStart() {
            startTime = Time.time;
        }
        protected override State OnUpdate() {
            if (Time.time - startTime >= duration) {
                return State.Success;
            }
            return State.Running;
        }
        protected override void OnStop() {
            
        }
    }
}