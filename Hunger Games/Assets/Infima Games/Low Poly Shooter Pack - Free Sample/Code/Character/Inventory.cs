using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    [Tooltip("Resets the entire inventory system to have 0 items picked up.")]
    public bool resetInventoryOnStart = true;

    [Tooltip("The number of slots for the system to generate")]
    public int maxInventorySlots = 5;

    [Tooltip("Enables the tooltip text in the UI, otherwise this will be ignored")]
    [SerializeField] private bool useTooltip;

    [Header("References")]
    [Tooltip("The parent UI object that holds all inventory slots")]
    public GameObject inventoryRoot;

    [Tooltip("Template for creating inventory slots")]
    public GameObject inventoryObjectTemplate;

    [Tooltip("Tooltip text for selected items")]
    [SerializeField] private Text tooltipText;

    private List<GameObject> inventorySlots = new List<GameObject>();
    private List<InventoryObject> objectsInInventory = new List<InventoryObject>();
    private List<InventoryObject> inventoryItems = new List<InventoryObject>();
    
    private int currentlySelectedItem = 0;
    private Camera playerCamera;
    private Canvas inventoryCanvas;

    void Start()
    {
        // ✅ Load Inventory Items from Resources
        inventoryItems = Resources.LoadAll<InventoryObject>("InventoryItems").ToList();

        // ✅ Setup UI Slots
        InitGUI();

        // ✅ If resetInventoryOnStart is true, reset inventory
        if (resetInventoryOnStart)
            ResetInventory();

        // ✅ If using tooltip, activate the tooltip text
        if (useTooltip && tooltipText != null)
        {
            tooltipText.gameObject.SetActive(true);
        }

        // ✅ Automatically find the player's camera
        FindPlayerCamera();

        // ✅ Ensure the inventory canvas is correctly configured
        ConfigureInventoryCanvas();
    }

    void Update()
    {
        // ✅ Allow scrolling to switch between inventory slots
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
            ToggleSlot(true);
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
            ToggleSlot(false);

        // ✅ Allow pressing Enter to use the selected item
        if (Input.GetButtonDown("Submit"))
            UseSelectedItem();
    }

    private void ToggleSlot(bool goUp)
    {
        inventorySlots[currentlySelectedItem].GetComponent<InventorySlot>().ToggleSlot(false);

        currentlySelectedItem = goUp ? currentlySelectedItem + 1 : currentlySelectedItem - 1;
        currentlySelectedItem = Mathf.Clamp(currentlySelectedItem, 0, inventorySlots.Count - 1);

        inventorySlots[currentlySelectedItem].GetComponent<InventorySlot>().ToggleSlot(true);

        // ✅ Update Tooltip
        if (useTooltip && tooltipText != null)
        {
            tooltipText.text = (currentlySelectedItem >= 0 && currentlySelectedItem < objectsInInventory.Count)
                ? objectsInInventory[currentlySelectedItem].objectTooltip
                : "";
        }
    }

    public void ToggleSlotAtID(int id)
    {
        inventorySlots[currentlySelectedItem].GetComponent<InventorySlot>().ToggleSlot(false);
        currentlySelectedItem = Mathf.Clamp(id, 0, inventorySlots.Count - 1);
        inventorySlots[currentlySelectedItem].GetComponent<InventorySlot>().ToggleSlot(true);

        if (useTooltip && tooltipText != null)
        {
            tooltipText.text = (currentlySelectedItem >= 0 && currentlySelectedItem < objectsInInventory.Count)
                ? objectsInInventory[currentlySelectedItem].objectTooltip
                : "";
        }
    }

    private void InitGUI()
    {
        if (inventoryRoot == null || inventoryObjectTemplate == null)
        {
            Debug.LogError("❌ InventoryRoot or InventoryObjectTemplate is missing!");
            return;
        }

        // ✅ Ensure inventorySlots list is empty before populating
        inventorySlots.Clear();

        for (int i = 0; i < maxInventorySlots; i++)
        {
            GameObject tempGameObject = Instantiate(inventoryObjectTemplate, inventoryRoot.transform);
            tempGameObject.GetComponent<RectTransform>().localPosition = Vector3.zero;
            tempGameObject.GetComponent<InventorySlot>().slotID = i;
            inventorySlots.Add(tempGameObject);
        }

        inventoryObjectTemplate.SetActive(false);
    }

    public void AddItemToInventory(CollectableObject obj)
    {
        if (objectsInInventory.Count >= inventorySlots.Count)
            return;

        if (!objectsInInventory.Any(x => x.itemLogic.name == obj.objectReference.name))
        {
            objectsInInventory.Add(obj.objectReference);
            obj.objectReference.quantity = obj.quantity;
            inventorySlots[objectsInInventory.Count - 1].GetComponent<InventorySlot>().SetItem(obj.objectReference.objectImage, obj.quantity);
        }
        else
        {
            int idx = objectsInInventory.FindIndex(x => x.itemLogic.name == obj.objectReference.name);
            objectsInInventory[idx].quantity += obj.quantity;
            inventorySlots[idx].GetComponent<InventorySlot>().SetItem(objectsInInventory[idx].objectImage, objectsInInventory[idx].quantity);
        }
    }

    public void UseSelectedItem()
    {
        if (currentlySelectedItem >= objectsInInventory.Count || objectsInInventory.Count == 0)
            return;

        objectsInInventory[currentlySelectedItem].quantity--;
        objectsInInventory[currentlySelectedItem].itemLogic.UseItem(transform, objectsInInventory[currentlySelectedItem]);
        inventorySlots[currentlySelectedItem].GetComponent<InventorySlot>().SetItem(objectsInInventory[currentlySelectedItem].objectImage, objectsInInventory[currentlySelectedItem].quantity);

        if (objectsInInventory[currentlySelectedItem].quantity <= 0)
        {
            objectsInInventory.RemoveAt(currentlySelectedItem);
            objectsInInventory.TrimExcess();
        }
    }

    public void UseItemAtID(int id)
    {
        if (id >= objectsInInventory.Count || objectsInInventory.Count == 0)
            return;

        objectsInInventory[id].quantity--;
        objectsInInventory[id].itemLogic.UseItem(transform, objectsInInventory[id]);
        inventorySlots[id].GetComponent<InventorySlot>().SetItem(objectsInInventory[id].objectImage, objectsInInventory[id].quantity);

        if (objectsInInventory[id].quantity <= 0)
        {
            objectsInInventory.RemoveAt(id);
            objectsInInventory.TrimExcess();
        }
    }

    private void ResetInventory()
    {
        foreach (InventoryObject inventoryObject in inventoryItems)
        {
            inventoryObject.quantity = 0;
        }
    }

    private void FindPlayerCamera()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            PlayerSetup playerSetup = player.GetComponent<PlayerSetup>();
            if (playerSetup != null)
            {
                playerCamera = playerSetup.playerCamera.GetComponent<Camera>();
                if (playerCamera != null)
                {
                    Debug.Log("✅ Player camera found: " + playerCamera.name);
                }
                else
                {
                    Debug.LogError("❌ Player camera not found!");
                }
            }
            else
            {
                Debug.LogError("❌ PlayerSetup component not found on Player object!");
            }
        }
        else
        {
            Debug.LogError("❌ Player object not found!");
        }
    }

    private void ConfigureInventoryCanvas()
    {
        if (inventoryRoot == null)
        {
            Debug.LogError("❌ InventoryRoot is missing! Make sure it exists in the Hierarchy.");
            return;
        }

        inventoryCanvas = inventoryRoot.GetComponent<Canvas>();
        if (inventoryCanvas != null)
        {
            inventoryCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        else
        {
            Debug.LogError("❌ Inventory Canvas not found!");
        }
    }
}
