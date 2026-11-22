using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureSpawner : MonoBehaviour
{
    [SerializeField, Tooltip("Any potential treasure that may spawn here")] private List<ItemScriptable> potentialTreasures = new List<ItemScriptable>();
    [SerializeField] private bool spawnsMultiple = false;
    [SerializeField, ShowIf(nameof(spawnsMultiple)), Tooltip("True if this spawner can have multiple treasures spawned at once")] private bool moreThanOnePresentAtATime = false;
    [SerializeField, ShowIf(EConditionOperator.And, nameof(spawnsMultiple), nameof(moreThanOnePresentAtATime))] private int maxTreasureAtOnce = 1;
    [SerializeField, ShowIf(nameof(spawnsMultiple))] private float timeBetweenSpawns = 3f;
    [SerializeField, ShowIf(nameof(spawnsMultiple))] private bool canSpawnUnlimitedTreasures = false;
    private bool limitedTreasure => !canSpawnUnlimitedTreasures;
    [SerializeField, ShowIf(EConditionOperator.And, nameof(spawnsMultiple), nameof(limitedTreasure))] private int maxTreasureTotal = 3;
    [SerializeField] private bool delayBeforeSpawningFirstItem = false;
    [SerializeField, ShowIf(nameof(delayBeforeSpawningFirstItem))] private float delayBeforeFirstSpawn = 1f;
    private int spawnedItemCount;
    private Coroutine spawningTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(!spawnsMultiple && !delayBeforeSpawningFirstItem)
        {
            SpawnItem();
        }
        else
        {
            spawningTimer = StartCoroutine(Spawner());
        }
    }

    private void SpawnItem()
    {
        GameObject itemToSpawn;
        if(potentialTreasures.Count == 0)
        {
            return;
        }
        int spawnIndex = UnityEngine.Random.Range(0, potentialTreasures.Count - 1);
        itemToSpawn = potentialTreasures[spawnIndex].GetVisual();
        GameObject spawnedItem = Instantiate(itemToSpawn, gameObject.transform);
        spawnedItemCount++;
        Treasure t = spawnedItem.GetComponent<Treasure>();
        t.ItemData = potentialTreasures[spawnIndex];
        t.TogglePhysics(true);
    }

    private IEnumerator Spawner()
    {
        bool waitedBeforeFirstSpawn = false;
        while ((spawnsMultiple && canSpawnUnlimitedTreasures) || (spawnsMultiple && spawnedItemCount < maxTreasureTotal) || (delayBeforeSpawningFirstItem && !waitedBeforeFirstSpawn))
        {
            if (delayBeforeSpawningFirstItem && !waitedBeforeFirstSpawn)
            {
                waitedBeforeFirstSpawn = true;
                yield return new WaitForSeconds(delayBeforeFirstSpawn);
            }

            if ((!moreThanOnePresentAtATime && spawnsMultiple) || transform.childCount >= maxTreasureAtOnce )
            {
                yield return new WaitForSeconds(timeBetweenSpawns);
                continue;
            }
           

            SpawnItem();

            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }
}
