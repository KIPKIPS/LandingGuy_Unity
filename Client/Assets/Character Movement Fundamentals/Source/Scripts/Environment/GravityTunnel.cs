using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CMF {
    //This script rotates all gameobjects inside the attached trigger collider around a central axis (the forward axis of this gameobject);
    //In combination with a tube-shaped collider, this script can be used to let a player walk around on the inside walls of a tunnel;
    public class GravityTunnel : MonoBehaviour {
        //List of rigidbodies inside the attached trigger;
        private readonly List<Rigidbody> _rigidbodies = new();

        private void FixedUpdate() {
            foreach (var t in _rigidbodies) {
                //Calculate center position based on rigidbody position;
                var transform1 = transform;
                var position = transform1.position;
                var position1 = t.transform.position;
                var center = Vector3.Project((position1 - position), ((position + transform1.forward) - position)) + transform.position;
                RotateRigidbody(t.transform, (center - position1).normalized);
            }
        }

        private void OnTriggerEnter(Collider col) {
            var component = col.GetComponent<Rigidbody>();
            if (!component) return;

            //Make sure that the entering collider is actually a character;
            if (col.GetComponent<Mover>() == null) return;
            _rigidbodies.Add(component);
        }

        private void OnTriggerExit(Collider col) {
            var component = col.GetComponent<Rigidbody>();
            if (!component) return;

            //Make sure that the leaving collider is actually a character;
            if (col.GetComponent<Mover>() == null) return;
            _rigidbodies.Remove(component);
            RotateRigidbody(component.transform, Vector3.up);

            //Reset rigidbody rotation;
            var eulerAngles = component.rotation.eulerAngles;
            eulerAngles.z = 0f;
            eulerAngles.x = 0f;
            component.MoveRotation(Quaternion.Euler(eulerAngles));
        }

        private void RotateRigidbody(Transform trs, Vector3 targetDirection) {
            //Get rigidbody component of transform;
            var component = trs.GetComponent<Rigidbody>();
            targetDirection.Normalize();

            //Calculate rotation difference;
            var rotationDifference = Quaternion.FromToRotation(trs.up, targetDirection);

            //Save start and end rotation;
            var rotation = trs.rotation;
            var endRotation = rotationDifference * rotation;

            //Rotate rigidbody;
            component.MoveRotation(endRotation);
        }

        //Calculate a counter rotation from a rotation;
        private Quaternion GetCounterRotation(Quaternion rotation) {
            rotation.ToAngleAxis(out var angle, out var axis);
            var rotationAdd = Quaternion.AngleAxis(Mathf.Sign(angle) * 180f, axis);
            return rotation * Quaternion.Inverse(rotationAdd);
        }
    }
}