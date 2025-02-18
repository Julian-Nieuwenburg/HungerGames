using UnityEngine;
using UnityEngine.UI;

public class HealthBarImage : MonoBehaviour
{
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private Image greenBar; // Drag the "GreenBar" Image here in the Inspector

    void Start()
    {
        // If not set in Inspector, try to find it
        if (healthSystem == null)
        {
            healthSystem = GetComponent<HealthSystem>();
        }
    }

    void Update()
    {
        // Safely handle if we have references
        if (healthSystem != null && greenBar != null)
        {
            // Convert currentHealth (0 - maxHealth) to a fraction (0.0 - 1.0)
           

            // Update the Image's fillAmount
            greenBar.fillAmount = (float)healthSystem.currentHealth / healthSystem.maxHealth;
        }
    }
}
