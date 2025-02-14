using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 5f; // Tijd voordat de kogel verdwijnt

    void Start()
    {
        // Vernietig de kogel na een bepaalde tijd om geheugenlekken te voorkomen
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Hier kun je logica toevoegen voor wat er gebeurt als de kogel iets raakt
        // Bijvoorbeeld, vernietig de kogel en het object dat geraakt is
        Destroy(gameObject);

        // Optioneel: Voeg logica toe om het object dat geraakt is te vernietigen
        // if (collision.gameObject.CompareTag("Target"))
        // {
        //     Destroy(collision.gameObject);
        // }
    }
}
