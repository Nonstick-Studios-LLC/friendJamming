using Unity.VisualScripting;
using UnityEngine;
[RequireComponent (typeof(Rigidbody), typeof(Collider))]

public class Treasure : Pickup, IInteractable
{
    private bool bought = true;
    private ItemScriptable itemData;
    public ItemScriptable ItemData { get { return itemData; } set { itemData = value;} }

    public void SetPurchaseStatus(bool status)
    {
        bought = status;
    }

    public override void PickupItem(PlayerController pc)
    {
        base.PickupItem(pc);
        PublicEvents.AddToInventory(itemData);
    }

    public override void DropItem()
    {
        base.DropItem();
        PublicEvents.RemoveFromInventory?.Invoke();
    }


}
