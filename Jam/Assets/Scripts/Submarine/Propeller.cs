using UnityEngine;


public class Propeller : ShipPart
{
    public KeyCode testKey;

    [Header("My variables dont touch")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Vector3 physicsApplyPoint;
    [SerializeField] private Vector3 ForwardDirection;
    public float TestSpeed = 5;

    private Vector3 actualPhyiscsApplyPoint => physicsApplyPoint + transform.position;
    private Vector3 adjustedForwardDirection => rb.transform.up; // idk what a cross product is

    private void FixedUpdate()
    {
        if (Input.GetKey(testKey))
        {
            Vector3 force = adjustedForwardDirection * TestSpeed;
            rb.AddForceAtPosition(force, actualPhyiscsApplyPoint, ForceMode.Force);
        }   
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.orange;

        Vector3 a = actualPhyiscsApplyPoint;
        Vector3 b = a + (-adjustedForwardDirection * TestSpeed);

        Gizmos.DrawLine(a, b);
    }
}
