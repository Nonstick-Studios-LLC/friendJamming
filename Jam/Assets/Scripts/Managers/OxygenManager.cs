using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class OxygenManager : Singleton<OxygenManager>
{
    [SerializeField] private float maxOxygen = 10;
    public static float BASE_REGEN_RATE = 1;

    private List<Coroutine> activeRegenCoroutines = new List<Coroutine>();
    private List<Coroutine> activeDepletionCoroutines = new List<Coroutine>();
    private float oxygenValues;

    protected override void Awake()
    {
        base.Awake();
        RegisterActivePlayer();
    }

    /// <summary>
    /// Registers an active player.
    /// </summary>
    public void RegisterActivePlayer()
    {
        //activeRegenCoroutines.Add(new List<Coroutine>());
    }

    /// <summary>
    /// Stops all oxygen regen for a player who is no longer in the game and clears their active regen coroutines.
    /// </summary>
    /// <param name="playerID"></param>
    public void DeregisterPlayer(int playerID)
    {
        StopPlayerOxygenRegen();
        StopPlayerOxygenDepletion();
    }

    /// <summary>
    /// Adds a flat amount of oxygen immediately.
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="amount"></param>
    public void AddOxygenFlat(float amount)
    {
        oxygenValues = Mathf.Clamp(oxygenValues+amount, 0, maxOxygen);
        if (oxygenValues == maxOxygen)
        {
            Debug.LogFormat("<color=green>The player has full personal oxygen reserves</color>");
        }
    }

    /// <summary>
    /// Adds oxygen at given rate for a period of time.
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="time"></param>
    /// <param name="regenRate"> Amount of oxygen a player gains per second </param>
    public void AddOxygenOverTime(int time, float regenRate)
    {
        activeRegenCoroutines.Add(StartCoroutine(OxygenRegen(time, regenRate)));
    }

    public void RemoveOxygenUntilEmpty(float depletionRate)
    {
        activeDepletionCoroutines.Add(StartCoroutine(OxygenDepletionUntilEmpty(depletionRate)));
    }

    /// <summary>
    /// Adds oxygen at given rate until player's oxygen is full.
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="regenRate"> Amount of oxygen a player gains per second. </param>
    public void AddOxygenUntilFull(float regenRate)
    {
        activeRegenCoroutines.Add(StartCoroutine(OxygenRegenUntilFull( regenRate)));
    }

    /// <summary>
    /// Stops all oxygen regen for a player of given playerID.
    /// </summary>
    /// <param name="playerID"></param>
    public void StopPlayerOxygenRegen()
    {
        foreach(Coroutine c in activeRegenCoroutines)
        {
            StopCoroutine(c);
        }

        activeRegenCoroutines.Clear();
    }

    public void StopPlayerOxygenDepletion()
    {
        foreach(Coroutine c in activeDepletionCoroutines)
        {
            StopCoroutine(c);
        }

        activeDepletionCoroutines.Clear();
    }

    /// <summary>
    /// Stops all players oxygen regen.
    /// </summary>
    public void StopAllPlayersOxygenRegen()
    {
        foreach (Coroutine c in activeRegenCoroutines)
        {
            StopCoroutine(c);

        }
        activeRegenCoroutines.Clear();
    }

    /// <summary>
    /// Regenerates oxygen at given rate for the given amount of time (in seconds).
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="time"></param>
    /// <param name="rate"></param>
    /// <returns></returns>
    private IEnumerator OxygenRegen(int time, float rate)
    {
        for(int i = 0; i < time; i++)
        {
            oxygenValues = Mathf.Clamp(oxygenValues + rate, 0, maxOxygen);
            yield return new WaitForSeconds(1f);
        }
    }

    /// <summary>
    /// Regenerates oxygen until the given player's oxygen is full.
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="rate"></param>
    /// <returns></returns>
    private IEnumerator OxygenRegenUntilFull(float rate)
    {
        while (oxygenValues < maxOxygen)
        {
            oxygenValues = Mathf.Clamp(oxygenValues + rate, 0, maxOxygen);
            yield return new WaitForSeconds(1f);
        }
        Debug.LogFormat("<color=green>The player has full personal oxygen reserves</color>");
    }

    private IEnumerator OxygenDepletionUntilEmpty(float rate)
    {
        while (oxygenValues > 0)
        {
            oxygenValues = Mathf.Clamp(oxygenValues - rate, 0, maxOxygen);
            yield return new WaitForSeconds(1f);
        }
        Debug.LogFormat("<color=red>The player has lost all personal oxygen</color>");
        PublicEvents.CallEndState?.Invoke(EndStateManager.EndType.Personal_Oxygen_Depletion);
    }
}
