using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent (typeof(Rigidbody), typeof(Collider))]

public class Treasure : Pickup, IInteractable
{
    private bool bought = true;
    [SerializeField] private ItemScriptable itemData;
    [SerializeField] bool spawned = false;

    protected override void Start()
    {
        base.Start();
        //TogglePhysics(false);
        StartCoroutine(Bandaid());
    }

    public void SetItemScriptableStatus(ItemScriptable data)
    {
        Debug.Log("Called on " + gameObject.name);
        TogglePhysics(false);
        spawned = true;
        itemData = data;
    }

    IEnumerator Bandaid()
    {
        yield return new WaitForSeconds(.1f);
        if(!spawned)
        {
            TogglePhysics(true);
        }
    }

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
