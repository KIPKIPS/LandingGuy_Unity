// author:KIPKIPS
// date:2023.02.08 19:50
// describe:行为树执行器
using UnityEngine;

namespace Framework.AI.BehaviorTree {
    public class BehaviorTreeRunner : MonoBehaviour {
        public BehaviorTree behaviorTree;
        private void Start() {
            behaviorTree = behaviorTree.Clone();
        }
        private void Update() {
            behaviorTree.Update();
        } 
    }
}
