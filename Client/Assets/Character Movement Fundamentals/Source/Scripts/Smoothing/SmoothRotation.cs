using UnityEngine;

namespace CMF {
    //This script smoothes the rotation of a gameobject;
    public class SmoothRotation : MonoBehaviour {
        //The target transform, whose rotation values will be copied and smoothed;
        public Transform target;
        private Transform _transform;

        private Quaternion _currentRotation;

        //Speed that controls how fast the current rotation will be smoothed toward the target rotation;
        public float smoothSpeed = 20f;

        //Whether rotation values will be extrapolated to compensate for delay caused by smoothing;
        public bool extrapolateRotation;

        //'UpdateType' controls whether the smoothing function is called in 'Update' or 'LateUpdate';
        public enum UpdateType {
            Update,
            LateUpdate
        }

        public UpdateType updateType;

        //Awake;
        private void Awake() {
            //If no target has been selected, choose this transform's parent as target;
            if (target == null) target = transform.parent;
            var t = transform;
            _transform = t;
            _currentRotation = t.rotation;
        }

        //OnEnable;
        private void OnEnable() {
            //Reset current rotation when gameobject is re-enabled to prevent unwanted interpolation from last rotation;
            ResetCurrentRotation();
        }

        private void Update() {
            if (updateType == UpdateType.LateUpdate) return;
            SmoothUpdate();
        }

        private void LateUpdate() {
            if (updateType == UpdateType.Update) return;
            SmoothUpdate();
        }

        private void SmoothUpdate() {
            //Smooth current rotation;
            _currentRotation = Smooth(_currentRotation, target.rotation, smoothSpeed);

            //Set rotation;
            _transform.rotation = _currentRotation;
        }

        //Smooth a rotation toward a target rotation based on 'smoothTime';
        private Quaternion Smooth(Quaternion curRotation, Quaternion targetRotation, float smooth) {
            //If 'extrapolateRotation' is set to 'true', calculate a new target rotation;
            if (!extrapolateRotation || !(Quaternion.Angle(curRotation, targetRotation) < 90f)) return Quaternion.Slerp(curRotation, targetRotation, Time.deltaTime * smooth);
            var difference = targetRotation * Quaternion.Inverse(curRotation);
            targetRotation *= difference;

            //Slerp rotation and return;
            return Quaternion.Slerp(curRotation, targetRotation, Time.deltaTime * smooth);
        }

        //Reset stored rotation and rotate this gameobject to macth the target's rotation;
        //Call this function if the target has just been rotatedand no interpolation should take place (instant rotation);
        private void ResetCurrentRotation() {
            _currentRotation = target.rotation;
        }
    }
}