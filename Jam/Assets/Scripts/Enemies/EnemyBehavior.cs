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
    bool chasing = false;

    [SerializeField, ShowIf("fishType", FishType.PlayerOriented)] float damageToPlayer;

    [SerializeField, Tooltip("How far does a collision send the player or PHISH back?")] float knockback;

    [SerializeField, ShowIf("fishType", FishType.ShipOriented)] FishEffect effectOnShip;

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
        if(chasing)
        {
            return;
        }

        if((fishType == FishType.PlayerOriented && trigger.GetComponent<PlayerController>()) ||
           (fishType == FishType.ShipOriented && effectOnShip == FishEffect.MovePHISH && trigger.gameObject.GetComponentInParent<SubmarineInputHandler>()))
        {
            StopCoroutine(moving);
            moving = null;
            moving = StartCoroutine(SwimmingTowardsTarget(trigger.gameObject));

            chasing = true;
        }
    }

    private void OnTriggerExit(Collider trigger)
    {
        if ((fishType == FishType.PlayerOriented && trigger.GetComponent<PlayerController>()) ||
           (fishType == FishType.ShipOriented && trigger.GetComponent<SubmarineInputHandler>()))
        {
            StopCoroutine(moving);
            moving = null;

            if(hasPath)
            {
                moving = StartCoroutine(SwimmingIdle());
            }

            chasing = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(fishType == FishType.PlayerOriented && collision.gameObject.GetComponent<PlayerController>())
        {
            collision.gameObject.GetComponent<PlayerHealthManager>().ChangeHealthValue(damageToPlayer);
            collision.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * knockback, ForceMode.Impulse);
        }

        if(fishType == FishType.ShipOriented && collision.gameObject.GetComponent<SubmarineInputHandler>())
        {
            if(effectOnShip == FishEffect.DisablePHISH)
            {
                //TODO: disable controllers
                //i'll do this after making phish controllers 
            }
            else if(effectOnShip == FishEffect.DrainPHISH)
            {
                //TODO: drain oxygen
            }
            else if(effectOnShip == FishEffect.MovePHISH)
            {
                //the phish has a shit ton of mass so the knockback will have to be super high for now lol
                collision.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * knockback, ForceMode.Impulse);
            }
        }
    }
}
