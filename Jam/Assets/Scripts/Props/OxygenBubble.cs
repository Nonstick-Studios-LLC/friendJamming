using System.Collections;
using UnityEngine;

public class OxygenBubble : MonoBehaviour
{
    [SerializeField] Vector3 bubbleSpeed;
    [SerializeField] Rigidbody rb;

    private BubbleChest parentChest;

    public void Initialize(BubbleChest chest)
    {
        parentChest = chest;
        rb.linearVelocity = bubbleSpeed;
    }
    private void Update()
    {
        rb.linearVelocity = bubbleSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
        {
            Debug.Log($"bubble on chest: {parentChest.gameObject.name} has been popped");
            parentChest.BubbleHasBeenPopped = true;

            // @jacob do this
            Debug.LogError("TODO: implement bubble oxygen interaction");
        }

        Destroy(this.gameObject);
    }

    IEnumerator DeleteTimeline()
    {
        yield return new WaitForSeconds(parentChest.bubbleLifetimeSeconds);
        Destroy(this.gameObject);
    }
}
