using NaughtyAttributes;
using UnityEngine;

public class Shopkeeper : MonoBehaviour, IInteractable
{
    public void EnterHover()
    {
        
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
        
    }

    public void ExitInteract()
    {
        
    }

}
