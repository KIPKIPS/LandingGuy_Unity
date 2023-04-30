// author:KIPKIPS
// date:2023.02.08 14:45
// describe:根节点
namespace Framework.AI {
    public class RootNode:Node {
        public Node child;
        protected override void OnStart() {
            
        }
        protected override State OnUpdate() {
            return child.Update();
        }
        protected override void OnStop() {
        }

        public override Node Clone() {
            var node = Instantiate(this);
            node.child = child.Clone();
            return node;
        }
    }
}