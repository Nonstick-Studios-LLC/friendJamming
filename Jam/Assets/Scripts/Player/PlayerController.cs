/*************************************************
Author Names        : 		Jay Embry, Cade Naylor
Date Created        : 		11/20/2026
Brief Description   : 		Handles player input
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
    private MovementType currentMovement;

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
    [SerializeField] private char standardCrosshair;
    [SerializeField] private Color standardCrosshairColor;
    [SerializeField] private char interactableCrosshair;
    [SerializeField] private Color interactableCrosshairColor;
    private Vector3 landCollider = new Vector3(1, 2, 1);
    private Vector3 waterCollider = new Vector3(1, 1, 2);
    Movement activeMovement;
    Vector2 cameraRotation;

    public Transform PickupPoint;
    [Layer] public int transitionLayer;

    [SerializeField] private bool canMove = true;
    public bool CanMove { get { return canMove; } set { canMove = value; } }

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
        if (!CanMove)
        {
            return;
        }

        Vector2 adjustedDelta = cameraVector * CameraSensitivity * Time.fixedDeltaTime;

        cameraRotation.x -= adjustedDelta.y;
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, minCameraYClamp, maxCameraYClamp);
        cameraRotation.y += adjustedDelta.x;

        CameraRotationPoint.localEulerAngles = cameraRotation;



        if (activeMovement.LookingAtObject())
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
    }

    /// <summary>
    /// Unsubscribes from toggle movement event
    /// </summary>
    private void OnDisable()
    {
        PublicEvents.ToggleMovement -= ToggleMovement;
    }


    /// <summary>
    /// Toggles movement based on event call
    /// checks for all of dictionary and turns off all other 
    /// movment scripts and activates the toggled one.
    /// </summary>
    /// <param name="type"></param>
    private void ToggleMovement(MovementType newType)
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == transitionLayer)
        {
            ToggleMovement();
        }
    }

}