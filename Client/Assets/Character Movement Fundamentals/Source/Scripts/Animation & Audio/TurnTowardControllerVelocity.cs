using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMF {
    //This script turns a gameobject toward the target controller's velocity direction;
    public class TurnTowardControllerVelocity : MonoBehaviour {
        //Target controller;
        public Controller controller;

        //Speed at which this gameobject turns toward the controller's velocity;
        public float turnSpeed = 500f;

        private Transform _parentTransform;
        private Transform _transform;

        //Current (local) rotation around the (local) y axis of this gameobject;
        private float _currentYRotation;

        //If the angle between the current and target direction falls below 'fallOffAngle', 'turnSpeed' becomes progressively slower (and eventually approaches '0f');
        //This adds a smoothing effect to the rotation;
        private const float FallOffAngle = 90f;

        //Whether the current controller momentum should be ignored when calculating the new direction;
        public bool ignoreControllerMomentum;

        //Setup;
        private void Start() {
            _transform = transform;
            _parentTransform = _transform.parent;

            //Throw warning if no controller has been assigned;
            if (controller != null) return;
            Debug.LogWarning("No controller script has been assigned to this 'TurnTowardControllerVelocity' component!", this);
            enabled = false;
        }

        private void LateUpdate() {
            //Get controller velocity;
            var velocity = ignoreControllerMomentum?controller.GetMovementVelocity():controller.GetVelocity();
            //Project velocity onto a plane defined by the 'up' direction of the parent transform;
            velocity = Vector3.ProjectOnPlane(velocity, _parentTransform.up);
            const float magnitudeThreshold = 0.001f;
            //If the velocity's magnitude is smaller than the threshold, return;
            if (velocity.magnitude < magnitudeThreshold) return;
            //Normalize velocity direction;
            velocity.Normalize();
            //Get current 'forward' vector;
            var currentForward = _transform.forward;
            //Calculate (signed) angle between velocity and forward direction;
            var angleDifference = VectorMath.GetAngle(currentForward, velocity, _parentTransform.up);
            //Calculate angle factor;
            var factor = Mathf.InverseLerp(0f, FallOffAngle, Mathf.Abs(angleDifference));
            //Calculate this frame's step;
            var step = Mathf.Sign(angleDifference) * factor * Time.deltaTime * turnSpeed;
            //Clamp step;
            if (angleDifference < 0f && step < angleDifference) {
                step = angleDifference;
            } else if (angleDifference > 0f && step > angleDifference) {
                step = angleDifference;
            }

            //Add step to current y angle;
            _currentYRotation += step;
            //Clamp y angle;
            if (_currentYRotation > 360f) _currentYRotation -= 360f;
            if (_currentYRotation < -360f) _currentYRotation += 360f;
            //Set transform rotation using Quaternion.Euler;
            _transform.localRotation = Quaternion.Euler(0f, _currentYRotation, 0f);
        }

        private void OnEnable() {
            _currentYRotation = transform.localEulerAngles.y;
        }
    }
}