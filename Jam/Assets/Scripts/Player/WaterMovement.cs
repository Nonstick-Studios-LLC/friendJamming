/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    07/30/2026
Date Last Modified : 	07/30/2026
Brief Description : 	Actually defines and handles water movement

External Resources :    	
***************************************************/
using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(PlayerController))]
public class WaterMovement : Movement
{
    private PlayerController pc;
    [SerializeField] private float minCameraYClamp = -60;
    [SerializeField] private float maxCameraYClamp = 60f;

    [SerializeField] private float baseWaterMovementSpeed = 1f;
    [SerializeField] private float timeToMaxAcceleration = 5f;
    [SerializeField] private float maxAccelerationMultiplier = 3f;
    [SerializeField] private float ascentWaterMovementSpeed = .5f;
    [SerializeField] private float descentWaterMovementSpeed = 1.2f;
    [SerializeField] private float waterGravity = -4f;
    private float accelleration = 1f;
    private Coroutine shiftHold;

    private bool ascending;
    private bool descending;
    private bool moving;

    Vector2 cameraRotation;
    Rigidbody rb;

    private IInteractable lookingAt;
    private IInteractable interactingWith;
    private bool interacting;
    private Coroutine waterGravityCour;
    [SerializeField] private float sightDistance = 2f;

    /// <summary>
    /// Grabs initial references and sets initial variables
    /// </summary>
    private void Start()
    {
        pc = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        cameraRotation = new Vector2(pc.CameraRotationPoint.eulerAngles.y, pc.CameraRotationPoint.eulerAngles.x);
    }


    /// <summary>
    /// Override from movement base class to add extra functionality
    /// Disables the player's gravity when in swim mode
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y * .5f, rb.linearVelocity.z);
        waterGravityCour = StartCoroutine(WaterGravity());

    }

    /// <summary>
    /// Override from movement base class to add extra functionality
    /// Enables the player's gravity when exiting swim mode
    /// </summary>
    protected override void OnDisable()
    {
        base.OnDisable();
        rb.useGravity = true;
        StopCoroutine(waterGravityCour);
    }

    /// <summary>
    /// Override from Movement base class
    /// Adjusts the delta using sensitivity and fixed timee
    /// Rotates the camera between clamps
    /// </summary>
    /// <param name="cameraVector">Vector2 containing the mouse's delta </param>
    protected override void OnMouseMove(Vector2 cameraVector)
    {
        if (!pc.CanMove)
        {
            return;
        }
        Vector2 adjustedDelta = cameraVector * pc.CameraSensitivity * Time.fixedDeltaTime;

        cameraRotation.x -= adjustedDelta.y;
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, minCameraYClamp, maxCameraYClamp);
        cameraRotation.y += adjustedDelta.x;

        pc.CameraRotationPoint.localEulerAngles = cameraRotation;


        if (LookingAtObject())
        {
            pc.SetCursorType(PlayerController.InteractStates.Interactable);
        }
        else
        {
            pc.SetCursorType(PlayerController.InteractStates.NonInteractable);
        }

    }

    /// <summary>
    /// Override from Movement base class
    /// Reads a value from the OnMove event and adjust velocity accordingly
    /// </summary>
    /// <param name="moveVector"></param>
    protected override void OnMove(Vector2 moveVector)
    {
        if (!pc.CanMove)
        {
            return;
        }
        moving = true;
        Vector3 newValue = ((pc.CameraRotationPoint.forward * moveVector.y) + (pc.CameraRotationPoint.right * moveVector.x)) * baseWaterMovementSpeed * 100f * accelleration * Time.fixedDeltaTime;

        if (ascending || descending)
        {
            newValue.y = rb.linearVelocity.y;
        }
        else
        {
            newValue.y = waterGravity;
        }

        rb.linearVelocity = newValue;

    }




    /// <summary>
    /// Override from Movement base class
    /// </summary>
    protected override void OnInteractStart()
    {
        if (!pc.CanMove)
        {
            return;
        }
        if (lookingAt != null)
        {
            if (interactingWith == null)
            {
                interactingWith = lookingAt;
                interactingWith.EnterInteract(pc);
            }
            else
            {
                if (lookingAt != interactingWith)
                {
                    interactingWith.ExitInteract();
                    interactingWith = lookingAt;
                    interactingWith.EnterInteract(pc);
                }
                else
                {
                    interactingWith.ExitInteract();
                    interactingWith = null;
                }
            }
        }
        else if (interactingWith != null)
        {
            interactingWith.ExitInteract();
            if (interactingWith == lookingAt)
            {
                interactingWith.EnterHover();
            }
            interactingWith = null;
        }

    }

    /// <summary>
    /// Override from movement base class
    /// Should make the player ascend in the water
    /// </summary>
    protected override void OnSpaceStarted()
    {
        if (!pc.CanMove)
        {
            return;
        }
        ascending = true;

        float ascendForce = ascentWaterMovementSpeed * accelleration * 100f * Time.fixedDeltaTime;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, ascendForce, rb.linearVelocity.z);
    }

    /// <summary>
    /// Override from movement base class
    /// </summary>
    protected override void OnSpaceFinished()
    {
        ascending = false;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
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
    /// Checks if the player is looking at an object that can be interacted with
    /// </summary>
    /// <returns>Returns true if they are</returns>
    protected override bool LookingAtObject()
    {
        RaycastHit hit;
        Vector3 direction = pc.CameraRotationPoint.forward;

        if (Physics.Raycast(pc.CameraRotationPoint.transform.position, direction, out hit, sightDistance))
        {
            if (hit.transform.GetComponent<IInteractable>() != null)
            {

                if (interactingWith != null && hit.transform.GetComponent<IInteractable>() == interactingWith)
                {
                    return true;
                }

                if (lookingAt != null && hit.transform.GetComponent<IInteractable>() != lookingAt)
                {
                    lookingAt.ExitHover();
                }

                lookingAt = hit.transform.GetComponent<IInteractable>();
                lookingAt.EnterHover();

                pc.SetCursorType(PlayerController.InteractStates.Interactable);

                return true;
            }
        }

        if (lookingAt != null)
        {
            if (lookingAt != interactingWith)
            {
                lookingAt.ExitHover();
            }
            lookingAt = null;
        }

        pc.SetCursorType(PlayerController.InteractStates.NonInteractable);
        return false;
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

            Debug.Log(accelleration);
            yield return null;
        }
    }

    /// <summary>
    /// Fakes adding water gravity
    /// </summary>
    /// <returns></returns>

    protected IEnumerator WaterGravity()
    {
        Vector3 newValue;
        while(true)
        {
            newValue = rb.linearVelocity;
            if(!moving && !ascending && !descending)
            {
                newValue.y = waterGravity;
            }
            rb.linearVelocity = newValue;

            yield return null;
        }
    }

    /// <summary>
    /// Override from movement class
    /// Stops movement
    /// </summary>
    protected override void OnMoveEnd()
    {
        moving = false;
    }

    protected override void OnInteractEnd()
    {
        // doesnt need to do anything
    }

    protected override void OnLeftClickStarted()
    {
        // start held action
    }

    protected override void OnLeftClickFinished()
    {
        // end held action
    }
}