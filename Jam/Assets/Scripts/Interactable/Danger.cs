using UnityEngine;

public class Danger : MonoBehaviour
{
    public bool damagesShip;
    public float damageNumber = 1;
    public bool killsPlayer;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<ShipPart>() && damagesShip)
        {
            collision.gameObject.GetComponent<ShipPart>().TakeDamage(damageNumber);
        }

        if(collision.gameObject.GetComponent<PlayerController>() && killsPlayer)
        {
            //connect to the code that kills the player
        }
    }
}
