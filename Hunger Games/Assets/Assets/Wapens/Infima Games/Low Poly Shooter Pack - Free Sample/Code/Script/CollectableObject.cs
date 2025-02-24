using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Collectable Object")]
public class CollectableObject : MonoBehaviour
{
    public string itemName;
    public Sprite itemIcon;
    public GameObject itemPrefab;

    // ✅ Fix: Add missing properties
    public InventoryObject objectReference;
    public int quantity;
}
