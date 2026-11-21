using UnityEngine;

public interface IInteractable
{

    public abstract void EnterInteract(PlayerController pc);


    public abstract void ExitInteract();


    public abstract void EnterHover();


    public abstract void ExitHover();
  
}
