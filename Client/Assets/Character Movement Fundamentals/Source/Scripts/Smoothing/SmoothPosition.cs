using UnityEngine;

namespace CMF {
    //This script smoothes the position of a gameobject;
    public class SmoothPosition : MonoBehaviour {
        //The target transform, whose position values will be copied and smoothed;
        public Transform target;
        private Transform _transform;

        private Vector3 _currentPosition;

        //Speed that controls how fast the current position will be smoothed toward the target position when 'Lerp' is selected as smoothType;
        public float lerpSpeed = 20f;

        //Time that controls how fast the current position will be smoothed toward the target position when 'SmoothDamp' is selected as smoothType;
        public float smoothDampTime = 0.02f;

        //Whether position values will be extrapolated to compensate for delay caused by smoothing;
        public bool extrapolatePosition;

        //'UpdateType' controls whether the smoothing function is called in 'Update' or 'LateUpdate';
        public enum UpdateType {
            Update,
            LateUpdate
        }

        public UpdateType updateType;

        //Different smoothtypes use different algorithms to smooth out the target's position; 
        public enum SmoothType {
            Lerp,
            SmoothDamp,
        }

        public SmoothType smoothType;

        //Local position offset at the start of the game;
        private Vector3 _localPositionOffset;

        private Vector3 _refVelocity;

        //Awake;
        private void Awake() {
            //If no target has been selected, choose this transform's parent as the target;
            if (target == null) target = transform.parent;
            var trs = transform;
            _transform = trs;
            _currentPosition = trs.position;
            _localPositionOffset = _transform.localPosition;
        }

        //OnEnable;
        private void OnEnable() {
            //Reset current position when gameobject is re-enabled to prevent unwanted interpolation from last position;
            ResetCurrentPosition();
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
            //Smooth current position;
            _currentPosition = Smooth(_currentPosition, target.position, lerpSpeed);

            //Set position;
            _transform.position = _currentPosition;
        }

        Vector3 Smooth(Vector3 start, Vector3 tar, float smoothTime) {
            //Convert local position offset to world coordinates;
            Vector3 offset = _transform.localToWorldMatrix * _localPositionOffset;

            //If 'extrapolateRotation' is set to 'true', calculate a new target position;
            if (extrapolatePosition) {
                var difference = tar - (start - offset);
                tar += difference;
            }

            //Add local position offset to target;
            tar += offset;

            //Smooth (based on chosen smoothType) and return position;
            switch (smoothType) {
                case SmoothType.Lerp:
                    return Vector3.Lerp(start, tar, Time.deltaTime * smoothTime);
                case SmoothType.SmoothDamp:
                    return Vector3.SmoothDamp(start, tar, ref _refVelocity, smoothDampTime);
                default:
                    return Vector3.zero;
            }
        }

        //Reset stored position and move this gameobject directly to the target's position;
        //Call this function if the target has just been moved a larger distance and no interpolation should take place (teleporting);
        private void ResetCurrentPosition() {
            //Convert local position offset to world coordinates;
            Vector3 offset = _transform.localToWorldMatrix * _localPositionOffset;
            //Add position offset and set current position;
            _currentPosition = target.position + offset;
        }
    }
}