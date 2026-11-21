using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class OxygenManager : Singleton<OxygenManager>
{
    [SerializeField] private float maxOxygen = 10;

    //Add player list var
    private List<List<Coroutine>> activeRegenCoroutines;
    private List<float> oxygenValues;

    private void Initialize()
    {
        //Set up procedures
    }

    /// <summary>
    /// Registers an active player.
    /// </summary>
    public void RegisterActivePlayer(/* Add player script var*/)
    {

    }

    /// <summary>
    /// Adds a flat amount of oxygen immediately.
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="amount"></param>
    public void AddOxygenFlat(int playerID, float amount)
    {
        oxygenValues[playerID] = Mathf.Clamp(oxygenValues[playerID] + amount, 0, maxOxygen);
    }

    /// <summary>
    /// Adds oxygen at given rate for a period of time.
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="time"></param>
    /// <param name="regenRate"> Amount of oxygen a player gains per second </param>
    public void AddOxygenOverTime(int playerID, int time, float regenRate)
    {
        activeRegenCoroutines[playerID].Add(StartCoroutine(OxygenRegen(playerID, time, regenRate)));
    }

    /// <summary>
    /// Adds oxygen at given rate until player's oxygen is full.
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="regenRate"> Amount of oxygen a player gains per second. </param>
    public void AddOxygenUntilFull(int playerID, float regenRate)
    {
        activeRegenCoroutines[playerID].Add(StartCoroutine(OxygenRegenUntilFull(playerID, regenRate)));
    }

    /// <summary>
    /// Stops all oxygen regen for a player of given playerID.
    /// </summary>
    /// <param name="playerID"></param>
    public void StopPlayerOxygenRegen(int playerID)
    {
        foreach(Coroutine c in activeRegenCoroutines[playerID])
        {
            StopCoroutine(c);
        }

        activeRegenCoroutines[playerID].Clear();
    }

    /// <summary>
    /// Stops all players oxygen regen.
    /// </summary>
    public void StopAllPlayersOxygenRegen()
    {
        foreach (List<Coroutine> l in activeRegenCoroutines)
        {
            foreach(Coroutine c in l)
            {
                StopCoroutine(c);
            }

            l.Clear();
        }
    }

    /// <summary>
    /// Regenerates oxygen at given rate for the given amount of time (in seconds).
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="time"></param>
    /// <param name="rate"></param>
    /// <returns></returns>
    private IEnumerator OxygenRegen(int playerID, int time, float rate)
    {
        for(int i = 0; i < time; i++)
        {
            oxygenValues[playerID] = Mathf.Clamp(oxygenValues[playerID] + rate, 0, maxOxygen);
            yield return new WaitForSeconds(1f);
        }
    }

    /// <summary>
    /// Regenerates oxygen until the given player's oxygen is full.
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="rate"></param>
    /// <returns></returns>
    private IEnumerator OxygenRegenUntilFull(int playerID, float rate)
    {
        while (oxygenValues[playerID] < maxOxygen)
        {
            oxygenValues[playerID] = Mathf.Clamp(oxygenValues[playerID] + rate, 0, maxOxygen);
            yield return new WaitForSeconds(1f);
        }
    }
}
