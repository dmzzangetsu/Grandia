using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthContainer : MonoBehaviour
{
    public Slider healthSlider;
    public Slider actionSlider;
    public TextMeshProUGUI maxHealthLabel;
    public TextMeshProUGUI currentHealthLabel;


    public void Start()
    {
        actionSlider.maxValue = 500;
    }

    public void SetMaxHealth(int health)
    {
        healthSlider.maxValue = health;
        healthSlider.value = health;
        maxHealthLabel.text = health.ToString();
        currentHealthLabel.text = health.ToString();
    }
    public void UpdateHealthBar(int health)
    {
        currentHealthLabel.text = health.ToString();
        healthSlider.value = health;
    }

    public void UpdateActionBar(int actionSpeed)
    {
        actionSlider.value = actionSpeed;
    }
}

