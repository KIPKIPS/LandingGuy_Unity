using UnityEngine;

namespace CMF {
    //This script rotates a gameobject based on user input.
    //Rotation around the x-axis (vertical) can be clamped/limited by setting 'upperVerticalLimit' and 'lowerVerticalLimit'.
    public class CameraController : MonoBehaviour {
        //Current rotation values (in degrees);
        private float _currentXAngle;
        private float _currentYAngle;

        //Upper and lower limits (in degrees) for vertical rotation (along the local x-axis of the gameobject);
        [Range(0f, 90f)]
        public float upperVerticalLimit = 60f;
        [Range(0f, 90f)]
        public float lowerVerticalLimit = 60f;

        //Variables to store old rotation values for interpolation purposes;
        private float _oldHorizontalInput;
        private float _oldVerticalInput;

        //Camera turning speed; 
        public float cameraSpeed = 250f;

        //Whether camera rotation values will be smoothed;
        public bool smoothCameraRotation;

        //This value controls how smoothly the old camera rotation angles will be interpolated toward the new camera rotation angles;
        //Setting this value to '50f' (or above) will result in no smoothing at all;
        //Setting this value to '1f' (or below) will result in very noticable smoothing;
        //For most situations, a value of '25f' is recommended;
        [Range(1f, 50f)]
        public float cameraSmoothingFactor = 25f;

        //Variables for storing current facing direction and upwards direction;
        private Vector3 _facingDirection;
        private Vector3 _upwardsDirection;

        //References to transform and camera components;
        private Transform _transform;
        private Camera _cam;
        private CameraInput _cameraInput;

        //Setup references.
        private void Awake() {
            _transform = transform;
            _cam = GetComponent<Camera>();
            _cameraInput = GetComponent<CameraInput>();
            if (_cameraInput == null) Debug.LogWarning("No camera input script has been attached to this gameObject", this.gameObject);

            //If no camera component has been attached to this gameobject, search the transform's children;
            if (_cam == null) _cam = GetComponentInChildren<Camera>();

            //Set angle variables to current rotation angles of this transform;
            var localRotation = _transform.localRotation;
            _currentXAngle = localRotation.eulerAngles.x;
            _currentYAngle = localRotation.eulerAngles.y;

            //Execute camera rotation code once to calculate facing and upwards direction;
            RotateCamera(0f, 0f);
            Setup();
        }

        //This function is called right after Awake(); It can be overridden by inheriting scripts;
        protected virtual void Setup() {
        }

        private void Update() {
            HandleCameraRotation();
        }

        //Get user input and handle camera rotation;
        //This method can be overridden in classes derived from this base class to modify camera behaviour;
        protected virtual void HandleCameraRotation() {
            if (_cameraInput == null) return;

            //Get input values;
            var inputHorizontal = _cameraInput.GetHorizontalCameraInput();
            var inputVertical = _cameraInput.GetVerticalCameraInput();
            RotateCamera(inputHorizontal, inputVertical);
        }

        //Rotate camera; 
        private void RotateCamera(float newHorizontalInput, float newVerticalInput) {
            if (smoothCameraRotation) {
                //Lerp input;
                _oldHorizontalInput = Mathf.Lerp(_oldHorizontalInput, newHorizontalInput, Time.deltaTime * cameraSmoothingFactor);
                _oldVerticalInput = Mathf.Lerp(_oldVerticalInput, newVerticalInput, Time.deltaTime * cameraSmoothingFactor);
            } else {
                //Replace old input directly;
                _oldHorizontalInput = newHorizontalInput;
                _oldVerticalInput = newVerticalInput;
            }

            //Add input to camera angles;
            _currentXAngle += _oldVerticalInput * cameraSpeed * Time.deltaTime;
            _currentYAngle += _oldHorizontalInput * cameraSpeed * Time.deltaTime;

            //Clamp vertical rotation;
            _currentXAngle = Mathf.Clamp(_currentXAngle, -upperVerticalLimit, lowerVerticalLimit);
            UpdateRotation();
        }

        //Update camera rotation based on x and y angles;
        private void UpdateRotation() {
            // Quaternion localRotation;
            // localRotation = Quaternion.Euler(new Vector3(0, _currentYAngle, 0));

            //Save 'facingDirection' and 'upwardsDirection' for later;
            _facingDirection = _transform.forward;
            _upwardsDirection = _transform.up;
            // var localRotation = Quaternion.Euler(new Vector3(_currentXAngle, _currentYAngle, 0));
            _transform.localRotation = Quaternion.Euler(new Vector3(_currentXAngle, _currentYAngle, 0));
        }

        //Set the camera's field-of-view (FOV);
        public void SetFOV(float fov) {
            if (_cam) _cam.fieldOfView = fov;
        }

        //Set x and y angle directly;
        protected void SetRotationAngles(float xAngle, float yAngle) {
            _currentXAngle = xAngle;
            _currentYAngle = yAngle;
            UpdateRotation();
        }

        //Rotate the camera toward a rotation that points at a world position in the scene;
        public void RotateTowardPosition(Vector3 position, float lookSpeed) {
            //Calculate target look vector;
            var direction = (position - _transform.position);
            RotateTowardDirection(direction, lookSpeed);
        }

        //Rotate the camera toward a look vector in the scene;
        private void RotateTowardDirection(Vector3 direction, float lookSpeed) {
            //Normalize direction;
            direction.Normalize();

            //Transform target look vector to this transform's local space;
            var parent = _transform.parent;
            direction = parent.InverseTransformDirection(direction);

            //Calculate (local) current look vector; 
            var currentLookVector = GetAimingDirection();
            currentLookVector = parent.InverseTransformDirection(currentLookVector);

            //Calculate x angle difference;
            var xAngleDifference = VectorMath.GetAngle(new Vector3(0f, currentLookVector.y, 1f), new Vector3(0f, direction.y, 1f), Vector3.right);

            //Calculate y angle difference;
            currentLookVector.y = 0f;
            direction.y = 0f;
            var yAngleDifference = VectorMath.GetAngle(currentLookVector, direction, Vector3.up);

            //Turn angle values into Vector2 variables for better clamping;
            var currentAngles = new Vector2(_currentXAngle, _currentYAngle);
            var angleDifference = new Vector2(xAngleDifference, yAngleDifference);

            //Calculate normalized direction;
            var angleDifferenceMagnitude = angleDifference.magnitude;
            if (angleDifferenceMagnitude == 0f) return;
            var angleDifferenceDirection = angleDifference / angleDifferenceMagnitude;

            //Check for overshooting;
            if (lookSpeed * Time.deltaTime > angleDifferenceMagnitude) {
                currentAngles += angleDifferenceDirection * angleDifferenceMagnitude;
            } else
                currentAngles += angleDifferenceDirection * lookSpeed * Time.deltaTime;

            //Set new angles;
            _currentYAngle = currentAngles.y;
            //Clamp vertical rotation;
            _currentXAngle = Mathf.Clamp(currentAngles.x, -upperVerticalLimit, lowerVerticalLimit);
            UpdateRotation();
        }

        protected float GetCurrentXAngle() {
            return _currentXAngle;
        }

        protected float GetCurrentYAngle() {
            return _currentYAngle;
        }

        //Returns the direction the camera is facing, without any vertical rotation;
        //This vector should be used for movement-related purposes (e.g., moving forward);
        public Vector3 GetFacingDirection() {
            return _facingDirection;
        }

        //Returns the 'forward' vector of this gameobject;
        //This vector points in the direction the camera is "aiming" and could be used for instantiating projectiles or raycasts.
        private Vector3 GetAimingDirection() {
            return _transform.forward;
        }

        // Returns the 'right' vector of this gameobject;
        public Vector3 GetStrafeDirection() {
            return _transform.right;
        }

        // Returns the 'up' vector of this gameobject;
        public Vector3 GetUpDirection() {
            return _upwardsDirection;
        }
    }
}