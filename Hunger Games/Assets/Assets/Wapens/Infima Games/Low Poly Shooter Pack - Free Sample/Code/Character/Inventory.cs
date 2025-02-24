using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{
    [Tooltip("Resets the entire inventory system to have 0 items picked up. This is useful for testing in editor and for restarting each game")]
    public bool resetInventoryOnStart = true;
    private List<InventoryObject> objectsInInventory;
    private List<GameObject> inventorySlots;
    [Tooltip("Should be set, leave me alone please")] public GameObject inventoryRoot;
    private GameObject inventoryObjectTemplate;
    [Tooltip("The number of slots for the system to generate")] public int maxInventorySlots = 5;
    private int currentlySelectedItem = 0;
    private List<InventoryObject> inventoryItems = new List<InventoryObject>();
    [Tooltip("Enables the tooltip text in the UI, otherwise this will be ignored")][SerializeField] private bool useTooltip;
    [Tooltip("Should be set, leave me alone please")][SerializeField] private Text tooltipText;

    void Start()
    {
        objectsInInventory = new List<InventoryObject>();
        inventorySlots = new List<GameObject>();
        inventoryObjectTemplate = GameObject.Find("InventoryTemplate");
        Rect rect = inventoryObjectTemplate.GetComponent<RectTransform>().rect;
        rect.width = rect.height;
        inventoryObjectTemplate.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.width, rect.height);
        inventoryObjectTemplate.GetComponent<RectTransform>().position = Vector3.zero;

        InitGUI();
        for (var i = 0; i < inventorySlots.Count; ++i)
            inventorySlots[i].GetComponent<InventorySlot>().ToggleSlot(false);
        inventorySlots[currentlySelectedItem].GetComponent<InventorySlot>().ToggleSlot(true);
        inventoryItems = Resources.LoadAll<InventoryObject>("InventoryItems").ToList();
        if(resetInventoryOnStart)
            ResetInventory();
        if(useTooltip)
        {
            tooltipText.gameObject.SetActive(true);
        }
    }

    // Update the input logic here to change how the the slots get toggled and the selected item is used.
    void Update()
    {
        // Cycle through the inventory
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
            ToggleSlot(true);
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
            ToggleSlot(false);

        if (Input.GetButtonDown("Submit"))
            UseSelectedItem();
        
    }

    private void ToggleSlot(bool goUp)
    {
        inventorySlots[currentlySelectedItem].GetComponent<InventorySlot>().ToggleSlot(false);
        if (goUp)
            currentlySelectedItem++;
        else
            currentlySelectedItem--;
        currentlySelectedItem = Mathf.Clamp(currentlySelectedItem, 0,inventorySlots.Count-1);
        inventorySlots[currentlySelectedItem].GetComponent<InventorySlot>().ToggleSlot(true);
        if(useTooltip)
        {
            if(currentlySelectedItem >= 0 && currentlySelectedItem < objectsInInventory.Count)
                tooltipText.text = objectsInInventory[currentlySelectedItem].objectTooltip;
            else
                tooltipText.text = "";
        }
    }

    public void ToggleSlotAtID(int id)
    {
        inventorySlots[currentlySelectedItem].GetComponent<InventorySlot>().ToggleSlot(false);
        currentlySelectedItem = id;
        currentlySelectedItem = Mathf.Clamp(currentlySelectedItem, 0,inventorySlots.Count-1);
        inventorySlots[currentlySelectedItem].GetComponent<InventorySlot>().ToggleSlot(true);
        if(useTooltip)
        {
            if(currentlySelectedItem >= 0 && currentlySelectedItem < objectsInInventory.Count)
                tooltipText.text = objectsInInventory[currentlySelectedItem].objectTooltip;
            else
                tooltipText.text = "";
        }
    }

    private void InitGUI()
    {
        float padding = 5.0f;

        bool even = maxInventorySlots%2 == 0;
        int slotCounter = 0;

        if (even)
        {
            GameObject tempGameObject = Instantiate(inventoryObjectTemplate);
            tempGameObject.transform.SetParent(inventoryRoot.transform);
            tempGameObject.GetComponent<RectTransform>().localPosition = Vector3.zero;
            var oldPos = tempGameObject.GetComponent<RectTransform>().position;
            oldPos.x -= (tempGameObject.GetComponent<RectTransform>().rect.width /2 + 2.5f);
            tempGameObject.GetComponent<RectTransform>().position = oldPos;
            inventorySlots.Add(tempGameObject);
            slotCounter++;
        }

        for (int i = 0; i < maxInventorySlots / 2; i++)
        {
            GameObject tempGameObject = Instantiate(inventoryObjectTemplate);
            tempGameObject.transform.SetParent(inventoryRoot.transform);
            tempGameObject.GetComponent<RectTransform>().localPosition = Vector3.zero;

            var oldPos = tempGameObject.GetComponent<RectTransform>().position;
            oldPos.x += i * tempGameObject.GetComponent<RectTransform>().rect.width + (i * padding) + (even ? (tempGameObject.GetComponent<RectTransform>().rect.width / 2) + 2.5f : 0);
            tempGameObject.GetComponent<RectTransform>().position = oldPos;
            int tempid = slotCounter;
            inventorySlots.Add(tempGameObject);
            slotCounter++;
        }

        for (int i = 1; i < maxInventorySlots / 2; i++)
        {
            GameObject tempGameObject = Instantiate(inventoryObjectTemplate);
            tempGameObject.transform.SetParent(inventoryRoot.transform);
            tempGameObject.GetComponent<RectTransform>().localPosition = Vector3.zero;

            var oldPos = tempGameObject.GetComponent<RectTransform>().position;
            oldPos.x -= ((i * tempGameObject.GetComponent<RectTransform>().rect.width) + (i * padding) + (even ? (tempGameObject.GetComponent<RectTransform>().rect.width / 2) + 2.5f : 0));
            tempGameObject.GetComponent<RectTransform>().position = oldPos;
            int tempid = slotCounter;
            inventorySlots.Add(tempGameObject);
            slotCounter++;
        }

        inventorySlots = inventorySlots.OrderBy(a => a.GetComponent<RectTransform>().position.x).ToList();
        for(int i = 0; i < inventorySlots.Count; ++i)
        {
            inventorySlots[i].GetComponent<InventorySlot>().slotID = i;
            inventorySlots[i].GetComponent<InventorySlot>().owningInventory = this;
            AddListener(inventorySlots[i].GetComponent<Button>(), i);
        }
        inventoryObjectTemplate.SetActive(false);
    }

    void AddListener(Button b, int value) 
    {
        b.onClick.AddListener(() => UseItemAtID(value));
    }

    
    public void AddItemToInventory(CollectableObject obj)
    {
        if(objectsInInventory.Count >= inventorySlots.Count)
            return;

        if (!objectsInInventory.Find(x => x.itemLogic.name == obj.objectReference.name))
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

        if(useTooltip)
        {
            if(currentlySelectedItem >= 0 && currentlySelectedItem < objectsInInventory.Count)
                tooltipText.text = objectsInInventory[currentlySelectedItem].objectTooltip;
            else
                tooltipText.text = "";
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
            int i;
            
            for (i = currentlySelectedItem; i < objectsInInventory.Count; i++)
            {
                inventorySlots[i].GetComponent<InventorySlot>().SetItem(objectsInInventory[i].objectImage, objectsInInventory[i].quantity);
            }
            for (var i1 = i; i1 < inventorySlots.Count; i1++)
            {
                inventorySlots[i1].GetComponent<InventorySlot>().SetItem(null, 0);
            }
        }
        if(useTooltip)
        {
            if(currentlySelectedItem >= 0 && currentlySelectedItem < objectsInInventory.Count)
                tooltipText.text = objectsInInventory[currentlySelectedItem].objectTooltip;
            else
                tooltipText.text = "";
        }
    }

    public void UseItemAtID(int id)
    {
        Debug.Log(id);
        if (id >= objectsInInventory.Count || objectsInInventory.Count == 0)
            return;

        objectsInInventory[id].quantity--;
        objectsInInventory[id].itemLogic.UseItem(transform, objectsInInventory[id]);
        inventorySlots[id].GetComponent<InventorySlot>().SetItem(objectsInInventory[id].objectImage, objectsInInventory[id].quantity);
        if (objectsInInventory[id].quantity <= 0)
        {
            objectsInInventory.RemoveAt(id);
            objectsInInventory.TrimExcess();
            int i;
            
            for (i = id; i < objectsInInventory.Count; i++)
            {
                inventorySlots[i].GetComponent<InventorySlot>().SetItem(objectsInInventory[i].objectImage, objectsInInventory[i].quantity);
            }
            for (var i1 = i; i1 < inventorySlots.Count; i1++)
            {
                inventorySlots[i1].GetComponent<InventorySlot>().SetItem(null, 0);
            }
        }
        if(useTooltip)
        {
            if(id >= 0 && id < objectsInInventory.Count)
                tooltipText.text = objectsInInventory[id].objectTooltip;
            else
                tooltipText.text = "";
        }
    }

    void ResetInventory()
    {
        foreach (InventoryObject inventoryObject in inventoryItems)
        {
            inventoryObject.quantity = 0;
        }
    }
}
