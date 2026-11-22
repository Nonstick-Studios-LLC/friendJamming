using TMPro;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    [SerializeField] float playerHealthMax;
    [HideInInspector] public float CurrentHealth;

    [SerializeField] TMP_Text healthDisplay;

    private void Start()
    {
        CurrentHealth = playerHealthMax;
        healthDisplay.text = $"Health: {CurrentHealth}";
    }

    public void ChangeHealthValue(float damage)
    {
        CurrentHealth -= damage;

        if(CurrentHealth < 0)
        {
            CurrentHealth = 0;
        }

        healthDisplay.text = $"Health: {CurrentHealth}";

        if (CurrentHealth <= 0)
        {
            //TODO: KILL player
        }
    }
}
