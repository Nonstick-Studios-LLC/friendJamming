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

    public void EnterInteract(PlayerController pc)
    {
        Debug.Log("Hit");
        if (isHeld) return;

        PickupItem(pc);

        Debug.Log($"{gameObject.name} is starting its interaction");
    }


}
