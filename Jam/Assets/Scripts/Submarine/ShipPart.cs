
using NaughtyAttributes;
using UnityEngine;

public abstract class ShipPart : MonoBehaviour
{
    public float MaxHealth = 10;

    public bool IsDamaged => currentHealth <= 0;

    [ReadOnly, SerializeField] float currentHealth;

    protected virtual void Start()
    {
        currentHealth = MaxHealth;
    }

    /// <summary>
    /// Damage is a positive number
    /// </summary>
    /// <param name="damage"></param>
    public virtual void TakeDamage(float damage)
    {
        // @sky call this function
        MaxHealth -= damage;
        MaxHealth = Mathf.Clamp(MaxHealth, 0, MaxHealth);
    }

    [Button]
    public void DebugBreakInstantly()
    {
        currentHealth = 0;
    }
}
