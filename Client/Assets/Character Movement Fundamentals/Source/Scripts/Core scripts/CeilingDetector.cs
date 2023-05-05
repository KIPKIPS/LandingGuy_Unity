using UnityEngine;

//This (optional) component can be added to a gameobject that also has a 'AdvancedWalkerController' attached;
//It will continuously check all collision detected by the internal physics calculation;
//If a collision qualifies as the character "hitting a ceiling" (based on surface normal), the result will be stored;
//The 'AdvancedWalkerController' then can use that information to react to ceiling hits; 
public class CeilingDetector : MonoBehaviour {
    private bool _ceilingWasHit;

    //Angle limit for ceiling hits;
    public float ceilingAngleLimit = 10f;

    //Ceiling detection methods;
    //'OnlyCheckFirstContact' - Only check the very first collision contact. This option is slightly faster but less accurate than the other two options.
    //'CheckAllContacts' - Check all contact points and register a ceiling hit as long as just one contact qualifies.
    //'CheckAverageOfAllContacts' - Calculate an average surface normal to check against.
    public enum CeilingDetectionMethod {
        OnlyCheckFirstContact,
        CheckAllContacts,
        CheckAverageOfAllContacts
    }

    public CeilingDetectionMethod ceilingDetectionMethod;

    //If enabled, draw debug information to show hit positions and hit normals;
    public bool isInDebugMode;
    //How long debug information is drawn on the screen;
    private const float DebugDrawDuration = 2.0f;

    private Transform _tr;

    private void Awake() {
        _tr = transform;
    }

    private void OnCollisionEnter(Collision collision) {
        CheckCollisionAngles(collision);
    }

    private void OnCollisionStay(Collision collision) {
        CheckCollisionAngles(collision);
    }

    //Check if a given collision qualifies as a ceiling hit;
    private void CheckCollisionAngles(Collision collision) {
        var angle = 0f;
        switch (ceilingDetectionMethod) {
            case CeilingDetectionMethod.OnlyCheckFirstContact: {
                //Calculate angle between hit normal and character;
                angle = Vector3.Angle(-_tr.up, collision.contacts[0].normal);

                //If angle is smaller than ceiling angle limit, register ceiling hit;
                if (angle < ceilingAngleLimit) _ceilingWasHit = true;

                //Draw debug information;
                if (isInDebugMode) Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal, Color.red, DebugDrawDuration);
                break;
            }
            case CeilingDetectionMethod.CheckAllContacts: {
                foreach (var t in collision.contacts) {
                    //Calculate angle between hit normal and character;
                    angle = Vector3.Angle(-_tr.up, t.normal);

                    //If angle is smaller than ceiling angle limit, register ceiling hit;
                    if (angle < ceilingAngleLimit) _ceilingWasHit = true;

                    //Draw debug information;
                    if (isInDebugMode) Debug.DrawRay(t.point, t.normal, Color.red, DebugDrawDuration);
                }
                break;
            }
            case CeilingDetectionMethod.CheckAverageOfAllContacts: {
                foreach (var t in collision.contacts) {
                    //Calculate angle between hit normal and character and add it to total angle count;
                    angle += Vector3.Angle(-_tr.up, t.normal);

                    //Draw debug information;
                    if (isInDebugMode) Debug.DrawRay(t.point, t.normal, Color.red, DebugDrawDuration);
                }

                //If average angle is smaller than the ceiling angle limit, register ceiling hit;
                if (angle / collision.contacts.Length < ceilingAngleLimit) _ceilingWasHit = true;
                break;
            }
        }
    }

    //Return whether ceiling was hit during the last frame;
    public bool HitCeiling() {
        return _ceilingWasHit;
    }

    //Reset ceiling hit flags;
    public void ResetFlags() {
        _ceilingWasHit = false;
    }
}