// author:KIPKIPS
// date:2023.04.29 20:23
// describe:
using Framework.Pool;

namespace Framework.AI {
    public class BaseState{
        protected StateMachine stateMachine;
        /// <summary>
        /// 初始化状态
        /// 只在状态第一次创建时执行
        /// </summary>
        /// <param name="owner">宿主</param>
        /// <param name="stateType">状态类型枚举的值</param>
        /// <param name="sm"></param>
        public virtual void Init(IStateMachineOwner owner, int stateType, StateMachine sm) {
            stateMachine = sm;
        }

        /// <summary>
        /// 反初始化
        /// 不再使用时候，放回对象池时调用
        /// 把一些引用置空，防止不能被GC
        /// </summary>
        public virtual void UnInit() { }

        /// <summary>
        /// 状态进入
        /// 每次进入都会执行
        /// </summary>
        public virtual void Enter() { }

        /// <summary>
        /// 状态退出
        /// </summary>
        public virtual void Exit() { }

        public virtual void Update() { }
        public virtual void LateUpdate() { }
        public virtual void FixedUpdate() { }
    }
}