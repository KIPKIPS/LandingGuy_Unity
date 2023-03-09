// author:KIPKIPS
// date:2023.02.22 21:16
// describe:延时节点
using UnityEngine;

namespace Framework.AI.BehaviorTree {
    public class WaitNode:ActionNode {
        public float duration = 1;
        private float _startTime;
        protected override void OnStart() {
            _startTime = Time.time;
        }
        protected override State OnUpdate() {
            return Time.time - _startTime >= duration ? State.Success : State.Running;
        }
        protected override void OnStop() {
            
        }
    }
}