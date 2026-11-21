/*
 * Toby
 */

using NaughtyAttributes;
using UnityEngine;


public class Propeller : ShipPart
{
    //public KeyCode testKey;
    [Header("Design Variables")]
    [SerializeField] float acceleration;

    [Header("My variables dont touch")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Vector3 physicsApplyPoint;

    private Vector3 actualPhyiscsApplyPoint => physicsApplyPoint + transform.position;
    private Vector3 adjustedForwardDirection => rb.transform.forward; // idk what a cross product is

    private Vector3 _lastForceApplied;

    [Header("Debug")]
    [SerializeField,ReadOnly] private float _realForce;


    /// <summary>
    /// This function is to be called every fixed frame that the propeller is active.
    /// </summary>
    /// <param name="targetForce"></param>
    public void ApplyForce(float targetForce)
    {
        _realForce = Mathf.MoveTowards(_realForce, targetForce, acceleration * Time.fixedDeltaTime);

        Vector3 force = adjustedForwardDirection * _realForce;
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
            Vector3 b = a + (-adjustedForwardDirection * 5);

            Gizmos.DrawLine(a, b);
        }

    }

   
}
