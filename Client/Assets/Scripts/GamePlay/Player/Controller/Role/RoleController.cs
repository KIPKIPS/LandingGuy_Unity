// author:KIPKIPS
// date:2023.04.29 17:34
// describe:
using System;
using UnityEngine;
using Framework.AI;
namespace GamePlay.Player {
    public class RoleController:MonoBehaviour,IStateMachineOwner {
        public AnimationClip[] clips;
        private AnimationController animCtrl;
        private StateMachine _stateMachine;

        private RoleState _roleState;
        private void Start() {
            animCtrl = GetComponent<AnimationController>();
            animCtrl.Init();
            _stateMachine = new StateMachine();
            _stateMachine.Init(this);
            ChangeState(RoleState.Idle);
        }
        public void ChangeState(RoleState state) {
            _roleState = state;
            switch (state) {
                case RoleState.Idle :
                    _stateMachine.ChangeState<IdleState>((int)state);
                    animCtrl.PlayAnimation(clips[0]);//todo:走配置
                    break;
            }
        }
    }
}