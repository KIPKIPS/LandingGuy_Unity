using System;
using UnityEngine;

namespace CMF {
    //This script handles all physics, collision detection and ground detection;
    //It expects a movement velocity (via 'SetVelocity') every 'FixedUpdate' frame from an external script (like a controller script) to work;
    //It also provides several getter methods for important information (whether the mover is grounded, the current surface normal [...]);
    public class Mover : MonoBehaviour {
        //Collider variables;
        [Header("Mover Options :")]
        [Range(0f, 1f)] [SerializeField]
        private float stepHeightRatio = 0.25f;
        [Header("Collider Options :")]
        [SerializeField] private float colliderHeight = 2f;
        [SerializeField] private float colliderThickness = 1f;
        [SerializeField] private Vector3 colliderOffset = Vector3.zero;

        //References to attached collider(s);
        private BoxCollider _boxCollider;
        private SphereCollider _sphereCollider;
        private CapsuleCollider _capsuleCollider;

        //Sensor variables;
        [Header("Sensor Options :")]
        [SerializeField] public Sensor.CastType sensorType = Sensor.CastType.Raycast;
        private const float SensorRadiusModifier = 0.8f;
        private int _currentLayer;
        [SerializeField] private bool isInDebugMode;
        [Header("Sensor Array Options :")]
        [SerializeField] [Range(1, 5)]
        private int sensorArrayRows = 1;
        [SerializeField] [Range(3, 10)] private int sensorArrayRayCount = 6;
        [SerializeField] private bool sensorArrayRowsAreOffset;

        [HideInInspector] public Vector3[] raycastArrayPreviewPositions;

        //Ground detection variables;
        private bool _isGrounded;

        //Sensor range variables;
        private bool _isUsingExtendedSensorRange = true;
        private float _baseSensorRange;

        //Current upwards (or downwards) velocity necessary to keep the correct distance to the ground;
        private Vector3 _currentGroundAdjustmentVelocity = Vector3.zero;

        //References to attached components;
        private Collider _col;
        private Rigidbody _rig;
        private Transform _transform;
        private Sensor _sensor;

        private void Awake() {
            Setup();

            //Initialize sensor;
            _sensor = new Sensor(_transform, _col);
            RecalculateColliderDimensions();
            RecalibrateSensor();
        }

        private void Reset() {
            Setup();
        }

        private void OnValidate() {
            //Recalculate collider dimensions;
            if (gameObject.activeInHierarchy) RecalculateColliderDimensions();

            //Recalculate raycast array preview positions;
            if (sensorType == Sensor.CastType.RaycastArray) raycastArrayPreviewPositions = Sensor.GetRaycastStartPositions(sensorArrayRows, sensorArrayRayCount, sensorArrayRowsAreOffset, 1f);
        }

        //Setup references to components;
        private void Setup() {
            _transform = transform;
            _col = GetComponent<Collider>();

            //If no collider is attached to this gameobject, add a collider;
            if (_col == null) {
                _transform.gameObject.AddComponent<CapsuleCollider>();
                _col = GetComponent<Collider>();
            }
            _rig = GetComponent<Rigidbody>();

            //If no rigidbody is attached to this gameobject, add a rigidbody;
            if (_rig == null) {
                _transform.gameObject.AddComponent<Rigidbody>();
                _rig = GetComponent<Rigidbody>();
            }
            _boxCollider = GetComponent<BoxCollider>();
            _sphereCollider = GetComponent<SphereCollider>();
            _capsuleCollider = GetComponent<CapsuleCollider>();

            //Freeze rigidbody rotation and disable rigidbody gravity;
            _rig.freezeRotation = true;
            _rig.useGravity = false;
        }

        //Draw debug information if debug mode is enabled;
        private void LateUpdate() {
            if (isInDebugMode) _sensor.DrawDebug();
        }

        //Recalculate collider height/width/thickness;
        private void RecalculateColliderDimensions() {
            //Check if a collider is attached to this gameobject;
            if (_col == null) {
                //Try to get a reference to the attached collider by calling Setup();
                Setup();

                //Check again;
                if (_col == null) {
                    Debug.LogWarning("There is no collider attached to " + this.gameObject.name + "!");
                    return;
                }
            }

            //Set collider dimensions based on collider variables;
            if (_boxCollider) {
                var size = Vector3.zero;
                size.x = colliderThickness;
                size.z = colliderThickness;
                _boxCollider.center = colliderOffset * colliderHeight;
                size.y = colliderHeight * (1f - stepHeightRatio);
                _boxCollider.size = size;
                _boxCollider.center += new Vector3(0f, stepHeightRatio * colliderHeight / 2f, 0f);
            } else if (_sphereCollider) {
                _sphereCollider.radius = colliderHeight / 2f;
                _sphereCollider.center = colliderOffset * colliderHeight;
                _sphereCollider.center += new Vector3(0f, stepHeightRatio * _sphereCollider.radius, 0f);
                _sphereCollider.radius *= (1f - stepHeightRatio);
            } else if (_capsuleCollider) {
                _capsuleCollider.height = colliderHeight;
                _capsuleCollider.center = colliderOffset * colliderHeight;
                _capsuleCollider.radius = colliderThickness / 2f;
                _capsuleCollider.center += new Vector3(0f, stepHeightRatio * _capsuleCollider.height / 2f, 0f);
                _capsuleCollider.height *= (1f - stepHeightRatio);
                if (_capsuleCollider.height / 2f < _capsuleCollider.radius) _capsuleCollider.radius = _capsuleCollider.height / 2f;
            }

            //Recalibrate sensor variables to fit new collider dimensions;
            if (_sensor != null) RecalibrateSensor();
        }

        //Recalibrate sensor variables;
        private void RecalibrateSensor() {
            //Set sensor ray origin and direction;
            _sensor.SetCastOrigin(GetColliderCenter());
            _sensor.SetCastDirection(Sensor.CastDirection.Down);

            //Calculate sensor layermask;
            RecalculateSensorLayerMask();

            //Set sensor cast type;
            _sensor.castType = sensorType;

            //Calculate sensor radius/width;
            var radius = colliderThickness / 2f * SensorRadiusModifier;

            //Multiply all sensor lengths with '_safetyDistanceFactor' to compensate for floating point errors;
            const float safetyDistanceFactor = 0.001f;

            //Fit collider height to sensor radius;
            if (_boxCollider)
                radius = Mathf.Clamp(radius, safetyDistanceFactor, (_boxCollider.size.y / 2f) * (1f - safetyDistanceFactor));
            else if (_sphereCollider)
                radius = Mathf.Clamp(radius, safetyDistanceFactor, _sphereCollider.radius * (1f - safetyDistanceFactor));
            else if (_capsuleCollider) radius = Mathf.Clamp(radius, safetyDistanceFactor, (_capsuleCollider.height / 2f) * (1f - safetyDistanceFactor));

            //Set sensor variables;

            //Set sensor radius;
            var localScale = _transform.localScale;
            _sensor.sphereCastRadius = radius * localScale.x;

            //Calculate and set sensor length;
            var length = 0f;
            length += (colliderHeight * (1f - stepHeightRatio)) * 0.5f;
            length += colliderHeight * stepHeightRatio;
            _baseSensorRange = length * (1f + safetyDistanceFactor) * localScale.x;
            _sensor.castLength = length * localScale.x;

            //Set sensor array variables;
            _sensor.arrayRows = sensorArrayRows;
            _sensor.arrayRayCount = sensorArrayRayCount;
            _sensor.offsetArrayRows = sensorArrayRowsAreOffset;
            _sensor.isInDebugMode = isInDebugMode;

            //Set sensor spherecast variables;
            _sensor.calculateRealDistance = true;
            _sensor.calculateRealSurfaceNormal = true;

            //Recalibrate sensor to the new values;
            _sensor.RecalibrateRaycastArrayPositions();
        }

        //Recalculate sensor layermask based on current physics settings;
        private void RecalculateSensorLayerMask() {
            var layerMask = 0;
            var objectLayer = gameObject.layer;

            //Calculate layermask;
            for (var i = 0; i < 32; i++) {
                if (!Physics.GetIgnoreLayerCollision(objectLayer, i)) layerMask |= (1 << i);
            }

            //Make sure that the calculated layermask does not include the 'Ignore Raycast' layer;
            if (layerMask == (layerMask | (1 << LayerMask.NameToLayer("Ignore Raycast")))) {
                layerMask ^= (1 << LayerMask.NameToLayer("Ignore Raycast"));
            }

            //Set sensor layermask;
            _sensor.layerMask = layerMask;

            //Save current layer;
            _currentLayer = objectLayer;
        }

        //Returns the collider's center in world coordinates;
        private Vector3 GetColliderCenter() {
            if (_col == null) Setup();
            return _col.bounds.center;
        }

        //Check if mover is grounded;
        //Store all relevant collision information for later;
        //Calculate necessary adjustment velocity to keep the correct distance to the ground;
        private void Check() {
            //Reset ground adjustment velocity;
            _currentGroundAdjustmentVelocity = Vector3.zero;

            //Set sensor length;
            if (_isUsingExtendedSensorRange)
                _sensor.castLength = _baseSensorRange + (colliderHeight * _transform.localScale.x) * stepHeightRatio;
            else
                _sensor.castLength = _baseSensorRange;
            _sensor.Cast();

            //If sensor has not detected anything, set flags and return;
            if (!_sensor.HasDetectedHit()) {
                _isGrounded = false;
                return;
            }

            //Set flags for ground detection;
            _isGrounded = true;

            //Get distance that sensor ray reached;
            var distance = _sensor.GetDistance();

            //Calculate how much mover needs to be moved up or down;
            var localScale = _transform.localScale;
            var upperLimit = ((colliderHeight * localScale.x) * (1f - stepHeightRatio)) * 0.5f;
            var middle = upperLimit + (colliderHeight * localScale.x) * stepHeightRatio;
            var distanceToGo = middle - distance;

            //Set new ground adjustment velocity for the next frame;
            _currentGroundAdjustmentVelocity = _transform.up * (distanceToGo / Time.fixedDeltaTime);
        }

        //Check if mover is grounded;
        public void CheckForGround() {
            //Check if object layer has been changed since last frame;
            //If so, recalculate sensor layer mask;
            if (_currentLayer != this.gameObject.layer) RecalculateSensorLayerMask();
            Check();
        }

        //Set mover velocity;
        public void SetVelocity(Vector3 velocity) {
            _rig.velocity = velocity + _currentGroundAdjustmentVelocity;
        }

        //Returns 'true' if mover is touching ground and the angle between hte 'up' vector and ground normal is not too steep (e.g., angle < slope_limit);
        public bool IsGrounded() {
            return _isGrounded;
        }

        //Setters;

        //Set whether sensor range should be extended;
        public void SetExtendSensorRange(bool isExtended) {
            _isUsingExtendedSensorRange = isExtended;
        }

        //Set height of collider;
        public void SetColliderHeight(float newColliderHeight) {
            if (Math.Abs(colliderHeight - newColliderHeight) < 0.001f) return;
            colliderHeight = newColliderHeight;
            RecalculateColliderDimensions();
        }

        //Set thickness/width of collider;
        public void SetColliderThickness(float newColliderThickness) {
            if (Math.Abs(colliderThickness - newColliderThickness) < 0.001f) return;
            if (newColliderThickness < 0f) newColliderThickness = 0f;
            colliderThickness = newColliderThickness;
            RecalculateColliderDimensions();
        }

        //Set acceptable step height;
        public void SetStepHeightRatio(float newStepHeightRatio) {
            newStepHeightRatio = Mathf.Clamp(newStepHeightRatio, 0f, 1f);
            stepHeightRatio = newStepHeightRatio;
            RecalculateColliderDimensions();
        }

        //Getters;

        public Vector3 GetGroundNormal() {
            return _sensor.GetNormal();
        }

        public Vector3 GetGroundPoint() {
            return _sensor.GetPosition();
        }

        public Collider GetGroundCollider() {
            return _sensor.GetCollider();
        }
    }
}