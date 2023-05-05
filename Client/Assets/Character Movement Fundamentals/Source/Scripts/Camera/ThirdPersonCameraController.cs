using UnityEngine;

namespace CMF {
    //This script is a slightly more specialized version of the regular 'CameraController' script, intended for games using a third-person camera.
    //By enabling 'turnCameraTowardMovementDirection', the camera will gradually rotate toward the current movement direction of the gameobject it is attached to;
    //The rate and speed of this rotation can be controlled using 'maximumMovementSpeed' and 'cameraTurnSpeed';
    public class ThirdPersonCameraController : CameraController {
        //Whether or not the camera turns towards the controller's movement direction;
        public bool turnCameraTowardMovementDirection = true;

        public Controller controller;

        //The maximum expected movement speed of this game object;
        //This value should be set to the maximum movement speed achievable by this gameobject;
        //The closer the current movement speed is to 'maximumMovementSpeed', the faster the camera will turn;
        //As a result, if the gameobject moves slower (i.e. "walking" instead of "running", in case of a character), the camera will turn slower as well.
        public float maximumMovementSpeed = 7f;

        //The general rate at which the camera turns toward the movement direction;
        public float cameraTurnSpeed = 120f;

        protected override void Setup() {
            if (controller == null) Debug.LogWarning("No controller reference has been assigned to this script.", this.gameObject);
        }

        protected override void HandleCameraRotation() {
            //Execute normal camera rotation code;
            base.HandleCameraRotation();
            if (controller == null) return;
            if (!turnCameraTowardMovementDirection || controller == null) return;
            //Get controller velocity;
            var controllerVelocity = controller.GetVelocity();
            RotateTowardsVelocity(controllerVelocity, cameraTurnSpeed);
        }

        //Rotate camera toward '_direction', at the rate of '_speed', around the upwards vector of this gameobject;
        private void RotateTowardsVelocity(Vector3 velocity, float speed) {
            //Remove any unwanted components of direction;
            velocity = VectorMath.RemoveDotVector(velocity, GetUpDirection());

            //Calculate angle difference of current direction and new direction;
            var angle = VectorMath.GetAngle(GetFacingDirection(), velocity, GetUpDirection());

            //Calculate sign of angle;
            var sign = Mathf.Sign(angle);

            //Calculate final angle difference;
            var finalAngle = Time.deltaTime * speed * sign * Mathf.Abs(angle / 90f);

            //If angle is greater than 90 degrees, recalculate final angle difference;
            if (Mathf.Abs(angle) > 90f) finalAngle = Time.deltaTime * speed * sign * ((Mathf.Abs(180f - Mathf.Abs(angle))) / 90f);

            //Check if calculated angle overshoots;
            if (Mathf.Abs(finalAngle) > Mathf.Abs(angle)) finalAngle = angle;

            //Take movement speed into account by comparing it to 'maximumMovementSpeed';
            finalAngle *= Mathf.InverseLerp(0f, maximumMovementSpeed, velocity.magnitude);
            SetRotationAngles(GetCurrentXAngle(), GetCurrentYAngle() + finalAngle);
        }
    }
}