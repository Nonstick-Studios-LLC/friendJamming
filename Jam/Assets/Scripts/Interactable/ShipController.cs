using System.Collections;
using UnityEngine;

public enum MovementType
{
    ForwardBackward,
    UpDown,
    LeftRight,
}

public class ShipController : MonoBehaviour, IInteractable
{
    SubmarineInputHandler submarineInputHandler;
    PlayerController playerController;

    [SerializeField] MovementType movementType;

    [SerializeField] Transform teleportPlayerSpot;

    void Start()
    {
        submarineInputHandler = GetComponentInParent<SubmarineInputHandler>();
    }

    #region INTERACTABLE

    Coroutine whileInteractingCoroutine;

    public void EnterInteract(PlayerController pc)
    {
        Debug.Log("interact ship control");

        PublicEvents.MoveDirection += MoveControls_Held;
        PublicEvents.MoveDirectionStarted += Move_controls_staert;
        PublicEvents.UpdownShipStarted += Updownship_start;

        if(playerController == null)
        {
            playerController = pc;
        }

        pc.CanMoveDirections = false;
        pc.transform.position = teleportPlayerSpot.position;
        whileInteractingCoroutine = StartCoroutine(WhileDrivingUpdate());
    }

    public void ExitInteract()
    {

        PublicEvents.MoveDirection -= MoveControls_Held;
        PublicEvents.MoveDirectionStarted -= Move_controls_staert;
        PublicEvents.UpdownShipStarted -= Updownship_start;

        submarineInputHandler.SetForwardMode(ForwardDirection.None);
        submarineInputHandler.SetTargetTurnSpeed(0);

        playerController.CanMoveDirections = true;
        StopCoroutine(whileInteractingCoroutine);
    }

    IEnumerator WhileDrivingUpdate()
    {
        // stop coroutine is called later dw
        while (true)
        {
            playerController.transform.position = teleportPlayerSpot.position;
            yield return new WaitForFixedUpdate();
        }
    }

    public void EnterHover()
    {

    }

    public void ExitHover()
    {

    }

    #endregion INTERACTABLE

    void Move_controls_staert(Vector2 dir)
    {
        // left right
        if (dir.x >= 0.5)
        {
            submarineInputHandler.SetTargetTurnSpeed(submarineInputHandler.currentTargetTurnAcceleration + 1);
        }
        else if (dir.x <= -0.5)
        {
            submarineInputHandler.SetTargetTurnSpeed(submarineInputHandler.currentTargetTurnAcceleration - 1);
        }
    }

    void Updownship_start(float dir)
    {
        Debug.Log("updown");
        //updown albumn
        if (dir > 0)
        {
            submarineInputHandler.SetTargetVerticalSpeed(submarineInputHandler.currentVerticalAcceleration + 1);
        }
        else if (dir < 0)
        {
            submarineInputHandler.SetTargetVerticalSpeed(submarineInputHandler.currentVerticalAcceleration - 1);
        }
    }

    void MoveControls_Held(Vector2 dir)
    {
        // forward
        if (dir.y >= 0.5)
        {
            submarineInputHandler.SetForwardMode(ForwardDirection.Forward);
        }
        else if (dir.y <= -0.5)
        {
            submarineInputHandler.SetForwardMode(ForwardDirection.Reverse);
        }
        else
        {
            submarineInputHandler.SetForwardMode(ForwardDirection.None);
        }

        return;

    }
}
