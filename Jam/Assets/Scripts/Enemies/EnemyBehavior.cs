using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public enum FishType
{
    PlayerOriented,
    ShipOriented,
    Neutral
}

public enum FishEffect
{
    MovePHISH,
    DrainPHISH,
    DisablePHISH,
}

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] FishType fishType;

    [SerializeField, Tooltip("Does this enemy swim around?")] bool hasPath;
    [SerializeField, ShowIf("hasPath"), Tooltip("What points does this enemy swim in between?")] List<Vector3> pathingPoints;
    [SerializeField, ShowIf("hasPath")] float idleSpeed;

    [Space(5)]
    [SerializeField] float chaseSpeed;

    [Space(5)]

    Coroutine moving;

    [SerializeField, ShowIf("fishType", FishType.PlayerOriented)] float damageToPlayer;
    [SerializeField, ShowIf("fishType", FishType.PlayerOriented), 
     Tooltip("How far does the player fly back after an attack?")] float playerKB;

    [SerializeField, ShowIf("fishType", FishType.ShipOriented)] FishEffect effectOnShip;

    //TODO: make an iinteractable for the clam's pearl

    private void Start()
    {
        if(hasPath)
        {
            BeginSwimming();
        }
    }

    void BeginSwimming()
    {
        moving = StartCoroutine(SwimmingIdle());
    }

    IEnumerator SwimmingIdle()
    {
        Vector3 currentCoordinate = pathingPoints[0];
        transform.rotation = Quaternion.LookRotation(currentCoordinate);

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentCoordinate, idleSpeed * Time.deltaTime);

            if(transform.position == currentCoordinate)
            {
                int currentLoop = pathingPoints.IndexOf(currentCoordinate) + 1;

                if(currentLoop > pathingPoints.Count - 1)
                {
                    currentCoordinate = pathingPoints[0];
                }
                else { currentCoordinate = pathingPoints[currentLoop]; }

                transform.rotation = Quaternion.LookRotation(currentCoordinate);
            }

            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator SwimmingTowardsTarget(GameObject target)
    {
        while(true)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, chaseSpeed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(target.transform.position);
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnTriggerEnter(Collider trigger)
    {
        if((fishType == FishType.PlayerOriented && trigger.GetComponent<PlayerController>()) ||
           (fishType == FishType.ShipOriented && trigger.GetComponent<SubmarineInputHandler>()))
        {
            moving = null;
            moving = StartCoroutine(SwimmingTowardsTarget(trigger.gameObject));
        }
    }

    private void OnTriggerExit(Collider trigger)
    {
        if ((fishType == FishType.PlayerOriented && trigger.GetComponent<PlayerController>()) ||
           (fishType == FishType.ShipOriented && trigger.GetComponent<SubmarineInputHandler>()))
        {
            moving = null;
            moving = StartCoroutine(SwimmingIdle());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(fishType == FishType.PlayerOriented && collision.gameObject.GetComponent<PlayerController>())
        {
            collision.gameObject.GetComponent<PlayerHealthManager>().ChangeHealthValue(damageToPlayer);
            collision.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * playerKB, ForceMode.Impulse);
        }

        if(fishType == FishType.ShipOriented && collision.gameObject.GetComponent<SubmarineInputHandler>())
        {
            if(effectOnShip == FishEffect.DisablePHISH)
            {

            }
            else if(effectOnShip == FishEffect.DrainPHISH)
            {

            }
            else if(effectOnShip == FishEffect.MovePHISH)
            {

            }
        }
    }
}
