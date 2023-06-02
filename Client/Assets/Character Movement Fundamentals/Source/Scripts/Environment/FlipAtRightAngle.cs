using UnityEngine;

namespace CMF {
    //This script flips any rigidbody (which also has a 'Controller' attached) that touches its trigger around a 90 degree angle;
    public class FlipAtRightAngle : MonoBehaviour {
        //Audiosource component which is played when switch is triggered;
        private AudioSource _audioSource;

        private Transform _transform;

        private void Start() {
            //Get component references;
            _transform = transform;
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter(Collider col) {
            if (col.GetComponent<Controller>() == null) return;
            SwitchDirection(_transform.forward, col.GetComponent<Controller>());
        }

        private void SwitchDirection(Vector3 newUpDirection, Controller controller) {
            const float angleThreshold = 0.001f;

            //Calculate angle;
            var angleBetweenUpDirections = Vector3.Angle(newUpDirection, controller.transform.up);

            //If angle between new direction and current rigidbody rotation is too small, return;
            if (angleBetweenUpDirections < angleThreshold) return;

            //Play audio cue;
            _audioSource.Play();
            var controllerTransform = controller.transform;

            //Rotate gameobject;
            var rotationDifference = Quaternion.FromToRotation(controllerTransform.up, newUpDirection);
            controllerTransform.rotation = rotationDifference * controllerTransform.rotation;
        }
    }
}