// author:KIPKIPS
// date:2023.04.30 21:20
// describe:
namespace Framework.Controller {
    public class IdleState:BaseRoleState {
        // public IdleState() {
        //     
        // }
        public override void Enter() {
            roleController.ChangeState(RoleState.Idle);
        }
    }
}