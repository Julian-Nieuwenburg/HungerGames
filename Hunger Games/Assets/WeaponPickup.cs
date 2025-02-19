using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    private bool isNearPlayer = false;
    private Transform player;
    private PlayerInventory inventory;

    [Header("Weapon Settings")]
    public GameObject weaponModel; // The weapon to be picked up

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearPlayer = true;
            player = other.transform;
            inventory = player.GetComponent<PlayerInventory>();

            Debug.Log($"✅ Player is near {weaponModel.name}.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearPlayer = false;
            Debug.Log($"❌ Player left {weaponModel.name} area.");
        }
    }

    void Update()
    {
        // ✅ Pick up only if player is near AND presses E
        if (isNearPlayer && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"🛠 Player pressed 'E' to pick up {weaponModel.name}.");
            PickUpWeapon();
        }
    }

    void PickUpWeapon()
    {
        if (player == null || inventory == null)
        {
            Debug.LogError("❌ Player or inventory reference is missing!");
            return;
        }

        // ✅ Add the weapon to inventory
        inventory.AddWeapon(weaponModel);

        // ✅ Ensure the weapon stays visible and equips properly
        weaponModel.SetActive(true);

        // ✅ Hide the pickup object from the ground
        gameObject.SetActive(false);

        Debug.Log($"🎯 {weaponModel.name} added to inventory and equipped!");
    }
}
