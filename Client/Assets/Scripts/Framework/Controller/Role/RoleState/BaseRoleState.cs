// author:KIPKIPS
// date:2023.04.30 21:17
// describe:
using Framework.AI;

namespace Framework.Controller {
    public abstract class BaseRoleState:BaseState {
        protected RoleController roleController;
        public override void Init(IStateMachineOwner owner, int stateType, StateMachine sm) {
            base.Init(owner, stateType, sm);
            roleController = (RoleController)owner;
        }
    }
}