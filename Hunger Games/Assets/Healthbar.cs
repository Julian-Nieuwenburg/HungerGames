using UnityEngine;
using UnityEngine.UI;

public class HealthBarImage : MonoBehaviour
{
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private Image greenBar;

    void Update()
    {
        if (healthSystem != null && greenBar != null)
        {
            float fraction = (float)healthSystem.currentHealth / 100;
            greenBar.fillAmount = fraction;
        }
    }
}