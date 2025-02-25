using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    // Reference to the HealthSystem (assumes you have a separate script named HealthSystem)
    public HealthSystem healthSystem;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Healthtest"))
        {
            healthSystem.TakeDamage(10);
        }
    }


}