using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : BaseHealth
{
    [Header("UI Reference")]
    public Slider healthSlider;
    public Gradient gradient;
    public Image fill;

    protected override void OnHealthChanged()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
            if (fill != null && gradient != null)
                fill.color = gradient.Evaluate(healthSlider.normalizedValue);
        }
    }

    protected override void Die()
    {
        Debug.Log("💀 Player destroyed! Game Over.");
        gameObject.SetActive(false);

        // Trigger Game Over panel
        FindObjectOfType<UIManager>()?.ShowGameOver();
    }
}
