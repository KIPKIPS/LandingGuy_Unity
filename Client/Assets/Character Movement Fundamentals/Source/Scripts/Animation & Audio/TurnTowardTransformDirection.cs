using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMF {
    //This script rotates an object toward the 'forward' direction of another target transform;
    public class TurnTowardTransformDirection : MonoBehaviour {
        public Transform targetTransform;
        private Transform _transform;
        private Transform _parentTransform;

        //Setup;
        private void Start() {
            _transform = transform;
            _parentTransform = _transform.parent;
            if (targetTransform == null) Debug.LogWarning("No target transform has been assigned to this script.", this);
        }

        //Update;
        private void LateUpdate() {
            if (!targetTransform) return;

            //Calculate up and forward direction;
            var up = _parentTransform.up;
            var forwardDirection = Vector3.ProjectOnPlane(targetTransform.forward, up).normalized;

            //Set rotation;
            _transform.rotation = Quaternion.LookRotation(forwardDirection, up);
        }
    }
}