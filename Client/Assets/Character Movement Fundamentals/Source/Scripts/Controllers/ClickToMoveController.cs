using UnityEngine;
using CMF;

//This controller provides basic 'click-to-move' functionality;
//It can be used as a starting point for a variety of top-down (or isometric) games, which are primarily controlled via mouse input;
public class ClickToMoveController : Controller {
    //Controller movement speed;
    public float movementSpeed = 10f;
    //Downward gravity;
    public float gravity = 30f;

    private float _currentVerticalSpeed;
    private bool _isGrounded;

    //Current position to move towards;
    private Vector3 _currentTargetPosition;
    //If the distance between controller and target position is smaller than this, the target is reached;
    private const float ReachTargetThreshold = 0.001f;

    //Whether the user can hold down the mouse button to continually move the controller;
    public bool holdMouseButtonToMove;

    //Whether the target position is determined by raycasting against an abstract plane or the actual level geometry;
    //'AbstractPlane' is less accurate, but simpler (and will automatically ignore colliders between the camera and target position);
    //'Raycast' is more accurate, but ceilings or intersecting geometry (between camera and target position) must be handled separately;
    public enum MouseDetectionType {
        AbstractPlane,
        Raycast
    }

    public MouseDetectionType mouseDetectionType = MouseDetectionType.AbstractPlane;

    //Layermask used when 'Raycast' is selected;
    public LayerMask raycastLayerMask = ~0;

    //Timeout variables;
    //If the controller is stuck walking against a wall, movement will be canceled if it hasn't moved at least a certain distance in a certain time;
    //'timeOutTime' controls the time window during which the controller has to move (or else it stops moving);
    public float timeOutTime = 1f;
    private float _currentTimeOutTime = 1f;
    //This controls the minimum amount of distance needed to be moved (or else the controller stops moving);
    public float timeOutDistanceThreshold = 0.05f;
    private Vector3 _lastPosition;

    //Reference to the player's camera (used for raycasting);
    public Camera playerCamera;

    //Whether or not the controller currently has a valid target position to move towards;
    private bool _hasTarget;

    private Vector3 _lastVelocity = Vector3.zero;
    private Vector3 _lastMovementVelocity = Vector3.zero;

    //Abstarct ground plane used when 'AbstractPlane' is selected;
    private Plane _groundPlane;

    //Reference to attached 'Mover' and transform component;
    private Mover _mover;
    private Transform _transform;

    private void Start() {
        //Get references to necessary components;
        _mover = GetComponent<Mover>();
        _transform = transform;
        if (playerCamera == null) Debug.LogWarning("No camera has been assigned to this controller!", this);

        //Initialize variables;
        var position = _transform.position;
        _lastPosition = position;
        _currentTargetPosition = transform.position;
        _groundPlane = new Plane(_transform.up, position);
    }

    private void Update() {
        //Handle mouse input (check for input, determine new target position);
        HandleMouseInput();
    }

    private void FixedUpdate() {
        //Run initial mover ground check;
        _mover.CheckForGround();

        //Check whether the character is grounded;
        _isGrounded = _mover.IsGrounded();

        //Handle timeout (stop controller if it is stuck);
        HandleTimeOut();
        //Calculate the final velocity for this frame;
        var velocity = CalculateMovementVelocity();
        _lastMovementVelocity = velocity;

        //Calculate and apply gravity;
        HandleGravity();
        velocity += _transform.up * _currentVerticalSpeed;

        //If the character is grounded, extend ground detection sensor range;
        _mover.SetExtendSensorRange(_isGrounded);
        //Set mover velocity;
        _mover.SetVelocity(velocity);

        //Save velocity for later;
        _lastVelocity = velocity;
    }

    //Calculate movement velocity based on the current target position;
    private Vector3 CalculateMovementVelocity() {
        //Return no velocity if controller currently has no target;	
        if (!_hasTarget) return Vector3.zero;

        //Calculate vector to target position;
        var toTarget = _currentTargetPosition - _transform.position;

        //Remove all vertical parts of vector;
        toTarget = VectorMath.RemoveDotVector(toTarget, _transform.up);

        //Calculate distance to target;
        var distanceToTarget = toTarget.magnitude;

        //If controller has already reached target position, return no velocity;
        if (distanceToTarget <= ReachTargetThreshold) {
            _hasTarget = false;
            return Vector3.zero;
        }
        var velocity = toTarget.normalized * movementSpeed;

        //Check for overshooting;
        if (!(movementSpeed * Time.fixedDeltaTime > distanceToTarget)) return velocity;
        velocity = toTarget.normalized * distanceToTarget;
        _hasTarget = false;
        return velocity;
    }

    //Calculate current gravity;
    private void HandleGravity() {
        //Handle gravity;
        if (!_isGrounded)
            _currentVerticalSpeed -= gravity * Time.deltaTime;
        else {
            if (_currentVerticalSpeed < 0f) {
                OnLand?.Invoke(_transform.up * _currentVerticalSpeed);
            }
            _currentVerticalSpeed = 0f;
        }
    }

    //Handle mouse input (mouse clicks, [...]);
    private void HandleMouseInput() {
        //If no camera has been assigned, stop function execution;
        if (playerCamera == null) return;

        //If a valid mouse press has been detected, raycast to determine the new target position;
        if ((holdMouseButtonToMove || !WasMouseButtonJustPressed()) && (!holdMouseButtonToMove || !IsMouseButtonPressed())) return;
        //Set up mouse ray (based on screen position);
        var mouseRay = playerCamera.ScreenPointToRay(GetMousePosition());
        if (mouseDetectionType == MouseDetectionType.AbstractPlane) {
            //Set up abstract ground plane;
            _groundPlane.SetNormalAndPosition(_transform.up, _transform.position);

            //Raycast against ground plane;
            if (_groundPlane.Raycast(mouseRay, out var enter)) {
                _currentTargetPosition = mouseRay.GetPoint(enter);
                _hasTarget = true;
            } else
                _hasTarget = false;
        } else if (mouseDetectionType == MouseDetectionType.Raycast) {
            //Raycast against level geometry;
            if (Physics.Raycast(mouseRay, out var hit, 100f, raycastLayerMask, QueryTriggerInteraction.Ignore)) {
                _currentTargetPosition = hit.point;
                _hasTarget = true;
            } else
                _hasTarget = false;
        }
    }

    //Handle timeout (stop controller from moving if it is stuck against level geometry);
    private void HandleTimeOut() {
        //If controller currently has no target, reset time and return;
        if (!_hasTarget) {
            _currentTimeOutTime = 0f;
            return;
        }

        //If controller has moved enough distance, reset time;
        if (Vector3.Distance(_transform.position, _lastPosition) > timeOutDistanceThreshold) {
            _currentTimeOutTime = 0f;
            _lastPosition = _transform.position;
        }
        //If controller hasn't moved a sufficient distance, increment current timeout time;
        else {
            _currentTimeOutTime += Time.deltaTime;

            //If current timeout time has reached limit, stop controller from moving;
            if (_currentTimeOutTime >= timeOutTime) {
                _hasTarget = false;
            }
        }
    }

    //Get current screen position of mouse cursor;
    //This function can be overridden to implement other input methods;
    private Vector2 GetMousePosition() {
        return Input.mousePosition;
    }

    //Check whether mouse button is currently pressed down;
    //This function can be overridden to implement other input methods;
    private bool IsMouseButtonPressed() {
        return Input.GetMouseButton(0);
    }

    //Check whether mouse button was just pressed down;
    //This function can be overridden to implement other input methods;
    private bool WasMouseButtonJustPressed() {
        return Input.GetMouseButtonDown(0);
    }

    public override bool IsGrounded() {
        return _isGrounded;
    }

    public override Vector3 GetMovementVelocity() {
        return _lastMovementVelocity;
    }

    public override Vector3 GetVelocity() {
        return _lastVelocity;
    }
}