using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthContainer : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI maxHealthLabel;
    public TextMeshProUGUI currentHealthLabel;
    

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        maxHealthLabel.text = health.ToString();
        currentHealthLabel.text = health.ToString();
    }
    public void UpdateHealthBar(int health)
    {
        currentHealthLabel.text = health.ToString();
        slider.value = health;
    }
}
