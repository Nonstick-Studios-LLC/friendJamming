using UnityEngine;

public class ShipInteriorTrigger : MonoBehaviour
{
    [SerializeField] private bool ChildPlayer = false;
    [SerializeField]
    private Transform shipParent;
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController pc))
        {
            pc.ToggleMovement(PlayerController.MovementType.Land);

            if(ChildPlayer)
                pc.transform.SetParent(shipParent);
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

            if (ChildPlayer)
                pc.transform.SetParent(null);
        }
    }
}
