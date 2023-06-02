using UnityEngine;

namespace CMF {
    //This simple script continually rotates the gameobject it is attached to around a chosen axis;
    //It is used in the 'ExternalCameraScene' to demonstrate a camera setup where camera and character are separate gameobjects;
    public class RotateObject : MonoBehaviour {
        private Transform _transform;
        //Speed of rotation;
        public float rotationSpeed = 20f;
        //Axis used for rotation;
        public Vector3 rotationAxis = new(0f, 1f, 0f);

        //Start;
        private void Start() {
            //Get transform component reference;
            _transform = transform;
        }

        //Update;
        private void Update() {
            //Rotate object;
            _transform.Rotate(rotationAxis * (rotationSpeed * Time.deltaTime));
        }
    }
}