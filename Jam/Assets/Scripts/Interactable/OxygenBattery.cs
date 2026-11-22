using NUnit.Framework.Interfaces;
using UnityEngine;

public class OxygenBattery : Pickup, IInteractable
{
    public int CurrentOxygenPercentage = 100;
    [SerializeField] private ItemScriptable batteryData;
    public override void PickupItem(PlayerController pc)
    {
        heldBy = pc;
        TogglePhysics(false);
        transform.parent = pc.PickupPoint;
        transform.localPosition = Vector3.zero;
        PublicEvents.AddToInventory(batteryData);
        PublicEvents.HoldingBattery(true);
    }

    public void PlaceBattery(BatterySlot bs)
    {
        PublicEvents.RemoveFromInventory?.Invoke();
        PublicEvents.HoldingBattery(false);
        bs.Placed(this);
    }

    public override void DropItem()
    {
        heldBy = null;
        transform.parent = null;
        TogglePhysics(true);
        PublicEvents.RemoveFromInventory?.Invoke();
        PublicEvents.HoldingBattery(false);
    }


}
