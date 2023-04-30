// author:KIPKIPS
// date:2023.02.08 08:46
// describe:节点基类
using UnityEngine;

namespace Framework.AI {
    public abstract class Node : ScriptableObject {
        public enum State {
            Running,Failure,Success,
        }

        [HideInInspector]public State state;
        [HideInInspector]public bool started;
        [HideInInspector]public string guid;
        [HideInInspector]public Vector2 position;
        [HideInInspector]public Blackboard blackboard;
        [HideInInspector]public float number;
        /// <summary>
        /// 更新逻辑
        /// </summary>
        /// <returns></returns>
        public State Update() {
            if (!started) {
                OnStart();
                started = true;
            }
            state = OnUpdate();
            if (state is not (State.Failure or State.Success)) return state;
            OnStop();
            started = false;
            return state;
        }

        protected abstract void OnStart();
        protected abstract State OnUpdate();
        protected abstract void OnStop();

        public virtual Node Clone() {
            return Instantiate(this);
        }
    }
    
}
