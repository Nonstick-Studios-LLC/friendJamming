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

    void Start()
    {
        submarineInputHandler = GetComponentInParent<SubmarineInputHandler>();
    }

    #region INTERACTABLE

    public void EnterInteract(PlayerController pc)
    {
        PublicEvents.MoveDirection += MoveControls;

        if(playerController == null)
        {
            playerController = pc;
        }
        
        pc.CanMoveDirections = false;
    }

    public void ExitInteract()
    {
        PublicEvents.MoveDirection -= MoveControls;
        playerController.CanMoveDirections = true;
    }

    public void EnterHover()
    {

    }

    public void ExitHover()
    {

    }

    #endregion INTERACTABLE

    void MoveControls(Vector2 dir)
    {
        switch(movementType)
        {
            case MovementType.ForwardBackward:

                if(dir.y >= 0.5)
                {
                    submarineInputHandler.SetForwardMode(ForwardDirection.Forward);
                }
                else if(dir.y <= -0.5)
                {
                    submarineInputHandler.SetForwardMode(ForwardDirection.Reverse);
                }
                else
                {
                    submarineInputHandler.SetForwardMode(ForwardDirection.None);
                }

                break;

            case MovementType.UpDown:

                if (dir.y >= 0.5)
                {
                    submarineInputHandler.SetTargetVerticalSpeed(submarineInputHandler.currentVerticalAcceleration +1);
                }
                else if (dir.y <= -0.5)
                {
                    submarineInputHandler.SetTargetVerticalSpeed(submarineInputHandler.currentVerticalAcceleration - 1);
                }

                break;

            case MovementType.LeftRight:

                if (dir.x >= 0.5)
                {
                    submarineInputHandler.SetTargetTurnSpeed(submarineInputHandler.currentTargetTurnAcceleration + 1);
                }
                else if (dir.x <= -0.5)
                {
                    submarineInputHandler.SetTargetTurnSpeed(submarineInputHandler.currentTargetTurnAcceleration - 1);
                }

                break;

            default:
                break;
        }
    }
}
