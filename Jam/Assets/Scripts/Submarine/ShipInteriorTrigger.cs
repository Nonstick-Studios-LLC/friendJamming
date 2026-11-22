using UnityEngine;

public class ShipInteriorTrigger : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController pc))
        {
            pc.ToggleMovement(PlayerController.MovementType.Land);
        }
        else
        {
            // fuck you die
            Physics.IgnoreCollision(this.GetComponent<Collider>(), other, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController pc))
        {
            pc.ToggleMovement(PlayerController.MovementType.Water);
        }
    }
}
