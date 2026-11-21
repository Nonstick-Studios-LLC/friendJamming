/*
 * Does not handle any player input. Other scripts need to call these functions.
 * Some acceleration is handled in the ship parts (propeller and floater)
 * 
 * I actually kinda fucked up and didnt implement turning right sorry gang.
 * 
 * We dont need to store a current turn speed variable because physics actually handle that for us!!
 */

using NaughtyAttributes;
using UnityEngine;

public class SubmarineInputHandler : MonoBehaviour
{
    [Header("Designer Settings")]

    [Tooltip("If 3, this means there is a -3 to 3 range (total of 7 increments")]
    [SerializeField] public int MaxRotationAccelerationIncrements = 3;
    [SerializeField] public int MaxVerticalAccelerationIncrements = 2;

    [Space(10)]

    [Tooltip("Higher number to slow turn speed faster")]
    [SerializeField] private float angularDamping = 0.2f;
    [Tooltip("Higher number to slow speed faster")]
    [SerializeField] private float velocityDamping = 0.5f;
    [SerializeField] private float totalMaxSpeed = 16;

    [Space(10)]

    [SerializeField] private float backPropellerForwardForce = 10;
    [SerializeField] private float damagedBackPropellerForwardForce = 2;
    [SerializeField] private bool backPropellerContributesToReverse = false;

    [SerializeField] private float sidePropellersForwardForce = 3;
    [SerializeField] private float damagedSidePropellersForwardForce = 1;

    [Space(10)]

    [SerializeField] private float floaterUpwardForce = 0.5f;
    [SerializeField] private float damagedFloaterUpwardForce = 0.125f;
    [SerializeField] private float floaterDownwardForce = 1;
    [SerializeField] private float damagedFloaterDownwardForce = 0.25f;

    [Space(10)]

    [Tooltip("The fastest the horizontal speed can go")]
    [SerializeField] private float maxHorizontalTurnForce = 10;
    [SerializeField] private float damagedMaxHorizontalTurnFoce = 2.5f;

    [Header("Ship Parts")]
    [SerializeField] Propeller leftPropeller;
    [SerializeField] Propeller rightPropeller;
    [SerializeField] Propeller backPropeller;
    [SerializeField] Floater floater;


    [Foldout("Ship Wheel"), SerializeField] private Transform wheelForwardRotationParent;

    [Foldout("Ship Wheel"), SerializeField] private Transform wheel;
    [Foldout("Ship Wheel"), SerializeField] private Vector2 wheelTurnRadius;

    [Foldout("Ship Wheel"), SerializeField] private Transform verticalLever;
    [Foldout("Ship Wheel"), SerializeField] private Vector2 leverRotateRadius;

    [Header("Audio")]
    [SerializeField, Foldout("Audio")] AudioSource reversingAudio;

    [Header("My variables dont touch")]
    [SerializeField] private Rigidbody rb;

    public enum TurnDirection{ None, Left, Right }
    public enum ForwardDirection{ None,Forward, Reverse }

    [Header("Debug Values")] 
    public int currentTargetTurnAcceleration { get; private set; }
    public int currentVerticalAcceleration { get; private set; }

    [SerializeField, ReadOnly] private ForwardDirection currentForwardControls = ForwardDirection.Forward;

    private float _turnAccelerationScalar => Mathf.InverseLerp(0, MaxRotationAccelerationIncrements, (float)Mathf.Abs(currentTargetTurnAcceleration));
    private float _turnAccelerationPercent => Mathf.InverseLerp(-MaxRotationAccelerationIncrements, MaxRotationAccelerationIncrements, currentTargetTurnAcceleration);
    private float _verticalAccelerationScalar => Mathf.InverseLerp(0, MaxVerticalAccelerationIncrements, (float)Mathf.Abs(currentVerticalAcceleration));
    private float _verticalAccelerationPercent => Mathf.InverseLerp(-MaxVerticalAccelerationIncrements, MaxVerticalAccelerationIncrements, currentVerticalAcceleration);

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
        if (Input.GetKey(KeyCode.Q))
            SetForwardMode(ForwardDirection.Forward);
        else if (Input.GetKey(KeyCode.E))
            SetForwardMode(ForwardDirection.Reverse);
        else
            SetForwardMode(ForwardDirection.None);

        // @jay remove this
        if (Input.GetKeyDown(KeyCode.A))
            SetTargetTurnSpeed(currentTargetTurnAcceleration - 1);
        if (Input.GetKeyDown(KeyCode.D))
            SetTargetTurnSpeed(currentTargetTurnAcceleration + 1);

        // @jay remove this too
        if (Input.GetKeyDown(KeyCode.W))
            SetTargetVerticalSpeed(currentVerticalAcceleration + 1);
        if (Input.GetKeyDown(KeyCode.S))
            SetTargetVerticalSpeed(currentVerticalAcceleration - 1);
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
        backPropeller.ApplyForce(GetBackPropellerForce());

        floater.ApplyForce(GetFloaterForce());
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

    private float GetFloaterForce()
    {
        if (currentVerticalAcceleration == 0) return 0;
        if (currentVerticalAcceleration > 0) return _verticalAccelerationScalar * (floater.IsDamaged ? damagedFloaterUpwardForce : floaterUpwardForce) * -1;
        if (currentVerticalAcceleration < 0) return _verticalAccelerationScalar * (floater.IsDamaged ? damagedFloaterDownwardForce : floaterDownwardForce);

        return float.NaN;
    }

    #endregion

    #region Player Input Interface

    /*
     * @jay: set tha forward mode
     */
    public void SetForwardMode(ForwardDirection forwardDirection)
    {
        if (forwardDirection == currentForwardControls) return;

        currentForwardControls = forwardDirection;

        switch (currentForwardControls)
        {
            case ForwardDirection.Forward:
                reversingAudio.Stop();
                wheelForwardRotationParent.localRotation = Quaternion.AngleAxis(-40f, new Vector3(1, 0, 0));
                break;
            case ForwardDirection.Reverse:
                reversingAudio.Play();
                wheelForwardRotationParent.localRotation = Quaternion.AngleAxis(20f, new Vector3(1, 0, 0));
                break;
            case ForwardDirection.None:
                reversingAudio.Stop();
                wheelForwardRotationParent.localRotation = Quaternion.AngleAxis(0, new Vector3(1, 0, 0));
                break;
        }
    }

    /*
     * -2 for all the way left. 0 for no turn. 2 for all the way right
     * 
     * @jay: add code that calls this function. the rest should be handled tbh
     */
    public void SetTargetTurnSpeed(int targetSpeed)
    {
        currentTargetTurnAcceleration = targetSpeed;
        currentTargetTurnAcceleration = (int)Mathf.Clamp(currentTargetTurnAcceleration, -MaxRotationAccelerationIncrements, MaxRotationAccelerationIncrements);
        Debug.Log($"New Turning acceleration: {currentTargetTurnAcceleration}");

        float visualAngle = Mathf.Lerp(wheelTurnRadius.x, wheelTurnRadius.y, _turnAccelerationPercent);
        wheel.transform.localEulerAngles = wheel.transform.localEulerAngles.WithY(visualAngle);
    }

    /*
     * -2 for all the way left. 0 for no turn. 2 for all the way right
     * 
     * @jay: add code that calls this function. the rest should be handled tbh
     */
    public void SetTargetVerticalSpeed(int targetSpeed)
    {
        currentVerticalAcceleration = targetSpeed;
        currentVerticalAcceleration = (int)Mathf.Clamp(currentVerticalAcceleration, -MaxVerticalAccelerationIncrements, MaxVerticalAccelerationIncrements);
        Debug.Log($"New Vertical acceleration: {currentVerticalAcceleration}");

        float visualAngle = Mathf.Lerp(leverRotateRadius.x, leverRotateRadius.y, _verticalAccelerationPercent);
        //verticalLever.transform.localEulerAngles = verticalLever.transform.localEulerAngles.WithX(visualAngle);
        verticalLever.transform.localRotation = Quaternion.AngleAxis(visualAngle, new Vector3(1,0,0));
        //verticalLever.transform.localRotation = Quaternion.Euler(verticalLever.transform.localEulerAngles.WithX(visualAngle));
        Debug.Log(visualAngle);
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
