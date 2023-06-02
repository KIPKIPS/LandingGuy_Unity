using UnityEngine;

//This script continually aligns the rigidbody it is attached to toward a target transform;
//As a result, the rigidbody's 'up' direction will always point away from the target;
//It can be used to align a controller toward the center of a planet, for games featuring planetary gravity;
public class AlignRigidbodyToTarget : MonoBehaviour {

    //Target transform used for alignment;
    public Transform target;

    private Transform _transform;
    private Rigidbody _r;

	// Use this for initialization
	private void Start () {
        _transform = transform;
        _r = GetComponent<Rigidbody>();
        if (target != null) return;
        Debug.LogWarning("No target has been assigned.", this);
        enabled = false;
	}

	private void FixedUpdate () {

        //Get this transform's 'forward' direction;
        var forwardDirection = _transform.forward;

        //Calculate new 'up' direction;
        var newUpDirection = (_transform.position - target.position).normalized;

        //Calculate rotation between this transform's current 'up' direction and the new 'up' direction;
        var rotationDifference = Quaternion.FromToRotation(_transform.up, newUpDirection);
        //Apply the rotation to this transform's 'forward' direction;
        var newForwardDirection = rotationDifference * forwardDirection;

        //Calculate final new rotation and set this rigidbody's rotation;
        var newRotation = Quaternion.LookRotation(newForwardDirection, newUpDirection);
        _r.MoveRotation(newRotation);
    }
}
