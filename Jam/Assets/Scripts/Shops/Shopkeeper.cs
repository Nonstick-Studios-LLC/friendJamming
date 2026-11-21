using NaughtyAttributes;
using UnityEngine;

public class Shopkeeper : MonoBehaviour, IInteractable
{
    public void EnterHover()
    {
        throw new System.NotImplementedException();
    }

    [Button]
    public void EnterInteract()
    {
        EnterInteract(null);
    }
    public void EnterInteract(PlayerController pc)
    {
        PublicEvents.StartSellInteraction?.Invoke();
    }

    public void ExitHover()
    {
        throw new System.NotImplementedException();
    }

    public void ExitInteract()
    {
        throw new System.NotImplementedException();
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
