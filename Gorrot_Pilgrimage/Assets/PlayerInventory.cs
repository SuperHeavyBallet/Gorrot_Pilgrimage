using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

    InventoryItem inventoryItem;

    GameObject[] currentPlayerInventory = new GameObject[4];

    public GameObject[] inventorySlots = new GameObject[4];

    bool hasFreeSlot = true;

    List<InventoryItemTemplate> allItemsList = new List<InventoryItemTemplate>();

    public ItemCatalogue itemCatalogue;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        allItemsList = itemCatalogue.GetAllItems();

        foreach (InventoryItemTemplate item in allItemsList)
        {
            Debug.Log(item.itemID);
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TestItemAdd(string itemID, int slotIndex)
    {
       InventorySlotController inventorySlotController = inventorySlots[slotIndex].GetComponent<InventorySlotController>();

        foreach (InventoryItemTemplate item in allItemsList)
        {
            if (item.itemID == itemID)
            {
                Debug.Log("This Item: " + itemID + "Is In The List of Items");
                inventorySlotController.PlaceItemInSlot(itemID);
                return;
            }

        }
        
    }

    public bool TryToAddItem(string itemID)
    {
        int freeSlotIndex = FindFreeSlot(itemID);

        // -2 Is the 'Add Duplicate' State - maybe find more elegant fix

        if (freeSlotIndex != -1 && freeSlotIndex != -2 && hasFreeSlot)
        {
            TestItemAdd(itemID, freeSlotIndex);
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
