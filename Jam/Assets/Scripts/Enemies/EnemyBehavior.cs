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
    StopPHISH,
}

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] FishType fishType;

    [SerializeField, Tooltip("Does this enemy swim around?")] bool hasPath;
    [SerializeField, ShowIf("hasPath"), Tooltip("What points does this enemy swim in between?")] List<Transform> pathingPoints;
    [SerializeField, ShowIf("hasPath")] float idleSpeed;

    [Space(5)]
    [SerializeField] float chaseSpeed;

    [Space(5)]

    Coroutine moving;
    bool chasing = false;

    [SerializeField, ShowIf("fishType", FishType.PlayerOriented)] float damageToPlayer;

    [SerializeField, Tooltip("How far does a collision send the player or PHISH back?")] float knockback;

    [SerializeField, ShowIf("fishType", FishType.ShipOriented)] FishEffect effectOnShip;

    [SerializeField, ShowIf("effectOnShip", FishEffect.DrainPHISH), Tooltip("How much more oxygen will this enemy drain from the PHISH?")]
    int drainAmount;
    [SerializeField, ShowIf("effectOnShip", FishEffect.StopPHISH), Tooltip("How long does this enemy disable the PHISH controls?")]
    int stunTime;


    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
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
        Vector3 currentCoordinate = pathingPoints[0].transform.position;
        int currentLoop = 1;

        //transform.rotation = Quaternion.LookRotation(currentCoordinate);
        transform.up = (currentCoordinate - transform.position).normalized;

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentCoordinate, idleSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, currentCoordinate) < 0.1f)
            {
                currentLoop++;

                if(currentLoop > pathingPoints.Count)
                {
                    currentLoop = 1;
                }

                currentCoordinate = pathingPoints[currentLoop - 1].transform.position;

                Vector3 direction = (currentCoordinate - transform.position).normalized;
                transform.up = direction;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator SwimmingTowardsTarget(GameObject target)
    {
        while(true)
        {
            //transform.position = Vector3.MoveTowards(transform.position, target.transform.position, chaseSpeed * Time.deltaTime);

            Vector3 direction = (target.transform.position - transform.position).normalized;
            rb.AddForce(direction * chaseSpeed, ForceMode.VelocityChange);

            //transform.rotation = Quaternion.LookRotation(target.transform.position);
            transform.up = direction;

            yield return null;
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
            collision.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * knockback, ForceMode.Impulse);
        }

        if(fishType == FishType.ShipOriented && collision.gameObject.GetComponent<SubmarineInputHandler>())
        {
            if(effectOnShip == FishEffect.StopPHISH)
            {
                //so that the phish doesn't get stunlocked
                collision.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * knockback, ForceMode.Impulse);
                collision.gameObject.GetComponent<SubmarineInputHandler>().StartStunCycle(stunTime);
            }
            else if(effectOnShip == FishEffect.DrainPHISH)
            {
                collision.gameObject.GetComponent<ShipOxygenStorage>().DrainModifier += drainAmount;
            }
            else if(effectOnShip == FishEffect.MovePHISH)
            {
                //the phish has a shit ton of mass so the knockback will have to be super high for now lol
                collision.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * knockback, ForceMode.Impulse);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (fishType == FishType.ShipOriented && collision.gameObject.GetComponent<SubmarineInputHandler>() &&
            effectOnShip == FishEffect.DrainPHISH)
        {
            ShipOxygenStorage storage = collision.gameObject.GetComponent<ShipOxygenStorage>();

            storage.DrainModifier -= drainAmount;

            if(storage.DrainModifier < 0)
            {
                storage.DrainModifier = 0;
            }
        }
    }
}
