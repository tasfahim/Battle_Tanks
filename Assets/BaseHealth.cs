using UnityEngine;

public abstract class BaseHealth : MonoBehaviour
{
    public int maxHealth = 100;
    protected int currentHealth;
    protected bool isDead = false;

    [HideInInspector] public string uniqueID;

    protected virtual void Start()
    {
        currentHealth = maxHealth;

        if (string.IsNullOrEmpty(uniqueID))
            uniqueID = gameObject.name + "_" + System.Guid.NewGuid().ToString();

        OnHealthChanged();
    }

    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged();

        if (currentHealth <= 0) Die();
    }

    public int GetHealth() => currentHealth;

    public void SetHealth(int value)
    {
        currentHealth = Mathf.Clamp(value, 0, maxHealth);
        OnHealthChanged();
    }

    protected abstract void Die();

    // 🔑 This lets children update UI automatically
    protected virtual void OnHealthChanged() { }
}
