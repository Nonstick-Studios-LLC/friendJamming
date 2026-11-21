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
    public float CameraSensitivity {  get { return cameraSensitivity; }}
    public Transform CameraRotationPoint { get { return cameraRotationParent; }}

    private Dictionary<MovementType, Movement> movementScripts;

    [SerializeField] private TMP_Text playerCrosshair;
    [SerializeField] private char standardCrosshair;
    [SerializeField] private Color standardCrosshairColor;
    [SerializeField] private char interactableCrosshair;
    [SerializeField] private Color interactableCrosshairColor;

    public Transform PickupPoint;

    private bool canMove = true;
    public bool CanMove { get { return canMove; } set { canMove = value; } } 




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
                newMovement = movement;

            }
            else
            {
                movement.enabled = false;
            }

        }
    }

}