/*
 * Got a lot of this code online tbh but did have to repurpose sooo much its basically mine atp
 * -Toby
 */

using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TetherManager : Singleton<TetherManager>
{
    [Header("Tether Endpoints")]
    public Transform anchorPoint;   // Static or starting end of the tether
    public Rigidbody followRB;  // the player
    public Transform followPoint;  // the player
    private PlayerController playerController;  // the player

    [Header("Collision Settings")]
    public float maxTetherLength = 10f;
    public float maxStretchLimit = 12f;

    public LayerMask obstacleLayer;
    [Tooltip("Small offset along surface normal to prevent self-intersection")]
    public float surfaceOffset = 0.05f;
    public int maxWrapPoints = 32;

    [Header("Spring")]
    [Tooltip("How strongly the tether pulls back when stretched")]
    public float springStiffness = 50f;
    [Tooltip("Prevents endless bouncing/oscillation")]
    public float springDamping = 5f;

    [Header("Reels")] // like instagram
    [SerializeField] float ReelSpeed = 5;

    [Foldout("Audio")] public Transform soundPlayPoint;
    [Foldout("Audio")] public AudioClip unhookTetherSound;

    private LineRenderer lineRenderer;
    private readonly List<Vector3> ropeNodes = new List<Vector3>();
    private int endIdx => ropeNodes.Count - 1;

    private bool playerLandMode => playerController.currentMovement == PlayerController.MovementType.Land; 
    private bool reelingIn = false; 
    [ReadOnly] public bool playerConnected = false; 
    private bool calculateTether => !playerLandMode && !reelingIn && playerConnected;

    private Coroutine reelCoroutine;

    void Start()
    {
        PublicEvents.DisconnectTetherStarted += DisconnectTetherStarted;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;

        // dont even.
        playerController = FindFirstObjectByType<PlayerController>();

        if (followRB == null)
        {
            followRB = playerController.GetComponent<Rigidbody>();
            followPoint = playerController.tetherAnchorPoint;
        }

        ResetTether();
    }

    [Button]
    public void ResetTether()
    {
        playerConnected = true;
        ropeNodes.Clear();
        if (anchorPoint != null && followPoint != null)
        {
            ropeNodes.Add(anchorPoint.position);
            ropeNodes.Add(followPoint.position);
        }
    }

    void LateUpdate()
    {
        if (!calculateTether || reelingIn || !playerConnected)
        {
            UpdateLineRenderer();
            return;
        }

        if (anchorPoint == null || followPoint == null || ropeNodes.Count < 2)
            return;

        ropeNodes[0] = anchorPoint.transform.position;
        ropeNodes[endIdx] = followPoint.transform.position;

        HandleWrapping();
        HandleUnwrapping();
        CleanupSimilarNodes();
        UpdateLineRenderer();
    }

    public float GetCurrentLength()
    {
        float totalLength = 0f;
        for (int i = 0; i < ropeNodes.Count - 1; i++)
        {
            // so inneficent who gaf
            totalLength += Vector3.Distance(ropeNodes[i], ropeNodes[i + 1]);
        }
        return totalLength;
    }

    #region self adjusting

    private void HandleWrapping()
    {
        if (ropeNodes.Count >= maxWrapPoints) return;

        if (CheckForNewWrap(0, 1)) return;
        if (CheckForNewWrap(endIdx-1, endIdx)) return;
    }

    private bool CheckForNewWrap(int idx1, int idx2)
    {
        Vector3 start = ropeNodes[idx1];
        Vector3 end = ropeNodes[idx2];

        // Check if an obstacle intersects the final segment
        bool ifHit = StaticUtilities.SphereLinecast(start, end, surfaceOffset, out RaycastHit hit, obstacleLayer);
        if (ifHit && !(StaticUtilities.ManhattanDistance(start, hit.point) < 0.1f || StaticUtilities.ManhattanDistance(end, hit.point) < 0.1f))
        {

            // Push the new corner slightly off the collider surface so future linecasts don't immediately hit it
            Vector3 wrapPoint = hit.point + hit.normal * surfaceOffset * 1.1f;
            ropeNodes.Insert(idx2, wrapPoint);
            Debug.Log("new tether point");

        }

        Debug.DrawLine(start, end, ifHit ? Color.green : Color.red);

        return false ;
    }

    private void HandleUnwrapping()
    {
        for (int i = 0; i <= endIdx-2 ; i++)
        {
            Vector3 a = ropeNodes[i];
            Vector3 c = ropeNodes[i + 2];

            //bool ifHit = StaticUtilities.SphereLinecast(start, end, surfaceOffset, out RaycastHit hit, obstacleLayer);
            bool ifHit = StaticUtilities.SphereLinecast(a, c, surfaceOffset, out RaycastHit hit, obstacleLayer);
            if (!ifHit)
            {
                ropeNodes.RemoveAt(i+1); // remove b
                Debug.Log("deleting tether point");
                return; // oh but what if it returns before we got them all? uhh this functions running every frame. its ok lil bro bro.
            }
        }
    }

    private void CleanupSimilarNodes()
    {
        if (ropeNodes.Count <= 2) return;

        for (int i = endIdx-2; i <= endIdx - 1; i++)
        {
            Vector3 a = ropeNodes[i];
            Vector3 b = ropeNodes[i + 1];

            if(StaticUtilities.ManhattanDistance(a,b) < 0.1f)
            {
                Debug.Log("deleting similar nodes");
                ropeNodes.RemoveAt(i + 1);
                return; // oh but what if it returns before we got them all? uhh this functions running every frame. its ok lil bro bro.
            }
        }
    }
    #endregion

    #region reeling
    
    /// <summary>
    /// Either youre going to get dumb fuck function names like this or youre not getting any work from me. so pick your poison, man.
    /// This is my cost. This is my price.
    /// </summary>
    public void ReeelItInBuddy()
    {
        Debug.Log("reeling it in!");
        reelingIn = true;
        playerController.ActivelyGettingReeled = true;

        if (reelCoroutine != null) return;

        reelCoroutine = StartCoroutine(ReeeelItInBuddy2());
    }

    private IEnumerator ReeeelItInBuddy2()
    {
        int currentNodeIdx = endIdx-1;
        float timeAtThisNode = 0;

        //followRB.GetComponent<Collider>().enabled = false;
        followRB.detectCollisions = false;

        while(ropeNodes.Count > 2)
        {
            timeAtThisNode += Time.deltaTime;

            // were close enough! yay!
            // or
            // ur taking too long. just fuckin teleport
            if(Vector3.Distance(followPoint.position, ropeNodes[currentNodeIdx]) < 0.2f 
                || Vector3.Distance(followRB.position, ropeNodes[currentNodeIdx]) < 0.2f
                || timeAtThisNode > 3)
            {
                followRB.position = ropeNodes[currentNodeIdx];
                ropeNodes.RemoveAt(currentNodeIdx);
                currentNodeIdx = endIdx - 1;
                timeAtThisNode = 0;
            }

            Vector3 direction = (ropeNodes[currentNodeIdx] - followPoint.position).normalized;
            //followRB.linearVelocity = direction * ReelSpeed;
            followRB.position = Vector3.MoveTowards(followRB.position, ropeNodes[currentNodeIdx], Time.fixedDeltaTime * ReelSpeed);

            yield return new WaitForFixedUpdate();
        }

        timeAtThisNode = 0;
        while (Vector3.Distance(ropeNodes[0], ropeNodes[endIdx]) > 5 || timeAtThisNode < 1)
        {
            timeAtThisNode += Time.fixedDeltaTime;
            followRB.position = Vector3.MoveTowards(followRB.position, ropeNodes[0], Time.fixedDeltaTime * ReelSpeed);
            yield return new WaitForFixedUpdate();
        }

        Vector3 direction2 = (anchorPoint.position - followPoint.position).normalized;
        followRB.linearVelocity = direction2 * ReelSpeed;

        reelingIn = false;
        playerController.ActivelyGettingReeled = false;
        Debug.Log("reel done go home");

        followRB.detectCollisions = true;
        reelCoroutine = null;
    }

    #endregion

    #region disconnect moments

    public void DisconnectTetherStarted()
    {
        playerConnected = false;

        if (reelCoroutine != null) return;

        reelCoroutine = StartCoroutine(ReelItInNoPlayer());
    }

    IEnumerator ReelItInNoPlayer()
    {
        Debug.Log("Playing reel in animation (no player)");

        playerConnected = false;

        while (ropeNodes.Count > 2)
        {
            ropeNodes[0] = anchorPoint.position;

            // were close enough! yay!
            if (Vector3.Distance(ropeNodes[endIdx-1], ropeNodes[endIdx]) < 0.1f)
            {
                ropeNodes.RemoveAt(endIdx);
            }

            //followRB.linearVelocity = direction * ReelSpeed;
            ropeNodes[endIdx] = Vector3.MoveTowards(ropeNodes[endIdx], ropeNodes[endIdx-1], Time.fixedDeltaTime * ReelSpeed);

            yield return null;
        }

        while (Vector3.Distance(ropeNodes[0], ropeNodes[1]) > 0.1f)
        {
            ropeNodes[1] = Vector3.MoveTowards(ropeNodes[1], ropeNodes[0], Time.fixedDeltaTime * ReelSpeed);
            yield return null;
        }

        reelCoroutine = null;
    }

    #endregion

    void FixedUpdate()
    {
        if(playerConnected)
            ropeNodes[endIdx] = followPoint.position;

        if (!calculateTether) return;

        float currentLength = GetCurrentLength();
        float stretch = currentLength - maxTetherLength;

        // Only apply tension if the rope is taut (stretched past maxTetherLength)
        if (stretch > 0f)
        {
            Vector3 lastPivot = ropeNodes[endIdx - 1];
            Vector3 toPlayer = followRB.position - lastPivot;
            float lastSegmentDist = toPlayer.magnitude;

            if (lastSegmentDist > Mathf.Epsilon)
            {
                

                // this makes the tether feel good but breaks literally everything
                //followRB.AddForce(springForce + dampingForce, ForceMode.Force);

                if ((maxStretchLimit > maxTetherLength && currentLength > maxStretchLimit) || ropeNodes.Count > maxWrapPoints)
                {
                    Vector3 outwardDir = toPlayer / lastSegmentDist;

                    Vector3 springForce = -outwardDir * (stretch * springStiffness);

                    float radialVelocity = Vector3.Dot(followRB.linearVelocity, outwardDir);
                    Vector3 dampingForce = -outwardDir * (radialVelocity * springDamping);

                    if (radialVelocity > 0f)
                        followRB.linearVelocity -= outwardDir * radialVelocity;

                    float hardOvershoot = currentLength - (maxStretchLimit);
                    followRB.MovePosition(followRB.position - outwardDir * hardOvershoot);
                }
            }
        }
    }

    private void UpdateLineRenderer()
    {
        if (playerLandMode)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        lineRenderer.positionCount = ropeNodes.Count;
        lineRenderer.SetPositions(ropeNodes.ToArray());
        soundPlayPoint.position = ropeNodes[endIdx];
    }

    void OnDrawGizmos()
    {
        foreach(var r in ropeNodes)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(r, surfaceOffset);
        }
    }
}