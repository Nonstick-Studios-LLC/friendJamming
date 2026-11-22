using UnityEngine;

public class Pickup : MonoBehaviour, IInteractable
{

    protected PlayerController heldBy;
    protected Rigidbody rb;
    protected Collider col;
    protected bool isHeld;

    protected void Start()
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
    public void PickupItem(PlayerController pc)
    {
        heldBy = pc;
        TogglePhysics(false);
        transform.parent = pc.PickupPoint;
        transform.localPosition = Vector3.zero;
    }

    public void DropItem()
    {
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
