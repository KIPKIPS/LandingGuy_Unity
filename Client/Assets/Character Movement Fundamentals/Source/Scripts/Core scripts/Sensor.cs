using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace CMF {
    //This script is responsible for casting rays and spherecasts;
    //It is instantiated by the 'Mover' component at runtime;
    [System.Serializable]
    public class Sensor {
        //Basic raycast parameters;
        public float castLength = 1f;
        public float sphereCastRadius = 0.2f;

        //Starting point of (ray-)cast;
        private Vector3 _origin = Vector3.zero;

        //Enum describing local transform axes used as directions for raycasting;
        public enum CastDirection {
            Forward,
            Right,
            Up,
            Backward,
            Left,
            Down
        }

        private CastDirection _castDirection;

        //Raycast hit information variables;
        private bool _hasDetectedHit;
        private Vector3 _hitPosition;
        private Vector3 _hitNormal;
        private float _hitDistance;
        private List<Collider> _hitColliders = new();
        private List<Transform> _hitTransforms = new();

        //Backup normal used for specific edge cases when using spherecasts;
        private Vector3 _backupNormal;

        //References to attached components;
        private Transform _tr;
        private Collider _col;

        //Enum describing different types of ground detection methods;
        public enum CastType {
            Raycast,
            RaycastArray,
            Spherecast
        }

        public CastType castType = CastType.Raycast;
        public LayerMask layerMask = 255;

        //Layer number for 'Ignore Raycast' layer;
        private int _ignoreRaycastLayer;

        //Spherecast settings;

        //Cast an additional ray to get the true surface normal;
        public bool calculateRealSurfaceNormal;
        //Cast an additional ray to get the true distance to the ground;
        public bool calculateRealDistance;

        //Array raycast settings;

        //Number of rays in every row;
        public int arrayRayCount = 9;
        //Number of rows around the central ray;
        public int arrayRows = 3;
        //Whether or not to offset every other row;
        public bool offsetArrayRows;

        //Array containing all array raycast start positions (in local coordinates);
        private Vector3[] _raycastArrayStartPositions;

        //Optional list of colliders to ignore when raycasting;
        private Collider[] _ignoreList;

        //Array to store layers of colliders in ignore list;
        private int[] _ignoreListLayers;

        //Whether to draw debug information (hit positions, hit normals...) in the editor;
        public bool isInDebugMode;

        private List<Vector3> _arrayNormals = new();
        private List<Vector3> _arrayPoints = new();

        //Constructor;
        public Sensor(Transform transform, Collider collider) {
            _tr = transform;
            if (collider == null) return;
            _ignoreList = new Collider[1];

            //Add collider to ignore list;
            _ignoreList[0] = collider;

            //Store "Ignore Raycast" layer number for later;
            _ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");

            //Setup array to store ignore list layers;
            _ignoreListLayers = new int[_ignoreList.Length];
        }

        //Reset all variables related to storing information on raycast hits;
        private void ResetFlags() {
            _hasDetectedHit = false;
            _hitPosition = Vector3.zero;
            _hitNormal = -GetCastDirection();
            _hitDistance = 0f;
            if (_hitColliders.Count > 0) _hitColliders.Clear();
            if (_hitTransforms.Count > 0) _hitTransforms.Clear();
        }

        //Returns an array containing the starting positions of all array rays (in local coordinates) based on the input arguments;
        public static Vector3[] GetRaycastStartPositions(int sensorRows, int sensorRayCount, bool offsetRows, float sensorRadius) {
            //Initialize list used to store the positions;
            var positions = new List<Vector3>();

            //Add central start position to the list;
            var startPosition = Vector3.zero;
            positions.Add(startPosition);
            for (var i = 0; i < sensorRows; i++) {
                //Calculate radius for all positions on this row;
                var rowRadius = (float)(i + 1) / sensorRows;
                for (var j = 0; j < sensorRayCount * (i + 1); j++) {
                    //Calculate angle (in degrees) for this individual position;
                    var angle = (360f / (sensorRayCount * (i + 1))) * j;

                    //If 'offsetRows' is set to 'true', every other row is offset;
                    if (offsetRows && i % 2 == 0) angle += (360f / (sensorRayCount * (i + 1))) / 2f;

                    //Combine radius and angle into one position and add it to the list;
                    var x = rowRadius * Mathf.Cos(Mathf.Deg2Rad * angle);
                    var y = rowRadius * Mathf.Sin(Mathf.Deg2Rad * angle);
                    positions.Add(new Vector3(x, 0f, y) * sensorRadius);
                }
            }
            //Convert list to array and return array;
            return positions.ToArray();
        }

        //Cast a ray (or sphere or array of rays) to check for colliders;
        public void Cast() {
            ResetFlags();

            //Calculate origin and direction of ray in world coordinates;
            var worldDirection = GetCastDirection();
            var worldOrigin = _tr.TransformPoint(_origin);

            //Check if ignore list length has been changed since last frame;
            if (_ignoreListLayers.Length != _ignoreList.Length) {
                //If so, setup ignore layer array to fit new length;
                _ignoreListLayers = new int[_ignoreList.Length];
            }

            //(Temporarily) move all objects in ignore list to 'Ignore Raycast' layer;
            for (var i = 0; i < _ignoreList.Length; i++) {
                _ignoreListLayers[i] = _ignoreList[i].gameObject.layer;
                _ignoreList[i].gameObject.layer = _ignoreRaycastLayer;
            }

            //Depending on the chosen mode of detection, call different functions to check for colliders;
            switch (castType) {
                case CastType.Raycast:
                    CastRay(worldOrigin, worldDirection);
                    break;
                case CastType.Spherecast:
                    CastSphere(worldOrigin, worldDirection);
                    break;
                case CastType.RaycastArray:
                    CastRayArray(worldOrigin, worldDirection);
                    break;
                default:
                    _hasDetectedHit = false;
                    break;
            }

            //Reset collider layers in ignoreList;
            for (var i = 0; i < _ignoreList.Length; i++) {
                _ignoreList[i].gameObject.layer = _ignoreListLayers[i];
            }
        }

        //Cast an array of rays into '_direction' and centered around '_origin';
        private void CastRayArray(Vector3 origin, Vector3 direction) {
            //Calculate origin and direction of ray in world coordinates;
            var rayDirection = GetCastDirection();

            //Clear results from last frame;
            _arrayNormals.Clear();
            _arrayPoints.Clear();

            //Cast array;
            foreach (var t in _raycastArrayStartPositions) {
                //Calculate ray start position;
                var rayStartPosition = origin + _tr.TransformDirection(t);
                if (!Physics.Raycast(rayStartPosition, rayDirection, out var hit, castLength, layerMask, QueryTriggerInteraction.Ignore)) continue;
                if (isInDebugMode) Debug.DrawRay(hit.point, hit.normal, Color.red, Time.fixedDeltaTime * 1.01f);
                _hitColliders.Add(hit.collider);
                _hitTransforms.Add(hit.transform);
                _arrayNormals.Add(hit.normal);
                _arrayPoints.Add(hit.point);
            }

            //Evaluate results;
            _hasDetectedHit = (_arrayPoints.Count > 0);
            if (!_hasDetectedHit) return;
            var averageNormal = Vector3.zero;
            foreach (var t in _arrayNormals) {
                averageNormal += t;
            }
            averageNormal.Normalize();

            //Calculate average surface point;
            var averagePoint = Vector3.zero;
            foreach (var t in _arrayPoints) {
                averagePoint += t;
            }
            averagePoint /= _arrayPoints.Count;
            _hitPosition = averagePoint;
            _hitNormal = averageNormal;
            _hitDistance = VectorMath.ExtractDotVector(origin - _hitPosition, direction).magnitude;
        }

        //Cast a single ray into '_direction' from '_origin';
        private void CastRay(Vector3 origin, Vector3 direction) {
            _hasDetectedHit = Physics.Raycast(origin, direction, out var hit, castLength, layerMask, QueryTriggerInteraction.Ignore);
            if (!_hasDetectedHit) return;
            _hitPosition = hit.point;
            _hitNormal = hit.normal;
            _hitColliders.Add(hit.collider);
            _hitTransforms.Add(hit.transform);
            _hitDistance = hit.distance;
        }

        //Cast a sphere into '_direction' from '_origin';
        private void CastSphere(Vector3 origin, Vector3 direction) {
            _hasDetectedHit = Physics.SphereCast(origin, sphereCastRadius, direction, out var hit, castLength - sphereCastRadius, layerMask, QueryTriggerInteraction.Ignore);
            if (!_hasDetectedHit) return;
            _hitPosition = hit.point;
            _hitNormal = hit.normal;
            _hitColliders.Add(hit.collider);
            _hitTransforms.Add(hit.transform);
            _hitDistance = hit.distance;
            _hitDistance += sphereCastRadius;

            //Calculate real distance;
            if (calculateRealDistance) {
                _hitDistance = VectorMath.ExtractDotVector(origin - _hitPosition, direction).magnitude;
            }
            var col = _hitColliders[0];

            //Calculate real surface normal by casting an additional raycast;
            if (!calculateRealSurfaceNormal) return;
            if (col.Raycast(new Ray(_hitPosition - direction, direction), out hit, 1.5f)) {
                _hitNormal = Vector3.Angle(hit.normal, -direction) >= 89f ? _backupNormal : hit.normal;
            } else
                _hitNormal = _backupNormal;
            _backupNormal = _hitNormal;
        }

        //Calculate a direction in world coordinates based on the local axes of this gameobject's transform component;
        Vector3 GetCastDirection() {
            switch (_castDirection) {
                case CastDirection.Forward:
                    return _tr.forward;
                case CastDirection.Right:
                    return _tr.right;
                case CastDirection.Up:
                    return _tr.up;
                case CastDirection.Backward:
                    return -_tr.forward;
                case CastDirection.Left:
                    return -_tr.right;
                case CastDirection.Down:
                    return -_tr.up;
                default:
                    return Vector3.one;
            }
        }

        //Draw debug information in editor (hit positions and ground surface normals);
        public void DrawDebug() {
            if (!_hasDetectedHit || !isInDebugMode) return;
            Debug.DrawRay(_hitPosition, _hitNormal, Color.red, Time.deltaTime);
            const float markerSize = 0.2f;
            Debug.DrawLine(_hitPosition + Vector3.up * markerSize, _hitPosition - Vector3.up * markerSize, Color.green, Time.deltaTime);
            Debug.DrawLine(_hitPosition + Vector3.right * markerSize, _hitPosition - Vector3.right * markerSize, Color.green, Time.deltaTime);
            Debug.DrawLine(_hitPosition + Vector3.forward * markerSize, _hitPosition - Vector3.forward * markerSize, Color.green, Time.deltaTime);
        }

        //Getters;

        //Returns whether the sensor has hit something;
        public bool HasDetectedHit() {
            return _hasDetectedHit;
        }

        //Returns how far the raycast reached before hitting a collider;
        public float GetDistance() {
            return _hitDistance;
        }

        //Returns the surface normal of the collider the raycast has hit;
        public Vector3 GetNormal() {
            return _hitNormal;
        }

        //Returns the position in world coordinates where the raycast has hit a collider;
        public Vector3 GetPosition() {
            return _hitPosition;
        }

        //Returns a reference to the collider that was hit by the raycast;
        public Collider GetCollider() {
            return _hitColliders[0];
        }

        //Returns a reference to the transform component attached to the collider that was hit by the raycast;
        public Transform GetTransform() {
            return _hitTransforms[0];
        }

        //Setters;

        //Set the position for the raycast to start from;
        //The input vector '_origin' is converted to local coordinates;
        public void SetCastOrigin(Vector3 origin) {
            if (_tr == null) return;
            _origin = _tr.InverseTransformPoint(origin);
        }

        //Set which axis of this gameobject's transform will be used as the direction for the raycast;
        public void SetCastDirection(CastDirection direction) {
            if (_tr == null) return;
            _castDirection = direction;
        }

        //Recalculate start positions for the raycast array;
        public void RecalibrateRaycastArrayPositions() {
            _raycastArrayStartPositions = GetRaycastStartPositions(arrayRows, arrayRayCount, offsetArrayRows, sphereCastRadius);
        }
    }
}