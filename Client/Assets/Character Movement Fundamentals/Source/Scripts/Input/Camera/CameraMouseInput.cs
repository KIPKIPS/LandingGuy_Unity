using UnityEngine;

namespace CMF {
    //This camera input class is an example of how to get input from a connected mouse using Unity's default input system;
    //It also includes an optional mouse sensitivity setting;
    public class CameraMouseInput : CameraInput {
        //Mouse input axes;
        public string mouseHorizontalAxis = "Mouse X";
        public string mouseVerticalAxis = "Mouse Y";

        //Invert input options;
        public bool invertHorizontalInput;
        public bool invertVerticalInput;

        //Use this value to fine-tune mouse movement;
        //All mouse input will be multiplied by this value;
        public float mouseInputMultiplier = 0.01f;

        public override float GetHorizontalCameraInput() {
            //Get raw mouse input;
            var input = Input.GetAxisRaw(mouseHorizontalAxis);

            //Since raw mouse input is already time-based, we need to correct for this before passing the input to the camera controller;
            if (Time.timeScale > 0f && Time.deltaTime > 0f) {
                input /= Time.deltaTime;
                input *= Time.timeScale;
            } else
                input = 0f;

            //Apply mouse sensitivity;
            input *= mouseInputMultiplier;

            //Invert input;
            if (invertHorizontalInput) input *= -1f;
            return input;
        }

        public override float GetVerticalCameraInput() {
            //Get raw mouse input;
            var input = -Input.GetAxisRaw(mouseVerticalAxis);

            //Since raw mouse input is already time-based, we need to correct for this before passing the input to the camera controller;
            if (Time.timeScale > 0f && Time.deltaTime > 0f) {
                input /= Time.deltaTime;
                input *= Time.timeScale;
            } else
                input = 0f;

            //Apply mouse sensitivity;
            input *= mouseInputMultiplier;

            //Invert input;
            if (invertVerticalInput) input *= -1f;
            return input;
        }
    }
}