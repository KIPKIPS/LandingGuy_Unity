// author:KIPKIPS
// date:2023.02.08 19:59
// describe:重复节点
namespace Framework.AI.BehaviorTree {
    public class RepeatNode :DecoratorNode {
        protected override void OnStart() {
            
        }
        protected override void OnStop(){
            
        }
        protected override State OnUpdate() {
            child.Update();
            return State.Running;
        }
    }
}