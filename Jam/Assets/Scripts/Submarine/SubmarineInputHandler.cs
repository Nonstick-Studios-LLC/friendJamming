/*
 * Does not handle any player input. Other scripts need to call these functions.
 * 
 * We dont need to store a current turn speed variable because physics actually handle that for us!!
 */

using NaughtyAttributes;
using UnityEngine;

public class SubmarineInputHandler : MonoBehaviour
{
    [Header("Designer Settings")]
    [Tooltip("Higher number to slow turn speed faster")]
    [SerializeField] private float angularDamping = 0.2f;
    [Tooltip("Higher number to slow speed faster")]
    [SerializeField] private float velocityDamping = 0.5f;

    [Space(10)]

    [SerializeField] private float maxSpeed = 16;
    [Tooltip("If 3, this means there is a -3 to 3 range (total of 7 increments")]
    [SerializeField] public int MaxSpeedIncrements = 2;

    [Space(10)]


    [SerializeField] private float backPropellerForwardForce = 10;
    [SerializeField] private float damagedBackPropellerForwardForce = 2;
    [SerializeField] private bool backPropellerContributesToReverse = false;

    [SerializeField] private float sidePropellersForwardForce = 3;
    [SerializeField] private float damagedSidePropellersForwardForce = 1;


    [Space(10)]

    [Tooltip("The fastest the horizontal speed can go")]
    [SerializeField] private float maxHorizontalTurnForce = 10;
    [SerializeField] private float damagedMaxHorizontalTurnFoce = 2.5f;

    [Header("Ship Parts")]
    [SerializeField] Propeller leftPropeller;
    [SerializeField] Propeller rightPropeller;
    [SerializeField] Propeller backPropeller;

    [Header("My variables dont touch")]
    [SerializeField] private Rigidbody rb;

    [SerializeField, ReadOnly] private ForwardDirection currentForwardControls = ForwardDirection.Forward;

    public enum TurnDirection{ None, Left, Right }
    public enum ForwardDirection{ None,Forward, Reverse }

    [Header("Debug Values")] public int currentTargetTurnAcceleration { get; private set; }

    private float _turnAccelerationScalar => Mathf.InverseLerp(0, MaxSpeedIncrements, (float)Mathf.Abs(currentTargetTurnAcceleration));

    private void Start()
    {
        rb.angularDamping = angularDamping;
        rb.linearDamping = velocityDamping;
    }

    private void Update()
    {
        // every frame so i can test in editor
        rb.angularDamping = angularDamping;
        rb.linearDamping = velocityDamping;

        // debug for now @jay get rid of this
        if (Input.GetKey(KeyCode.W))
            currentForwardControls = ForwardDirection.Forward;
        else if (Input.GetKey(KeyCode.S))
            currentForwardControls = ForwardDirection.Reverse;
        else
            currentForwardControls = ForwardDirection.None;

        // @jay remove this
        if (Input.GetKeyDown(KeyCode.A))
            SetTargetTurnSpeed(currentTargetTurnAcceleration - 1);
        if (Input.GetKeyDown(KeyCode.D))
            SetTargetTurnSpeed(currentTargetTurnAcceleration + 1);
    }


    private void FixedUpdate()
    {
        //  janky fix to evil rotation
        rb.angularVelocity = rb.angularVelocity.WithXZ(0, 0);
        transform.eulerAngles = transform.eulerAngles.WithXZ(0, 0);

        ApplyPropellerForces();
    }

    #region Propeller Forces

    private void ApplyPropellerForces()
    {
        leftPropeller.ApplyForce(GetLeftPropellerForce());
        rightPropeller.ApplyForce(GetRightPropellerForce());
    }

    private float GetLeftPropellerForce()
    {
        // if the ship is not trying to turn, otherwise side propellers priortize turning
        if (currentTargetTurnAcceleration == 0)
        {
            switch (currentForwardControls)
            {
                case ForwardDirection.Forward:
                    return leftPropeller.IsDamaged ? damagedSidePropellersForwardForce : sidePropellersForwardForce;
                case ForwardDirection.Reverse:
                    return leftPropeller.IsDamaged ? -damagedSidePropellersForwardForce : -sidePropellersForwardForce;
                case ForwardDirection.None:
                    return 0;
            }
        }
        float baseTurnForce = leftPropeller.IsDamaged ? damagedMaxHorizontalTurnFoce : maxHorizontalTurnForce;

        // if trying to turn left -> left propeller pushes forward (negative force)
        if (currentTargetTurnAcceleration < 0)
        {
            return -1 * baseTurnForce * _turnAccelerationScalar;
        }

        // if trying to turn right -> left propeller pushes backward
        if (currentTargetTurnAcceleration > 0)
        {
            return baseTurnForce * _turnAccelerationScalar;
        }

        Debug.LogError("error case. sorry.");
        return -1;
    }

    private float GetRightPropellerForce()
    {
        // if the ship is not trying to turn, otherwise side propellers priortize turning
        if (currentTargetTurnAcceleration == 0)
        {
            switch (currentForwardControls)
            {
                case ForwardDirection.Forward:
                    return rightPropeller.IsDamaged ? damagedSidePropellersForwardForce : sidePropellersForwardForce;
                case ForwardDirection.Reverse:
                    return rightPropeller.IsDamaged ? -damagedSidePropellersForwardForce : -sidePropellersForwardForce;
                case ForwardDirection.None:
                    return 0;
            }
        }
        float baseTurnForce = leftPropeller.IsDamaged ? damagedMaxHorizontalTurnFoce : maxHorizontalTurnForce;

        // if trying to turn left -> right propeller pushes backward
        if (currentTargetTurnAcceleration < 0)
        {
            return baseTurnForce * _turnAccelerationScalar;
        }

        // if trying to turn right -> left propeller pushes forward (negative force)
        if (currentTargetTurnAcceleration > 0)
        {
            return -1 * baseTurnForce * _turnAccelerationScalar;
        }

        Debug.LogError("error case. sorry.");
        return -1;
    }

    private float GetBackPropellerForce()
    {
        switch (currentForwardControls)
        {
            case ForwardDirection.Forward:
                return backPropeller.IsDamaged ? damagedBackPropellerForwardForce : backPropellerForwardForce;
            case ForwardDirection.Reverse:
                return backPropellerContributesToReverse ?
                    (backPropeller.IsDamaged ? 
                        damagedBackPropellerForwardForce : 
                        backPropellerForwardForce) :
                    0;
            case ForwardDirection.None:
                return 0;

            default: return float.NaN; // lol fuck you.
        }
    }

    #endregion

    #region Player Input Interface

    /*
     * @jay: implement it like this:
     * on W down: SetForwardMode(ForwardDirection.Forward);
     * on S down: SetForwardMode(ForwardDirection.Reverse);
     * on W or S released (make sure both are currently released): SetForwardMode(ForwardDirection.None);
     */
    public void SetForwardMode(ForwardDirection forwardDirection)
    {
        currentForwardControls = forwardDirection;
    }

    /*
     * -2 for all the way left. 0 for no turn. 2 for all the way right
     * 
     * @jay: add code that calls this function. the rest should be handled tbh
     */
    public void SetTargetTurnSpeed(int targetSpeed)
    {
        currentTargetTurnAcceleration = targetSpeed;
        currentTargetTurnAcceleration = (int)Mathf.Clamp(currentTargetTurnAcceleration, - maxHorizontalTurnForce, maxHorizontalTurnForce);
        Debug.Log($"New Turning acceleration: {currentTargetTurnAcceleration}");
    }

    #endregion

    /*
    public void WhileMoveForward()
    {
        //TODO: acceleration

        float leftSpeed  = leftPropeller.IsDamaged  ? damagedSidePropellersForwardForce : sidePropellersForwardForce;
        float rightSpeed = rightPropeller.IsDamaged ? damagedSidePropellersForwardForce : sidePropellersForwardForce;
        float backSpeed  = backPropeller.IsDamaged  ? damagedBackPropellerForwardForce  : backPropellerForwardForce;

        leftPropeller.ApplyForce(leftSpeed);
        rightPropeller.ApplyForce(rightSpeed);
        backPropeller.ApplyForce(backSpeed);

        rb.maxLinearVelocity = maxSpeed;
    }
    */
}
