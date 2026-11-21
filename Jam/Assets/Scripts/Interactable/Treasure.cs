using UnityEngine;

public class Treasure : MonoBehaviour, IInteractable
{
    private bool bought = true;
    private ItemScriptable itemData;
    public ItemScriptable ItemData { get { return itemData; } set { itemData = value;} }

    public void SetPurchaseStatus(bool status)
    {
        bought = status;
    }


    public void EnterHover()
    {
        
    }

    public void EnterInteract(PlayerController pc)
    {
        // get picked up
    }

    public void ExitHover()
    {
        
    }

    public void ExitInteract()
    {
        // get dropped?
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
