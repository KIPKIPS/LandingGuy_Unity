using UnityEngine;

namespace CMF {
    //This script controls the character's animation by passing velocity values and other information ('isGrounded') to an animator component;
    public class AnimationControl : MonoBehaviour {
        private Controller _controller;
        private Animator _animator;
        private Transform _animatorTransform;
        private Transform _transform;

        //Whether the character is using the strafing blend tree;
        public bool useStrafeAnimations;

        //Velocity threshold for landing animation;
        //Animation will only be triggered if downward velocity exceeds this threshold;
        public float landVelocityThreshold = 5f;

        private const float SmoothingFactor = 40f;
        private Vector3 _oldMovementVelocity = Vector3.zero;
        private static readonly int VerticalSpeed = Animator.StringToHash("VerticalSpeed");
        private static readonly int HorizontalSpeed = Animator.StringToHash("HorizontalSpeed");
        private static readonly int ForwardSpeed = Animator.StringToHash("ForwardSpeed");
        private static readonly int StrafeSpeed = Animator.StringToHash("StrafeSpeed");
        private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
        private static readonly int IsStrafing = Animator.StringToHash("IsStrafing");
        private static readonly int Land = Animator.StringToHash("OnLand");

        //Setup;
        private void Awake() {
            _controller = GetComponent<Controller>();
            _animator = GetComponentInChildren<Animator>();
            _animatorTransform = _animator.transform;
            _transform = transform;
        }

        //OnEnable;
        void OnEnable() {
            //Connect events to controller events;
            _controller.OnLand += OnLand;
            _controller.OnJump += OnJump;
        }

        //OnDisable;
        void OnDisable() {
            //Disconnect events to prevent calls to disabled gameobjects;
            _controller.OnLand -= OnLand;
            _controller.OnJump -= OnJump;
        }

        //Update;
        void Update() {
            //Get controller velocity;
            var velocity = _controller.GetVelocity();

            //Split up velocity;
            var up = _transform.up;
            var horizontalVelocity = VectorMath.RemoveDotVector(velocity, up);
            var verticalVelocity = velocity - horizontalVelocity;

            //Smooth horizontal velocity for fluid animation;
            horizontalVelocity = Vector3.Lerp(_oldMovementVelocity, horizontalVelocity, SmoothingFactor * Time.deltaTime);
            _oldMovementVelocity = horizontalVelocity;
            _animator.SetFloat(VerticalSpeed, verticalVelocity.magnitude * VectorMath.GetDotProduct(verticalVelocity.normalized, up));
            _animator.SetFloat(HorizontalSpeed, horizontalVelocity.magnitude);

            //If animator is strafing, split up horizontal velocity;
            if (useStrafeAnimations) {
                var localVelocity = _animatorTransform.InverseTransformVector(horizontalVelocity);
                _animator.SetFloat(ForwardSpeed, localVelocity.z);
                _animator.SetFloat(StrafeSpeed, localVelocity.x);
            }

            //Pass values to animator;
            _animator.SetBool(IsGrounded, _controller.IsGrounded());
            _animator.SetBool(IsStrafing, useStrafeAnimations);
        }

        private void OnLand(Vector3 v) {
            //Only trigger animation if downward velocity exceeds threshold;
            if (VectorMath.GetDotProduct(v, _transform.up) > -landVelocityThreshold) return;
            _animator.SetTrigger(Land);
        }

        private void OnJump(Vector3 v) {
        }
    }
}