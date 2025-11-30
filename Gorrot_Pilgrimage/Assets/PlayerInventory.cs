using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

    InventoryItem inventoryItem;

    GameObject[] currentPlayerInventory = new GameObject[4];

    public GameObject[] inventorySlots = new GameObject[4];

    bool hasFreeSlot = true;

    public InventoryItemTemplate[] allInventoryItems;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        for(int i = 0; i < allInventoryItems.Length; i++)
        {
            Debug.Log(allInventoryItems[i].name);
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TestItemAdd(string itemName, int slotIndex)
    {
       InventorySlotController inventorySlotController = inventorySlots[slotIndex].GetComponent<InventorySlotController>();
        inventorySlotController.PlaceItemInSlot(itemName);
    }

    public bool TryToAddItem(string itemName)
    {
        int freeSlotIndex = FindFreeSlot(itemName);

        // -2 Is the 'Add Duplicate' State - maybe find more elegant fix

        if (freeSlotIndex != -1 && freeSlotIndex != -2 && hasFreeSlot)
        {
            TestItemAdd(itemName, freeSlotIndex);
            return true;
        }
        else if(freeSlotIndex == -2)
        {
            return true;
        }
        else
        {
            return false;
        }



    }


    int FindFreeSlot(string itemName)
    {

        hasFreeSlot = false;

        int freeSlotIndex = -1;

        for (int i = 0;i < inventorySlots.Length;i++)
        {
            InventorySlotController inventorySlotController = inventorySlots[i].GetComponent<InventorySlotController>();

            string currentItemName = inventorySlotController.GetCurrentItemName();

            if(currentItemName != itemName)
            {
                if (inventorySlotController.CheckSlotEmpty())
                {
                    hasFreeSlot = true;
                    freeSlotIndex = i;
                    break;
                }
            }
            else
            {
                inventorySlotController.PlaceDuplcateItemInSlot();
                freeSlotIndex = -2;
                break;
            }
           

        }

        return freeSlotIndex;


    }
}
