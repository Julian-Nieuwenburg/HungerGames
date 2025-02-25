using UnityEngine;

public class BowPickup : MonoBehaviour
{
    private bool isNearPlayer = false;
    private Transform player;
    private PlayerInventory inventory;

    [Header("Bow Settings")]
    public GameObject bowModel;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearPlayer = true;
            player = other.transform;
            inventory = player.GetComponent<PlayerInventory>();

            Debug.Log("✅ Player entered pickup zone."); // Debug message
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearPlayer = false;
            Debug.Log("❌ Player left pickup zone.");
        }
    }

    void Update()
    {
        if (isNearPlayer && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("🛠 Player pressed E to pick up the bow.");
            PickUpBow(); // ✅ Calls only one valid function
        }
    }

    // ✅ Keep only one version of this function
    void PickUpBow()
    {
        if (player == null)
        {
         Debug.LogError("❌ Player reference is null!");
         return;
        }

        if (inventory == null)
        {
            Debug.LogError("❌ Inventory reference is null!");
            return;
        }

    // Add the bow to the inventory
        inventory.AddWeapon(bowModel);

    // Equip the bow immediately
        inventory.EquipWeapon(inventory.collectedWeapons.Count - 1); // Last weapon added

    // Hide the pickup object (ground bow)
        gameObject.SetActive(false);

        Debug.Log("🎯 Bow added to inventory and equipped!");
    }

}
