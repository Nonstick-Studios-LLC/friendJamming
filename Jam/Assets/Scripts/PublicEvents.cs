using System;
using UnityEditor.PackageManager;
using UnityEngine;

public static class PublicEvents
{
    public static Action<Vector2> MoveDirectionStarted;
    public static Action<Vector2> MoveDirection;
    public static Action MoveDirectionStopped;

    public static Action<int> UpdateScrapCount;
    public static Action ItemBought;
    public static Action StartSellInteraction;
    public static Action<ItemScriptable> StartBuyInteraction;

    public static Action<Vector2> MousePosition;
    public static Action InteractStarted;
    public static Action InteractCancelled;
    public static Action SpaceStarted;
    public static Action SpaceHeld;
    public static Action SpaceEnded;
    public static Action ShiftStarted;
    public static Action ShiftEnded;
    public static Action ControlStarted;
    public static Action ControlHeld;
    public static Action ControlEnded;
    public static Action LeftClickStarted;
    public static Action LeftClickEnded;
    public static Action<PlayerController.MovementType> ToggleMovement;
    public static Action ReelStarted;
    public static Action<float> ReelHeld; // float duration
    public static Action<float> Reeleased; // get it
    public static Action DisconnectTetherStarted;
    public static Action<float> UpdownShipStarted;
    public static Action<float> UpdownShipHeld;

    public static Action<ItemScriptable> AddToInventory;
    public static Action RemoveFromInventory;

}
