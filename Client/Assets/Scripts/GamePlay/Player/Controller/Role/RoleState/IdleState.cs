// author:KIPKIPS
// date:2023.04.30 21:20
// describe:
namespace GamePlay.Player {
    public class IdleState:BaseRoleState {
        public override void Enter() {
            roleController.ChangeState(RoleState.Idle);
        }
    }
}