using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

  

    public GameObject[] inventorySlots = new GameObject[4];

    bool hasFreeSlot = true;

    List<InventoryItemTemplate> allItemsList = new List<InventoryItemTemplate>();

    public ItemCatalogue itemCatalogue;


    private void Start()
    {
        BuildItemsList();
    }

    void TestItemAdd(string itemID, int slotIndex)
    {
       InventorySlotController inventorySlotController = inventorySlots[slotIndex].GetComponent<InventorySlotController>();


        foreach (InventoryItemTemplate item in allItemsList)
        {
            if (item.itemID == itemID)
            {
                inventorySlotController.PlaceItemInSlot(itemID);
                return;
            }

        }
        
    }

    InventorySlotController GetInventorySlotController(int index)
    {
        return inventorySlots[index].GetComponent<InventorySlotController>();

    }

    void AddDuplicateItems(string itemID, int slotIndex)
    {
        Debug.Log("Should Add Duplicate in slot: " + slotIndex);
    }

    public bool TryToAddItem(string itemID)
    {
        int freeSlotIndex = FindFreeSlot(itemID);
        bool itemAlreadyHeld = false;

        Debug.Log("Try To Add Item: " + itemID);

        // First, check if the Inventory already has this item:

       for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlotController inventorySlotController = GetInventorySlotController(i);

            Debug.Log("Current item in slot: " + i + " : " + inventorySlotController.GetCurrentItemID());

            if(inventorySlotController.GetCurrentItemID() == itemID)
            {
                itemAlreadyHeld = true;
                Debug.Log("Item Already Held");
                break;
            }


        }

        // -2 Is the 'Add Duplicate' State - maybe find more elegant fix

        if (freeSlotIndex != -1 && freeSlotIndex != -2 && hasFreeSlot)
        {
            TestItemAdd(itemID, freeSlotIndex);
            return true;
        }
        else if(freeSlotIndex == -2)
        {
            AddDuplicateItems(itemID, freeSlotIndex);
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

            string currentItemName = inventorySlotController.GetCurrentItemID();

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

    void BuildItemsList()
    {
        allItemsList = itemCatalogue.GetAllItems();
    }
}
