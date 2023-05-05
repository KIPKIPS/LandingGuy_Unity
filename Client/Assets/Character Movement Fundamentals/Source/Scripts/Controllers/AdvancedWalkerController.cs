using UnityEngine;

namespace CMF {
    //Advanced walker controller script;
    //This controller is used as a basis for other controller types ('SidescrollerController');
    //Custom movement input can be implemented by creating a new script that inherits 'AdvancedWalkerController' and overriding the 'CalculateMovementDirection' function;
    public class AdvancedWalkerController : Controller {
        //References to attached components;
        protected Transform tr;
        private Mover _mover;
        protected CharacterInput characterInput;
        private CeilingDetector _ceilingDetector;

        //Jump key variables;
        private bool _jumpInputIsLocked;
        private bool _jumpKeyWasPressed;
        private bool _jumpKeyWasLetGo;
        private bool _jumpKeyIsPressed;

        //Movement speed;
        public float movementSpeed = 7f;

        //How fast the controller can change direction while in the air;
        //Higher values result in more air control;
        public float airControlRate = 2f;

        //Jump speed;
        public float jumpSpeed = 10f;

        //Jump duration variables;
        public float jumpDuration = 0.2f;
        private float _currentJumpStartTime;

        //'AirFriction' determines how fast the controller loses its momentum while in the air;
        //'GroundFriction' is used instead, if the controller is grounded;
        public float airFriction = 0.5f;
        public float groundFriction = 100f;

        //Current momentum;
        private Vector3 _momentum = Vector3.zero;

        //Saved velocity from last frame;
        private Vector3 _savedVelocity = Vector3.zero;

        //Saved horizontal movement velocity from last frame;
        private Vector3 _savedMovementVelocity = Vector3.zero;

        //Amount of downward gravity;
        public float gravity = 30f;
        [Tooltip("How fast the character will slide down steep slopes.")]
        public float slideGravity = 5f;

        //Acceptable slope angle limit;
        public float slopeLimit = 80f;

        [Tooltip("Whether to calculate and apply momentum relative to the controller's transform.")]
        public bool useLocalMomentum;

        //Enum describing basic controller states; 
        private enum ControllerState {
            Grounded,
            Sliding,
            Falling,
            Rising,
            Jumping
        }

        private ControllerState _currentControllerState = ControllerState.Falling;

        [Tooltip("Optional camera transform used for calculating movement direction. If assigned, character movement will take camera view into account.")]
        public Transform cameraTransform;

        //Get references to all necessary components;
        private void Awake() {
            _mover = GetComponent<Mover>();
            tr = transform;
            characterInput = GetComponent<CharacterInput>();
            _ceilingDetector = GetComponent<CeilingDetector>();
            if (characterInput == null) Debug.LogWarning("No character input script has been attached to this gameObject", this.gameObject);
            Setup();
        }

        //This function is called right after Awake(); It can be overridden by inheriting scripts;
        protected virtual void Setup() {
        }

        private void Update() {
            HandleJumpKeyInput();
        }

        //Handle jump booleans for later use in FixedUpdate;
        private void HandleJumpKeyInput() {
            var newJumpKeyPressedState = IsJumpKeyPressed();
            if (_jumpKeyIsPressed == false && newJumpKeyPressedState) {
                _jumpKeyWasPressed = true;
            }
            if (_jumpKeyIsPressed && newJumpKeyPressedState == false) {
                _jumpKeyWasLetGo = true;
                _jumpInputIsLocked = false;
            }
            _jumpKeyIsPressed = newJumpKeyPressedState;
        }

        private void FixedUpdate() {
            ControllerUpdate();
        }

        //Update controller;
        //This function must be called every fixed update, in order for the controller to work correctly;
        private void ControllerUpdate() {
            //Check if mover is grounded;
            _mover.CheckForGround();

            //Determine controller state;
            _currentControllerState = DetermineControllerState();

            //Apply friction and gravity to 'momentum';
            HandleMomentum();

            //Check if the player has initiated a jump;
            HandleJumping();

            //Calculate movement velocity;
            var velocity = Vector3.zero;
            if (_currentControllerState == ControllerState.Grounded) velocity = CalculateMovementVelocity();

            //If local momentum is used, transform momentum into world space first;
            var worldMomentum = _momentum;
            if (useLocalMomentum) worldMomentum = tr.localToWorldMatrix * _momentum;

            //Add current momentum to velocity;
            velocity += worldMomentum;

            //If player is grounded or sliding on a slope, extend mover's sensor range;
            //This enables the player to walk up/down stairs and slopes without losing ground contact;
            _mover.SetExtendSensorRange(IsGrounded());

            //Set mover velocity;		
            _mover.SetVelocity(velocity);

            //Store velocity for next frame;
            _savedVelocity = velocity;

            //Save controller movement velocity;
            _savedMovementVelocity = CalculateMovementVelocity();

            //Reset jump key booleans;
            _jumpKeyWasLetGo = false;
            _jumpKeyWasPressed = false;

            //Reset ceiling detector, if one is attached to this gameobject;
            if (_ceilingDetector != null) _ceilingDetector.ResetFlags();
        }

        //Calculate and return movement direction based on player input;
        //This function can be overridden by inheriting scripts to implement different player controls;
        protected virtual Vector3 CalculateMovementDirection() {
            //If no character input script is attached to this object, return;
            if (characterInput == null) return Vector3.zero;
            var velocity = Vector3.zero;

            //If no camera transform has been assigned, use the character's transform axes to calculate the movement direction;
            if (cameraTransform == null) {
                velocity += tr.right * characterInput.GetHorizontalMovementInput();
                velocity += tr.forward * characterInput.GetVerticalMovementInput();
            } else {
                //If a camera transform has been assigned, use the assigned transform's axes for movement direction;
                //Project movement direction so movement stays parallel to the ground;
                var up = tr.up;
                velocity += Vector3.ProjectOnPlane(cameraTransform.right, up).normalized * characterInput.GetHorizontalMovementInput();
                velocity += Vector3.ProjectOnPlane(cameraTransform.forward, up).normalized * characterInput.GetVerticalMovementInput();
            }

            //If necessary, clamp movement vector to magnitude of 1f;
            if (velocity.magnitude > 1f) velocity.Normalize();
            return velocity;
        }

        //Calculate and return movement velocity based on player input, controller state, ground normal [...];
        protected virtual Vector3 CalculateMovementVelocity() {
            //Calculate (normalized) movement direction;
            var velocity = CalculateMovementDirection();

            //Multiply (normalized) velocity with movement speed;
            velocity *= movementSpeed;
            return velocity;
        }

        //Returns 'true' if the player presses the jump key;
        protected virtual bool IsJumpKeyPressed() {
            //If no character input script is attached to this object, return;
            return characterInput != null && characterInput.IsJumpKeyPressed();
        }

        //Determine current controller state based on current momentum and whether the controller is grounded (or not);
        //Handle state transitions;
        ControllerState DetermineControllerState() {
            //Check if vertical momentum is pointing upwards;
            var isRising = IsRisingOrFalling() && (VectorMath.GetDotProduct(GetMomentum(), tr.up) > 0f);
            //Check if controller is sliding;
            var isSliding = _mover.IsGrounded() && IsGroundTooSteep();

            //Grounded;
            if (_currentControllerState == ControllerState.Grounded) {
                if (isRising) {
                    OnGroundContactLost();
                    return ControllerState.Rising;
                }
                if (!_mover.IsGrounded()) {
                    OnGroundContactLost();
                    return ControllerState.Falling;
                }
                if (!isSliding) return ControllerState.Grounded;
                OnGroundContactLost();
                return ControllerState.Sliding;
            }

            //Falling;
            if (_currentControllerState == ControllerState.Falling) {
                if (isRising) {
                    return ControllerState.Rising;
                }
                if (!_mover.IsGrounded() || isSliding) return isSliding ? ControllerState.Sliding : ControllerState.Falling;
                OnGroundContactRegained();
                return ControllerState.Grounded;
            }

            //Sliding;
            if (_currentControllerState == ControllerState.Sliding) {
                if (isRising) {
                    OnGroundContactLost();
                    return ControllerState.Rising;
                }
                if (!_mover.IsGrounded()) {
                    OnGroundContactLost();
                    return ControllerState.Falling;
                }
                if (!_mover.IsGrounded() || isSliding) return ControllerState.Sliding;
                OnGroundContactRegained();
                return ControllerState.Grounded;
            }

            //Rising;
            if (_currentControllerState == ControllerState.Rising) {
                if (!isRising) {
                    if (_mover.IsGrounded() && !isSliding) {
                        OnGroundContactRegained();
                        return ControllerState.Grounded;
                    }
                    if (isSliding) {
                        return ControllerState.Sliding;
                    }
                    if (!_mover.IsGrounded()) {
                        return ControllerState.Falling;
                    }
                }

                //If a ceiling detector has been attached to this gameobject, check for ceiling hits;
                if (_ceilingDetector == null) return ControllerState.Rising;
                if (!_ceilingDetector.HitCeiling()) return ControllerState.Rising;
                OnCeilingContact();
                return ControllerState.Falling;
            }

            //Jumping;
            if (_currentControllerState != ControllerState.Jumping) return ControllerState.Falling;
            //Check for jump timeout;
            if ((Time.time - _currentJumpStartTime) > jumpDuration) return ControllerState.Rising;

            //Check if jump key was let go;
            if (_jumpKeyWasLetGo) return ControllerState.Rising;

            //If a ceiling detector has been attached to this gameobject, check for ceiling hits;
            if (_ceilingDetector == null) return ControllerState.Jumping;
            if (!_ceilingDetector.HitCeiling()) return ControllerState.Jumping;
            OnCeilingContact();
            return ControllerState.Falling;
        }

        //Check if player has initiated a jump;
        private void HandleJumping() {
            if (_currentControllerState != ControllerState.Grounded) return;
            if ((!_jumpKeyIsPressed && !_jumpKeyWasPressed) || _jumpInputIsLocked) return;
            //Call events;
            OnGroundContactLost();
            OnJumpStart();
            _currentControllerState = ControllerState.Jumping;
        }

        //Apply friction to both vertical and horizontal momentum based on 'friction' and 'gravity';
        //Handle movement in the air;
        //Handle sliding down steep slopes;
        void HandleMomentum() {
            //If local momentum is used, transform momentum into world coordinates first;
            if (useLocalMomentum) _momentum = tr.localToWorldMatrix * _momentum;
            var verticalMomentum = Vector3.zero;
            var horizontalMomentum = Vector3.zero;

            //Split momentum into vertical and horizontal components;
            if (_momentum != Vector3.zero) {
                verticalMomentum = VectorMath.ExtractDotVector(_momentum, tr.up);
                horizontalMomentum = _momentum - verticalMomentum;
            }

            //Add gravity to vertical momentum;
            verticalMomentum -= tr.up * (gravity * Time.deltaTime);

            //Remove any downward force if the controller is grounded;
            if (_currentControllerState == ControllerState.Grounded && VectorMath.GetDotProduct(verticalMomentum, tr.up) < 0f) verticalMomentum = Vector3.zero;

            //Manipulate momentum to steer controller in the air (if controller is not grounded or sliding);
            if (!IsGrounded()) {
                Vector3 movementVelocity = CalculateMovementVelocity();

                //If controller has received additional momentum from somewhere else;
                if (horizontalMomentum.magnitude > movementSpeed) {
                    //Prevent unwanted accumulation of speed in the direction of the current momentum;
                    if (VectorMath.GetDotProduct(movementVelocity, horizontalMomentum.normalized) > 0f) movementVelocity = VectorMath.RemoveDotVector(movementVelocity, horizontalMomentum.normalized);

                    //Lower air control slightly with a multiplier to add some 'weight' to any momentum applied to the controller;
                    const float airControlMultiplier = 0.25f;
                    horizontalMomentum += movementVelocity * (Time.deltaTime * airControlRate * airControlMultiplier);
                }
                //If controller has not received additional momentum;
                else {
                    //Clamp _horizontal velocity to prevent accumulation of speed;
                    horizontalMomentum += movementVelocity * (Time.deltaTime * airControlRate);
                    horizontalMomentum = Vector3.ClampMagnitude(horizontalMomentum, movementSpeed);
                }
            }

            //Steer controller on slopes;
            if (_currentControllerState == ControllerState.Sliding) {
                //Calculate vector pointing away from slope;
                var pointDownVector = Vector3.ProjectOnPlane(_mover.GetGroundNormal(), tr.up).normalized;

                //Calculate movement velocity;
                var slopeMovementVelocity = CalculateMovementVelocity();
                //Remove all velocity that is pointing up the slope;
                slopeMovementVelocity = VectorMath.RemoveDotVector(slopeMovementVelocity, pointDownVector);

                //Add movement velocity to momentum;
                horizontalMomentum += slopeMovementVelocity * Time.fixedDeltaTime;
            }

            //Apply friction to horizontal momentum based on whether the controller is grounded;
            horizontalMomentum = VectorMath.IncrementVectorTowardTargetVector(horizontalMomentum, _currentControllerState == ControllerState.Grounded ? groundFriction : airFriction, Time.deltaTime, Vector3.zero);

            //Add horizontal and vertical momentum back together;
            _momentum = horizontalMomentum + verticalMomentum;

            //Additional momentum calculations for sliding;
            if (_currentControllerState == ControllerState.Sliding) {
                //Project the current momentum onto the current ground normal if the controller is sliding down a slope;
                _momentum = Vector3.ProjectOnPlane(_momentum, _mover.GetGroundNormal());

                //Remove any upwards momentum when sliding;
                if (VectorMath.GetDotProduct(_momentum, tr.up) > 0f) _momentum = VectorMath.RemoveDotVector(_momentum, tr.up);

                //Apply additional slide gravity;
                var slideDirection = Vector3.ProjectOnPlane(-tr.up, _mover.GetGroundNormal()).normalized;
                _momentum += slideDirection * (slideGravity * Time.deltaTime);
            }

            //If controller is jumping, override vertical velocity with jumpSpeed;
            if (_currentControllerState == ControllerState.Jumping) {
                var up = tr.up;
                _momentum = VectorMath.RemoveDotVector(_momentum, up);
                _momentum += up * jumpSpeed;
            }
            if (useLocalMomentum) _momentum = tr.worldToLocalMatrix * _momentum;
        }

        //Events;

        //This function is called when the player has initiated a jump;
        void OnJumpStart() {
            //If local momentum is used, transform momentum into world coordinates first;
            if (useLocalMomentum) _momentum = tr.localToWorldMatrix * _momentum;

            //Add jump force to momentum;
            _momentum += tr.up * jumpSpeed;

            //Set jump start time;
            _currentJumpStartTime = Time.time;

            //Lock jump input until jump key is released again;
            _jumpInputIsLocked = true;

            //Call event;
            OnJump?.Invoke(_momentum);
            if (useLocalMomentum) _momentum = tr.worldToLocalMatrix * _momentum;
        }

        //This function is called when the controller has lost ground contact, i.e. is either falling or rising, or generally in the air;
        private void OnGroundContactLost() {
            //If local momentum is used, transform momentum into world coordinates first;
            if (useLocalMomentum) _momentum = tr.localToWorldMatrix * _momentum;

            //Get current movement velocity;
            var velocity = GetMovementVelocity();

            //Check if the controller has both momentum and a current movement velocity;
            if (velocity.sqrMagnitude >= 0f && _momentum.sqrMagnitude > 0f) {
                //Project momentum onto movement direction;
                var projectedMomentum = Vector3.Project(_momentum, velocity.normalized);
                //Calculate dot product to determine whether momentum and movement are aligned;
                var dot = VectorMath.GetDotProduct(projectedMomentum.normalized, velocity.normalized);

                //If current momentum is already pointing in the same direction as movement velocity,
                //Don't add further momentum (or limit movement velocity) to prevent unwanted speed accumulation;
                if (projectedMomentum.sqrMagnitude >= velocity.sqrMagnitude && dot > 0f) {
                    velocity = Vector3.zero;
                }
                else if (dot > 0f) {
                    velocity -= projectedMomentum;
                }
            }

            //Add movement velocity to momentum;
            _momentum += velocity;
            if (useLocalMomentum) _momentum = tr.worldToLocalMatrix * _momentum;
        }

        //This function is called when the controller has landed on a surface after being in the air;
        void OnGroundContactRegained() {
            //Call 'OnLand' event;
            if (OnLand == null) return;
            var collisionVelocity = _momentum;
            //If local momentum is used, transform momentum into world coordinates first;
            if (useLocalMomentum) collisionVelocity = tr.localToWorldMatrix * collisionVelocity;
            OnLand(collisionVelocity);
        }

        //This function is called when the controller has collided with a ceiling while jumping or moving upwards;
        private void OnCeilingContact() {
            //If local momentum is used, transform momentum into world coordinates first;
            if (useLocalMomentum) _momentum = tr.localToWorldMatrix * _momentum;

            //Remove all vertical parts of momentum;
            _momentum = VectorMath.RemoveDotVector(_momentum, tr.up);
            if (useLocalMomentum) _momentum = tr.worldToLocalMatrix * _momentum;
        }

        //Helper functions;

        //Returns 'true' if vertical momentum is above a small threshold;
        private bool IsRisingOrFalling() {
            //Calculate current vertical momentum;
            var verticalMomentum = VectorMath.ExtractDotVector(GetMomentum(), tr.up);

            //Setup threshold to check against;
            //For most applications, a value of '0.001f' is recommended;
            const float limit = 0.001f;

            //Return true if vertical momentum is above '_limit';
            return verticalMomentum.magnitude > limit;
        }

        //Returns true if angle between controller and ground normal is too big (> slope limit), i.e. ground is too steep;
        private bool IsGroundTooSteep() {
            if (!_mover.IsGrounded()) return true;
            return Vector3.Angle(_mover.GetGroundNormal(), tr.up) > slopeLimit;
        }

        //Getters;

        //Get last frame's velocity;
        public override Vector3 GetVelocity() {
            return _savedVelocity;
        }

        //Get last frame's movement velocity (momentum is ignored);
        public override Vector3 GetMovementVelocity() {
            return _savedMovementVelocity;
        }

        //Get current momentum;
        private Vector3 GetMomentum() {
            var worldMomentum = _momentum;
            if (useLocalMomentum) worldMomentum = tr.localToWorldMatrix * _momentum;
            return worldMomentum;
        }

        //Returns 'true' if controller is grounded (or sliding down a slope);
        public override bool IsGrounded() {
            return _currentControllerState == ControllerState.Grounded || _currentControllerState == ControllerState.Sliding;
        }

        //Returns 'true' if controller is sliding;
        public bool IsSliding() {
            return _currentControllerState == ControllerState.Sliding;
        }

        //Add momentum to controller;
        public void AddMomentum(Vector3 momentum) {
            if (useLocalMomentum) _momentum = tr.localToWorldMatrix * _momentum;
            _momentum += momentum;
            if (useLocalMomentum) _momentum = tr.worldToLocalMatrix * _momentum;
        }

        //Set controller momentum directly;
        public void SetMomentum(Vector3 newMomentum) {
            if (useLocalMomentum)
                _momentum = tr.worldToLocalMatrix * newMomentum;
            else
                _momentum = newMomentum;
        }
    }
}