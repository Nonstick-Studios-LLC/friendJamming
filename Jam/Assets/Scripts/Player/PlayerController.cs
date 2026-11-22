/*************************************************
Author Names        : 		Jay Embry, Cade Naylor
Date Created        : 		11/20/2026
Brief Description   : 		Handles player input functions
                            Handles shared behavior across input systems (look, interact)
                            
External Resources  : 	    Nonme
***************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using System.Collections;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public enum MovementType
    {
        Land, Water
    };

    public enum InteractStates
    {
        NonInteractable, Interactable
    }
    [SerializeField] private MovementType startingMovement;
    [ReadOnly] public MovementType currentMovement;

    [SerializeField] private Camera playerCamera;
    [SerializeField] private float cameraSensitivity;
    [SerializeField] private Transform cameraRotationParent;
    [SerializeField] public Transform tetherAnchorPoint;
    public float CameraSensitivity {  get { return cameraSensitivity; }}
    public Transform CameraRotationPoint { get { return cameraRotationParent; }}
    [SerializeField] private float minCameraYClamp = -60;
    [SerializeField] private float maxCameraYClamp = 60f;
    [SerializeField] private GameObject arms;

    private Dictionary<MovementType, Movement> movementScripts;

    [SerializeField] private TMP_Text playerCrosshair;
    [SerializeField] private Slider reelInTimer;
    [SerializeField] private char standardCrosshair;
    [SerializeField] private Color standardCrosshairColor;
    [SerializeField] private char interactableCrosshair;
    [SerializeField] private Color interactableCrosshairColor;
    private Vector3 landCollider = new Vector3(1, 2, 1);
    private Vector3 waterCollider = new Vector3(1, 1, 2);
    Movement activeMovement;
    Vector2 cameraRotation;
    [SerializeField] private float sightDistance = 10f;
    [SerializeField] private float secondsToReelIn = 2;

    public Transform PickupPoint;
    [Layer] public int transitionLayer;
    private bool lookingAtShopkeeper = false;


    private IInteractable lookingAt;
    private IInteractable interactingWith;

    [SerializeField] private bool canMove = true;

    //this sucks but i just want a solution rn man
    [SerializeField] private bool canMoveDirections = true;
    [SerializeField] public bool ActivelyGettingReeled = false;
    public bool CanMove { get { return canMove && !ActivelyGettingReeled; } set { canMove = value; } }
    public bool CanMoveDirections { get { return canMoveDirections && !ActivelyGettingReeled; } set { canMoveDirections = value; } }

    private void Awake()
    {
        PublicEvents.MousePosition += OnMouseMove;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDestroy()
    {
        PublicEvents.MousePosition -= OnMouseMove;
    }


    /// <summary>
    /// Start function
    /// Populates MovementType Movement dictionary
    /// Sets default movement script to active
    /// </summary>
    private void Start()
    {
        Cursor.visible = false;
        SetCursorType(InteractStates.NonInteractable);
        reelInTimer.value = 0;

        movementScripts = new Dictionary<MovementType, Movement>()
            {
                { MovementType.Land, GetComponent<LandMovement>() },
                { MovementType.Water, GetComponent<WaterMovement>() }
            };

        ToggleMovement(startingMovement);

    }

    /// <summary>
    /// Override from Movement base class
    /// Adjusts the delta using sensitivity and fixed timee
    /// Rotates the camera between clamps
    /// also lowkey this wont work as well for multiplayer but for now i'm throwing rayxast look logic in here
    /// </summary>
    /// <param name="cameraVector">Vector2 containing the mouse's delta </param>
    private void OnMouseMove(Vector2 cameraVector)
    {
        if (!CanMove || ActivelyGettingReeled)
        {
            return;
        }

        Vector2 adjustedDelta = cameraVector * CameraSensitivity * Time.fixedDeltaTime;

        cameraRotation.x -= adjustedDelta.y;
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, minCameraYClamp, maxCameraYClamp);
        cameraRotation.y += adjustedDelta.x;

        CameraRotationPoint.localEulerAngles = cameraRotation;



        if (LookingAtObject())
        {
            SetCursorType(InteractStates.Interactable);
        }
        else
        {
            SetCursorType(InteractStates.NonInteractable);
        }

        if (arms != null)
        {
            Vector3 armsRotation = arms.transform.localEulerAngles;
            armsRotation.y = cameraRotation.y;
            armsRotation.x = Mathf.Clamp(cameraRotation.x, -40, 40);
            arms.transform.localEulerAngles = armsRotation;
        }
    }

    public void ResetInteraction()
    {
        interactingWith = null;
    }

    public void SetCursorType(InteractStates type)
    {
        playerCrosshair.text = (type == InteractStates.NonInteractable ? standardCrosshair : interactableCrosshair).ToString();
        playerCrosshair.color = (type == InteractStates.NonInteractable ? standardCrosshairColor : interactableCrosshairColor);
    }

    /// <summary>
    /// Subscribes to Toggle Movement Event
    /// </summary>
    private void OnEnable()
    {
        PublicEvents.ToggleMovement += ToggleMovement;
        PublicEvents.InteractStarted += OnInteractStart;
        PublicEvents.ReelHeld += WhileReelHeld;
        PublicEvents.Reeleased += OnReeleased;
    }

    /// <summary>
    /// Unsubscribes from toggle movement event
    /// </summary>
    private void OnDisable()
    {
        PublicEvents.ToggleMovement -= ToggleMovement;
        PublicEvents.InteractStarted -= OnInteractStart;
        PublicEvents.ReelHeld -= WhileReelHeld;
        PublicEvents.Reeleased -= OnReeleased;
    }

    #region interac


    /// <summary>
    /// Checks if the player is looking at an object that can be interacted with
    /// </summary>
    /// <returns>Returns true if they are</returns>
    public bool LookingAtObject()
    {
        RaycastHit hit;
        Vector3 direction = CameraRotationPoint.forward;

        if (Physics.Raycast(CameraRotationPoint.transform.position, direction, out hit, sightDistance, 7))
        {
            Debug.Log("Hit " + gameObject.name);
            if (hit.transform.GetComponent<IInteractable>() != null)
            {
                if(hit.transform.GetComponent<Shopkeeper>()!=null)
                {
                    lookingAtShopkeeper = true;
                }
                else
                {
                    lookingAtShopkeeper = false;
                }
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

                SetCursorType(InteractStates.Interactable);

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

        SetCursorType(InteractStates.NonInteractable);
        return false;
    }

    /// <summary>
    /// Override from Movement base class
    /// </summary>
    public void OnInteractStart()
    {
        if (!CanMove)
        {
            return;
        }

        if(interactingWith == null)
        {
            interactingWith = null;
        }

        if (lookingAt != null)
        {
            if(lookingAtShopkeeper)
            {
                lookingAt.EnterInteract(this);
                return;
            }
            if (interactingWith == null)
            {
                interactingWith = lookingAt;
                interactingWith.EnterInteract(this);
            }
            else
            {
                if (lookingAt != interactingWith)
                {
                    
                    interactingWith.ExitInteract();
                    interactingWith = lookingAt;
                    interactingWith.EnterInteract(this);
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

    #endregion

    #region Reeling in
    // hmm this seems like a good place to put this region,. yes.
    void WhileReelHeld(float duration)
    {
        if(currentMovement == MovementType.Land)
        {
            reelInTimer.value = 0;
            return;
        }

        reelInTimer.value = duration / secondsToReelIn;
    }

    void OnReeleased(float duration)
    {
        //reelInTimer.value = 0;

        //if (duration > secondsToReelIn)
        if(reelInTimer.value >= 1)
        {
            TetherManager.Instance.ReeelItInBuddy();
        }
        reelInTimer.value = 0;
    }

    #endregion

    #region movement type
    /// <summary>
    /// Toggles movement based on event call
    /// checks for all of dictionary and turns off all other 
    /// movment scripts and activates the toggled one.
    /// </summary>
    /// <param name="type"></param>
    public void ToggleMovement(MovementType newType)
    {
        currentMovement = newType;
        
        Movement newMovement = movementScripts[0];
        foreach (var movement in movementScripts.Values)
        {
            if (movementScripts[newType].Equals(movement))
            {
                movement.enabled = true;
                activeMovement = movement;
                newMovement = movement;

            }
            else
            {
                movement.enabled = false;
            }

        }

        if(newType == MovementType.Land)
        {
            GetComponent<BoxCollider>().size = landCollider;
        }
        else
        {
            GetComponent<BoxCollider>().size = waterCollider;
        }

        Debug.Log("Current movement set to " + newType.ToString());
    }

    private void ToggleMovement()
    {
        if(currentMovement == MovementType.Land)
        {
            ToggleMovement(MovementType.Water);
        }
        else
        {
            ToggleMovement(MovementType.Land);
        }
    }

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == transitionLayer)
        {
            //ToggleMovement();
        }
    }

}