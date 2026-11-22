using UnityEngine;

public class Pickup : MonoBehaviour, IInteractable
{

    protected PlayerController heldBy;
    protected Rigidbody rb;
    protected Collider col;
    protected bool isHeld;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }
    public void EnterHover()
    {
        
    }

    public void EnterInteract(PlayerController pc)
    {
        if (isHeld) return;
        heldBy = pc;
        PickupItem(pc);

        Debug.Log($"{gameObject.name} is starting its interaction");
    }

    public void ExitHover()
    {
        
    }

    public void ExitInteract()
    {
        DropItem();
    }

    /// <summary>
    /// Sets the item as picked up
    /// </summary>
    public virtual void PickupItem(PlayerController pc)
    {
        PublicEvents.RemoveFromInventory += DestroyItem;
        heldBy = pc;
        TogglePhysics(false);
        transform.parent = pc.PickupPoint;
        transform.localPosition = Vector3.zero;
    }

    public virtual void DestroyItem()
    {
        heldBy.ResetInteraction();
        heldBy = null;
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        PublicEvents.RemoveFromInventory -= DestroyItem;
    }

    public virtual void DropItem()
    {
        PublicEvents.RemoveFromInventory -= DestroyItem;
        heldBy = null;
        transform.parent = null;
        TogglePhysics(true);
    }

    public void TogglePhysics(bool physicsEnabled)
    {
        if(col == null)
        {
            col = GetComponent<Collider>();
        }
        col.enabled = physicsEnabled;
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        rb.isKinematic = !physicsEnabled;
    }

}
