using UnityEngine;

namespace CMF {
    //This character movement input class is an example of how to get input from a gamepad/joystick to control the character;
    //It comes with a dead zone threshold setting to bypass any unwanted joystick "jitter";
    public class CharacterJoystickInput : CharacterInput {
        public string horizontalInputAxis = "Horizontal";
        public string verticalInputAxis = "Vertical";
        public KeyCode jumpKey = KeyCode.Joystick1Button0;

        //If this is enabled, Unity's internal input smoothing is bypassed;
        public bool useRawInput = true;

        //If any input falls below this value, it is set to '0';
        //Use this to prevent any unwanted small movements of the joysticks ("jitter");
        public float deadZoneThreshold = 0.2f;

        public override float GetHorizontalMovementInput() {
            var horizontalInput = useRawInput ? Input.GetAxisRaw(horizontalInputAxis) : Input.GetAxis(horizontalInputAxis);
            //Set any input values below threshold to '0';
            if (Mathf.Abs(horizontalInput) < deadZoneThreshold) horizontalInput = 0f;
            return horizontalInput;
        }

        public override float GetVerticalMovementInput() {
            var verticalInput = useRawInput ? Input.GetAxisRaw(verticalInputAxis) : Input.GetAxis(verticalInputAxis);
            //Set any input values below threshold to '0';
            if (Mathf.Abs(verticalInput) < deadZoneThreshold) verticalInput = 0f;
            return verticalInput;
        }

        public override bool IsJumpKeyPressed() {
            return Input.GetKey(jumpKey);
        }
    }
}