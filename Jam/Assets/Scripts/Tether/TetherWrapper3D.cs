using NaughtyAttributes;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TetherWrapper3D : MonoBehaviour
{
    [Header("Tether Endpoints")]
    public Transform anchorPoint;   // Static or starting end of the tether
    public Rigidbody followPoint;  // the player

    [Header("Collision Settings")]
    public float maxTetherLength = 10f;
    public LayerMask obstacleLayer;
    [Tooltip("Small offset along surface normal to prevent self-intersection")]
    public float surfaceOffset = 0.05f;
    public int maxWrapPoints = 32;

    private LineRenderer lineRenderer;
    private readonly List<Vector3> ropeNodes = new List<Vector3>();
    private int endIdx => ropeNodes.Count - 1;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
    }

    void Start()
    {
        ResetTether();
    }

    [Button]
    public void ResetTether()
    {
        ropeNodes.Clear();
        if (anchorPoint != null && followPoint != null)
        {
            ropeNodes.Add(anchorPoint.position);
            ropeNodes.Add(followPoint.position);
        }
    }

    void LateUpdate()
    {
        if (anchorPoint == null || followPoint == null || ropeNodes.Count < 2)
            return;

        ropeNodes[0] = anchorPoint.transform.position;
        ropeNodes[endIdx] = followPoint.transform.position;

        HandleWrapping();
        HandleUnwrapping();
        //CleanupSimilarNodes();
        UpdateLineRenderer();
    }

    public float GetCurrentLength()
    {
        float totalLength = 0f;
        for (int i = 0; i < ropeNodes.Count - 1; i++)
        {
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

        for (int i = 0; i <= endIdx - 1; i++)
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

    void FixedUpdate()
    {
        //if (ropeNodes.Count < 2)
         //   return;

        // Update the player's current position in the node list for accurate length math
        ropeNodes[endIdx] = followPoint.position;

        float currentLength = GetCurrentLength();
        if (currentLength >= maxTetherLength)
        {
            Vector3 lastPivot = ropeNodes[endIdx - 1];
            Vector3 toPlayer = followPoint.position - lastPivot;
            float lastSegmentDist = toPlayer.magnitude;

            if (lastSegmentDist > Mathf.Epsilon)
            {
                Vector3 outwardDir = toPlayer / lastSegmentDist;

                // 1. Strip out any velocity moving away from the pivot (preserves swinging/tangential movement)
                float outwardSpeed = Vector3.Dot(followPoint.linearVelocity, outwardDir);
                if (outwardSpeed > 0f)
                {
                    followPoint.linearVelocity -= outwardDir * outwardSpeed;
                }

                // 2. Clamp position so the player can't slowly drift past maxTetherLength
                float overshoot = currentLength - maxTetherLength;
                if (overshoot > 0f)
                {
                    followPoint.MovePosition(followPoint.position - outwardDir * overshoot);
                }
            }
        }
    }

    private void UpdateLineRenderer()
    {
        lineRenderer.positionCount = ropeNodes.Count;
        lineRenderer.SetPositions(ropeNodes.ToArray());
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