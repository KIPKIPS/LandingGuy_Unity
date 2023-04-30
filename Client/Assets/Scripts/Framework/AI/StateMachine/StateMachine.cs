// author:KIPKIPS
// date:2023.04.29 20:24
// describe:
using System.Collections.Generic;
using Framework.Pool;

namespace Framework.AI {
    public interface IStateMachineOwner {
    }
    public class StateMachine {
        // 当前状态
        public int CurrStateType { get; private set; } = -1;
        // 当前生效中的状态
        private BaseState currStateObj;

        // 宿主
        private IStateMachineOwner owner;

        // 所有的状态 Key:状态枚举的值 Value:具体的状态
        private readonly Dictionary<int, BaseState> stateDic = new();
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="o"></param>
        public void Init(IStateMachineOwner o) {
            owner = o;
        }
        /// <summary>
        /// 切换状态
        /// </summary>
        /// <typeparam name="T">具体要切换到的状态脚本类型</typeparam>
        /// <param name="newState">新状态</param>
        /// <param name="newStateType"></param>
        /// <param name="reCurState">新状态和当前状态一致的情况下，是否也要切换</param>
        /// <returns></returns>
        public bool ChangeState<T>(int newStateType, bool reCurState = false) where T : BaseState, new() {
            // 状态一致，并且不需要刷新状态，则切换失败
            if (newStateType == CurrStateType && !reCurState) return false;

            // 退出当前状态
            if (currStateObj != null) {
                currStateObj.Exit();
                LUtil.RemoveUpdate(currStateObj.Update);
                LUtil.RemoveLateUpdate(currStateObj.LateUpdate);
                LUtil.RemoveFixedUpdate(currStateObj.FixedUpdate);
            }
            // 进入新状态
            currStateObj = GetState<T>(newStateType);
            CurrStateType = newStateType;
            currStateObj.Enter();
            LUtil.AddUpdate(currStateObj.Update);
            LUtil.AddLateUpdate(currStateObj.LateUpdate);
            LUtil.AddFixedUpdate(currStateObj.FixedUpdate);
            return true;
        }
        /// <summary>
        /// 从对象池获取一个状态
        /// </summary>
        private BaseState GetState<T>(int stateType) where T : BaseState, new() {
            if (stateDic.ContainsKey(stateType)) return stateDic[stateType];
            // BaseState state = ResManager.Load<T>();
            BaseState state = new T();
            state.Init(owner, stateType, this);
            stateDic.Add(stateType, state);
            return state;
        }
        /// <summary>
        /// 停止工作
        /// 把所有状态都释放，但是StateMachine未来还可以工作
        /// </summary>
        public void Stop() {
            // 处理当前状态的额外逻辑
            currStateObj.Exit();
            LUtil.RemoveUpdate(currStateObj.Update);
            LUtil.RemoveLateUpdate(currStateObj.LateUpdate);
            LUtil.RemoveFixedUpdate(currStateObj.FixedUpdate);
            CurrStateType = -1;
            currStateObj = null;
            // 处理缓存中所有状态的逻辑
            using var enumerator = stateDic.GetEnumerator();
            while (enumerator.MoveNext()) {
                enumerator.Current.Value.UnInit();
            }
            stateDic.Clear();
        }
        /// <summary>
        /// 销毁，宿主应该释放掉StateMachine的引用
        /// </summary>
        public void Destroy() {
            owner = null;
            Stop();
        }
    }
}