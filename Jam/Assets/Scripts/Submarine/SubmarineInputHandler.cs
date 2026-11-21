/*
 * Does not handle any player input. Other scripts need to call these functions
 */

using NaughtyAttributes;
using UnityEngine;

public class SubmarineInputHandler : MonoBehaviour
{
    [Header("Designer Settings")]
    [SerializeField] private float maxSpeed = 16;

    [Space(10)]

    [SerializeField] private float sidePropellersForwardSpeed = 3;
    [SerializeField] private float backPropellerForwardSpeed = 10;

    [SerializeField] private float damagedSidePropellersForwardSpeed = 1;
    [SerializeField] private float damagedBackPropellerForwardSpeed = 2;

    [SerializeField] private float horizontalTurnScalar = 10; 
    [SerializeField] private float damagedHorizontalTurnScalar = 2.5f;

    [Header("Ship Parts")]
    [SerializeField] Propeller leftPropeller;
    [SerializeField] Propeller rightPropeller;
    [SerializeField] Propeller backPropeller;

    [Header("My variables dont touch")]
    [SerializeField] private Rigidbody rb;

    [Header("Debug Values")]
    [SerializeField,ReadOnly] private float currentHorizontalTurnSpeed;

    private void FixedUpdate()
    {
        //  janky fix to evil rotation
        rb.angularVelocity = rb.angularVelocity.WithXZ(0, 0);
        transform.eulerAngles = transform.eulerAngles.WithXZ(0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            WhileMoveForward();
        }
    }

    public void WhileMoveForward()
    {
        //TODO: acceleration

        float leftSpeed  = leftPropeller.IsDamaged  ? damagedSidePropellersForwardSpeed : sidePropellersForwardSpeed;
        float rightSpeed = rightPropeller.IsDamaged ? damagedSidePropellersForwardSpeed : sidePropellersForwardSpeed;
        float backSpeed  = backPropeller.IsDamaged  ? damagedBackPropellerForwardSpeed  : backPropellerForwardSpeed;

        leftPropeller.ApplyForce(leftSpeed);
        rightPropeller.ApplyForce(rightSpeed);
        backPropeller.ApplyForce(backSpeed);

        rb.maxLinearVelocity = maxSpeed;
    }
}
