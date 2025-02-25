using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Weapon Settings")]
    public List<GameObject> collectedWeapons = new List<GameObject>(); // Stores collected weapons
    private int currentWeaponIndex = 0;

    [Header("Weapon Holder")]
    public Transform weaponHolder; // The place where weapons are attached (player's hands)

    void Start()
    {
        if (collectedWeapons.Count > 0)
        {
            EquipWeapon(0); // Equip the first weapon by default
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Press 'Q' to switch weapons
        {
            SwitchWeapon();
        }
    }

    public void AddWeapon(GameObject weapon)
    {
        if (!collectedWeapons.Contains(weapon))
        {
            collectedWeapons.Add(weapon);
            weapon.SetActive(false); // Hide weapon when added
            Debug.Log(weapon.name + " added to inventory.");

            // ✅ Automatically equip the first weapon if this is the only one
            if (collectedWeapons.Count == 1)
            {
                EquipWeapon(0);
            }
        }
    }

    public void EquipWeapon(int index)
    {
        if (collectedWeapons.Count == 0) return;

        // Hide all weapons
        foreach (GameObject weapon in collectedWeapons)
        {
            weapon.SetActive(false);
        }

        // Equip selected weapon
        collectedWeapons[index].SetActive(true);
        collectedWeapons[index].transform.SetParent(weaponHolder);
        collectedWeapons[index].transform.localPosition = Vector3.zero;
        collectedWeapons[index].transform.localRotation = Quaternion.identity;

        currentWeaponIndex = index;
        Debug.Log("Equipped: " + collectedWeapons[index].name);
    }

    public void SwitchWeapon()
    {
        if (collectedWeapons.Count < 2) return; // No switching if only one weapon

        currentWeaponIndex++;
        if (currentWeaponIndex >= collectedWeapons.Count)
        {
            currentWeaponIndex = 0; // Loop back to first weapon
        }

        EquipWeapon(currentWeaponIndex);
    }
}
