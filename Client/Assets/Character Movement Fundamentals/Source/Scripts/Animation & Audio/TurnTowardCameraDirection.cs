using UnityEngine;

namespace CMF {
    //This script turns a gameobject toward the look direction of a chosen 'CameraController' component;
    public class TurnTowardCameraDirection : MonoBehaviour {
        public CameraController cameraController;
        private Transform _transform;

        //Setup;
        private void Start() {
            _transform = transform;
            if (cameraController == null) Debug.LogWarning("No camera controller reference has been assigned to this script.", this);
        }

        //Update;
        private void LateUpdate() {
            if (!cameraController) return;

            //Calculate up and forwward direction;
            var forwardDirection = cameraController.GetFacingDirection();
            var upDirection = cameraController.GetUpDirection();

            //Set rotation;
            _transform.rotation = Quaternion.LookRotation(forwardDirection, upDirection);
        }
    }
}