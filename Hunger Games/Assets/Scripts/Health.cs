using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    // The maximum health the character can have
    public int maxHealth = 100;

    // The current health of the character
    public int currentHealth;

    void Start()
    {
        // Initialize current health to max
        currentHealth = maxHealth;

        Debug.Log("Health: " + currentHealth);
    }

    // Call this method to deal damage
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // For demonstration, log the current health
        Debug.Log("Health: " + currentHealth);

        // Check if the character is "dead" 
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Call this method to heal
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Log the current health (for demonstration)
        Debug.Log("Health: " + currentHealth);
    }

    // What happens when health hits zero
    private void Die()
    {
        Debug.Log(gameObject.name + " died!");

        // Example: disable the GameObject or destroy it
        // Destroy(gameObject);
        // or
        // gameObject.SetActive(false);

        // This will vary depending on your game’s design
    }
}
