using TMPro;
using UnityEngine;
using System.Collections;

public class ShipOxygenStorage : MonoBehaviour
{
    //call me paparazzi or whatever caleb said
    [HideInInspector] public int DrainModifier = 0;

    private float totalOxygen => Mathf.Clamp((batteries[0].CurrentOxygenPercentage + batteries[1].CurrentOxygenPercentage) / 2f, 0f, 100f);
    [SerializeField] private float drainRate = 1;
    [SerializeField] private int drainAmount = 1;
    [Tooltip("Time in seconds before game over once all oxygen batteries are empty")]
    [SerializeField] private float gameOverBufferTime = 5;

    private Coroutine batteryDrain;

    public OxygenBattery[] batteries = new OxygenBattery[2];

    [SerializeField] private TextMeshPro oxygenDisplay;

    private void Start()
    {
        batteryDrain = StartCoroutine(BatteryDrain());
    }

    private void UpdateDisplay()
    {
        oxygenDisplay.text = totalOxygen + "%";
    }

    private void RestartDrain()
    {
        batteryDrain = StartCoroutine(BatteryDrain());
    }

    /// <summary>
    /// Drains batteries by given amount at the given rate.
    /// </summary>
    /// <returns></returns>
    private IEnumerator BatteryDrain()
    {
        for(; ; )
        {
            if (batteries[0].CurrentOxygenPercentage > 0)
                batteries[0].CurrentOxygenPercentage -= (drainAmount + DrainModifier);
            else if (batteries[1].CurrentOxygenPercentage > 0)
                batteries[1].CurrentOxygenPercentage -= (drainAmount + DrainModifier);
            else
            {
                StartCoroutine(EmptyBatteryBuffer());
                StopCoroutine(batteryDrain);
            }

            UpdateDisplay();

            yield return new WaitForSeconds(drainRate);
        }
    }

    /// <summary>
    /// Buffer to allow player's time to clutch before hitting game over.
    /// </summary>
    /// <returns></returns>
    private IEnumerator EmptyBatteryBuffer()
    {
        for(; ; )
        {
            //Activate warning UI

            yield return new WaitForSeconds(gameOverBufferTime);

            if (totalOxygen <= 0)
            {
                //Activate game over
                Debug.Log("Game Over");
            }
            else
            {
                RestartDrain();
            }
        }
    }


}
