using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : BaseHealth
{
    [Header("UI Reference (HUD)")]
    public Slider healthSlider;
    public Gradient gradient;
    public Image fill;

    protected override void Start()
    {
        base.Start();
        OnHealthChanged();
    }

    protected override void OnHealthChanged()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;

            if (fill != null && gradient != null)
                fill.color = gradient.Evaluate(healthSlider.normalizedValue);
        }
    }

    protected override void Die()
    {
        Debug.Log("🔥 Enemy destroyed!");
        gameObject.SetActive(false);

        if (healthSlider != null)
            healthSlider.gameObject.SetActive(false);

        // ✅ Immediately show Mission Complete
        UIManager ui = FindObjectOfType<UIManager>();
        if (ui != null)
        {
            Debug.Log("🎉 Mission Complete!");
            ui.ShowMissionComplete();
        }
    }
}
