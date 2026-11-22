using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class BubbleChest : MonoBehaviour
{
    [SerializeField] OxygenBubble bubblePrefab;
    [SerializeField] Animator animator;
    [SerializeField] float secondsBetweenHypeBubbleMomentsAndAura = 30;
    [SerializeField] public float bubbleLifetimeSeconds = 15;
    [SerializeField] public Vector3 bubblespawn;
    [SerializeField] public float bubbleSize;

    [ReadOnly] public bool BubbleHasBeenPopped;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(BubbleSpawnInterval());
    }

    IEnumerator BubbleSpawnInterval()
    {
        float initialDelay = UnityEngine.Random.Range(0.25f, secondsBetweenHypeBubbleMomentsAndAura);
        yield return new WaitForSeconds(initialDelay);

        while (!BubbleHasBeenPopped)
        {
            animator.SetTrigger("Open Chest");
            yield return new WaitForSeconds(secondsBetweenHypeBubbleMomentsAndAura);
        }
    }

    /// <summary>
    /// Called by animation event
    /// </summary>
    public void InstantiateBubble()
    {
        var bubble = Instantiate(bubblePrefab, bubblespawn + transform.position, Quaternion.identity);
        bubble.transform.localScale = Vector3.one * bubbleSize;
        bubble.Initialize(this);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + bubblespawn, bubbleSize/2);
    }
}
