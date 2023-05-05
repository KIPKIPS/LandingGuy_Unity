using UnityEngine;

namespace CMF {
    //A very simplified controller script;
    //This script is an example of a very simple walker controller that covers only the basics of character movement;
    public class SimpleWalkerController : Controller {
        private Mover _mover;
        private float _currentVerticalSpeed;
        private bool _isGrounded;
        public float movementSpeed = 7f;
        public float jumpSpeed = 10f;
        public float gravity = 10f;

        private Vector3 _lastVelocity = Vector3.zero;

        public Transform cameraTransform;
        private CharacterInput _characterInput;
        private Transform _tr;

        // Use this for initialization
        private void Start() {
            _tr = transform;
            _mover = GetComponent<Mover>();
            _characterInput = GetComponent<CharacterInput>();
        }

        void FixedUpdate() {
            //Run initial mover ground check;
            _mover.CheckForGround();

            //If character was not grounded int the last frame and is now grounded, call 'OnGroundContactRegained' function;
            if (_isGrounded == false && _mover.IsGrounded()) OnGroundContactRegained(_lastVelocity);

            //Check whether the character is grounded and store result;
            _isGrounded = _mover.IsGrounded();
            var velocity = Vector3.zero;

            //Add player movement to velocity;
            velocity += CalculateMovementDirection() * movementSpeed;

            //Handle gravity;
            if (!_isGrounded) {
                _currentVerticalSpeed -= gravity * Time.deltaTime;
            } else {
                if (_currentVerticalSpeed <= 0f) _currentVerticalSpeed = 0f;
            }

            //Handle jumping;
            if ((_characterInput != null) && _isGrounded && _characterInput.IsJumpKeyPressed()) {
                OnJumpStart();
                _currentVerticalSpeed = jumpSpeed;
                _isGrounded = false;
            }

            //Add vertical velocity;
            velocity += _tr.up * _currentVerticalSpeed;

            //Save current velocity for next frame;
            _lastVelocity = velocity;
            _mover.SetExtendSensorRange(_isGrounded);
            _mover.SetVelocity(velocity);
        }

        private Vector3 CalculateMovementDirection() {
            //If no character input script is attached to this object, return no input;
            if (_characterInput == null) return Vector3.zero;
            var direction = Vector3.zero;

            //If no camera transform has been assigned, use the character's transform axes to calculate the movement direction;
            if (cameraTransform == null) {
                direction += _tr.right * _characterInput.GetHorizontalMovementInput();
                direction += _tr.forward * _characterInput.GetVerticalMovementInput();
            } else {
                //If a camera transform has been assigned, use the assigned transform's axes for movement direction;
                //Project movement direction so movement stays parallel to the ground;
                var up = _tr.up;
                direction += Vector3.ProjectOnPlane(cameraTransform.right, up).normalized * _characterInput.GetHorizontalMovementInput();
                direction += Vector3.ProjectOnPlane(cameraTransform.forward, up).normalized * _characterInput.GetVerticalMovementInput();
            }

            //If necessary, clamp movement vector to magnitude of 1f;
            if (direction.magnitude > 1f) direction.Normalize();
            return direction;
        }

        //This function is called when the controller has landed on a surface after being in the air;
        private void OnGroundContactRegained(Vector3 collisionVelocity) {
            //Call 'OnLand' delegate function;
            OnLand?.Invoke(collisionVelocity);
        }

        //This function is called when the controller has started a jump;
        private void OnJumpStart() {
            //Call 'OnJump' delegate function;
            OnJump?.Invoke(_lastVelocity);
        }

        //Return the current velocity of the character;
        public override Vector3 GetVelocity() {
            return _lastVelocity;
        }

        //Return only the current movement velocity (without any vertical velocity);
        public override Vector3 GetMovementVelocity() {
            return _lastVelocity;
        }

        //Return whether the character is currently grounded;
        public override bool IsGrounded() {
            return _isGrounded;
        }
    }
}