/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    11/21/2026
Brief Description : 	Handles unique land movement functionality and checks

External Resources :    Yoinked these from the first prototype and modified them
***************************************************/
using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(PlayerController))]
public class LandMovement : Movement
{
    private PlayerController pc;

    [Header("Movement Variables")]
    [SerializeField] private float baseLandMovementSpeed = 1f;
    [SerializeField] private float fullJumpHeight;
    [SerializeField] private float timeToMaxAcceleration = 5f;
    [SerializeField] private float maxAccelerationMultiplier = 3f;
    [SerializeField] private float landGravity = -10f;
    private float accelleration = 1f;
    private Coroutine shiftHold;

    private IInteractable lookingAt;
    private IInteractable interactingWith;

    [Header("Camera Interaction")]
    [SerializeField] private float sightDistance = 2f;

    bool jumpThisFrame;

    Rigidbody rb;


    /// <summary>
    /// runs when loaded into a scene
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();

    }

    /// <summary>
    /// Grabs initial references and sets initial variables
    /// </summary>
    private void Start()
    {
        pc = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }

    

    /// <summary>
    /// Return the jump height
    /// </summary>
    /// <returns></returns>
    public float GetJumpHeight()
    {
        return Mathf.Sqrt(fullJumpHeight * landGravity * -2f) + 1f;
    }

    /// <summary>
    /// Override from Movement base class
    /// Reads a value from the OnMove event and adjust velocity accordingly
    /// </summary>
    /// <param name="moveVector"></param>
    protected override void OnMove(Vector2 moveVector)
    {
        if (!pc.CanMove || !pc.CanMoveDirections)
        {
            return;
        }
        Vector3 newValue = ((pc.CameraRotationPoint.forward * moveVector.y) + (pc.CameraRotationPoint.right * moveVector.x)) * baseLandMovementSpeed * 100f * accelleration * Time.fixedDeltaTime;

        if (!Grounded() || jumpThisFrame)
        {
            newValue.y = rb.linearVelocity.y;
        }
        else
        {
            newValue.y = 0;
        }

        rb.linearVelocity = newValue;

    }

    

    /// <summary>
    /// Override from movement base class
    /// </summary>
    protected override void OnSpaceStarted()
    {
        if (!pc.CanMove)
        {
            return;
        }

        if (Grounded())
        {
            jumpThisFrame = true;
            float jumpForce = Mathf.Sqrt(fullJumpHeight * landGravity * -2f);
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
    }

    /// <summary>
    /// Override from movement base class
    /// </summary>
    protected override void OnSpaceFinished()
    {
        jumpThisFrame = false;
    }

    /// <summary>
    /// override from movement base class
    /// </summary>
    protected override void OnShiftStarted()
    {
        if (!pc.CanMove)
        {
            return;
        }
        if (shiftHold == null)
        {
            shiftHold = StartCoroutine(Accelerate());
        }
    }

    /// <summary>
    /// override from movement base class
    /// </summary>
    protected override void OnShiftFinished()
    {
        if (shiftHold != null)
        {
            StopCoroutine(shiftHold);
            shiftHold = null;
            accelleration = 1;
        }
    }


    /// <summary>
    /// Checks if the player is on the ground
    /// </summary>
    /// <returns></returns>
    protected bool Grounded()
    {
        RaycastHit hit;
        float groundCheckDistance = transform.localScale.y + .1f;
        return Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance);
    }



    /// <summary>
    /// Accellerates the player while shift is held
    /// </summary>
    /// <returns></returns>
    protected IEnumerator Accelerate()
    {
        float timer = 0f;
        while (timer < timeToMaxAcceleration)
        {
            accelleration = 1f + (timer / timeToMaxAcceleration) * maxAccelerationMultiplier;
            Mathf.Clamp(accelleration, 1f, maxAccelerationMultiplier);

            timer += Time.fixedDeltaTime;
            yield return null;
        }
    }

    protected override void OnMoveEnd()
    {
        //rb.linearVelocity = Vector3.zero;
    }

    protected override void OnInteractEnd()
    {
        // doesn't need to do anything
    }

    protected override void OnLeftClickStarted()
    {
        // handle any hold interactions
    }

    protected override void OnLeftClickFinished()
    {
        // end any hold interactions
    }

    protected override void OnControlStarted()
    {
        
    }

    protected override void OnControlEnded()
    {
        
    }
}
