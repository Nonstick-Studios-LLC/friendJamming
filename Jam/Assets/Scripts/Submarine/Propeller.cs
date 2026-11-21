using UnityEngine;


public class Propeller : MonoBehaviour
{
    public KeyCode testKey;

    [Header("My variables dont touch")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Vector3 physicsApplyPoint;
    [SerializeField] private Vector3 ForwardDirection;
    public float TestSpeed = 5;

    private Vector3 actualPhyiscsApplyPoint => physicsApplyPoint + transform.position;
    private Vector3 adjustedForwardDirection => Vector3.Cross(ForwardDirection,transform.forward); // idk what a cross product is

    private void FixedUpdate()
    {
        if (Input.GetKey(testKey))
        {
            Vector3 force = transform.forward * TestSpeed;
            rb.AddForce(force, ForceMode.Force);
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
