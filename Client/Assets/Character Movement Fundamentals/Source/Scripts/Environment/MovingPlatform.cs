using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CMF {
    //This script moves a rigidbody along a set of waypoints;
    //It also moves any controllers on top along with it;
    public class MovingPlatform : MonoBehaviour {
        //Movement speed;
        public float movementSpeed = 10f;

        //Check to reverse order of waypoints;
        public bool reverseDirection;

        //Wait time after reaching a waypoint;
        public float waitTime = 1f;

        //This boolean is used to stop movement while the platform is waiting;
        private bool _isWaiting;

        //References to attached components;
        private Rigidbody _r;
        private TriggerArea _triggerArea;

        //List of transforms used as waypoints;
        public List<Transform> waypoints = new();
        private int _currentWaypointIndex;
        private Transform _currentWaypoint;

        //Start;
        private void Start() {
            //Get references to components;
            _r = GetComponent<Rigidbody>();
            _triggerArea = GetComponentInChildren<TriggerArea>();

            //Disable gravity, freeze rotation of rigidbody and set to kinematic;
            _r.freezeRotation = true;
            _r.useGravity = false;
            _r.isKinematic = true;

            //Check if any waypoints have been assigned and if not, throw a warning;
            if (waypoints.Count <= 0) {
                Debug.LogWarning("No waypoints have been assigned to 'MovingPlatform'!");
            } else {
                //Set first waypoint;
                _currentWaypoint = waypoints[_currentWaypointIndex];
            }

            //Start coroutines;
            StartCoroutine(WaitRoutine());
            StartCoroutine(LateFixedUpdate());
        }

        //This coroutine ensures that platform movement always occurs after Fixed Update;
        private IEnumerator LateFixedUpdate() {
            var instruction = new WaitForFixedUpdate();
            while (true) {
                yield return instruction;
                MovePlatform();
            }
        }

        private void MovePlatform() {
            //If no waypoints have been assigned, return;
            if (waypoints.Count <= 0) return;
            if (_isWaiting) return;

            //Calculate a vector to the current waypoint;
            var toCurrentWaypoint = _currentWaypoint.position - transform.position;

            //Get normalized movement direction;
            var movement = toCurrentWaypoint.normalized;

            //Get movement for this frame;
            movement *= movementSpeed * Time.deltaTime;

            //If the remaining distance to the next waypoint is smaller than this frame's movement, move directly to next waypoint;
            //Else, move toward next waypoint;
            if (movement.magnitude >= toCurrentWaypoint.magnitude || movement.magnitude == 0f) {
                _r.transform.position = _currentWaypoint.position;
                UpdateWaypoint();
            } else {
                _r.transform.position += movement;
            }
            if (_triggerArea == null) return;

            //Move all controllrs on top of the platform the same distance;
            foreach (var t in _triggerArea.rigidbodiesInTriggerArea) {
                t.MovePosition(t.position + movement);
            }
        }

        //This function is called after the current waypoint has been reached;
        //The next waypoint is chosen from the list of waypoints;
        private void UpdateWaypoint() {
            if (reverseDirection)
                _currentWaypointIndex--;
            else
                _currentWaypointIndex++;

            //If end of list has been reached, reset index;
            if (_currentWaypointIndex >= waypoints.Count) _currentWaypointIndex = 0;
            if (_currentWaypointIndex < 0) _currentWaypointIndex = waypoints.Count - 1;
            _currentWaypoint = waypoints[_currentWaypointIndex];

            //Stop platform movement;
            _isWaiting = true;
        }

        //Coroutine that keeps track of the wait time and sets 'isWaiting' back to 'false', after 'waitTime' has passed;
        private IEnumerator WaitRoutine() {
            var waitInstruction = new WaitForSeconds(waitTime);
            while (true) {
                if (_isWaiting) {
                    yield return waitInstruction;
                    _isWaiting = false;
                }
                yield return null;
            }
        }
    }
}