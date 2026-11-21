using NaughtyAttributes;
using UnityEngine;

public class Floater : ShipPart
{
    [Header("Design Variables")]
    [SerializeField] float acceleration = 0.1f;

    [Header("My variables dont touch")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Vector3 physicsApplyPoint;

    private Vector3 actualPhyiscsApplyPoint => physicsApplyPoint + transform.position;
    private Vector3 adjustedDirection => -rb.transform.up; 

    private Vector3 _lastForceApplied;

    [Header("Debug")]
    [SerializeField, ReadOnly] private float _realForce;


    /// <summary>
    /// This function is to be called every fixed frame that the propeller is active.
    /// </summary>
    /// <param name="targetForce"></param>
    public void ApplyForce(float targetForce)
    {
        _realForce = Mathf.MoveTowards(_realForce, targetForce, acceleration * Time.fixedDeltaTime);

        Vector3 force = adjustedDirection * _realForce;
        rb.AddForceAtPosition(force, actualPhyiscsApplyPoint, ForceMode.Force);

        _lastForceApplied = force;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.orange;

        if (Application.isPlaying)
        {
            Vector3 a = actualPhyiscsApplyPoint;
            Vector3 b = a - _lastForceApplied;

            Gizmos.DrawLine(a, b);
        }
        else
        {
            Vector3 a = actualPhyiscsApplyPoint;
            Vector3 b = a + (-adjustedDirection * 5);

            Gizmos.DrawLine(a, b);
        }

    }
}
