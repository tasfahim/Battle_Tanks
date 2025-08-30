using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public void SetMaxHealth(int health)
    {
        if (slider == null) return;

        slider.maxValue = health;
        slider.value = health;

        if (fill != null && gradient != null)
            fill.color = gradient.Evaluate(1f); // full health
    }

    public void SetHealth(int health)
    {
        if (slider == null) return;

        slider.value = health;

        if (fill != null && gradient != null)
            fill.color = gradient.Evaluate(slider.normalizedValue);
        // normalizedValue = slider.value / slider.maxValue
    }
}
