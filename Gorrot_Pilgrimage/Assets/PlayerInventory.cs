using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

  

    public GameObject[] inventorySlots = new GameObject[4];

    bool hasFreeSlot = true;

    List<InventoryItemTemplate> allItemsList = new List<InventoryItemTemplate>();

    public ItemCatalogue itemCatalogue;

    public void BubbleSortEmptySlotsToEnd()
    {

        foreach(InventoryItemTemplate item in allItemsList)
        {
            Debug.Log(item.itemID + " , " + item.amountHeld);
        }

    }


    private void Start()
    {
        BuildItemsList();
    }

    void AddNewItem(string itemID, int slotIndex)
    {
       InventorySlotController inventorySlotController = inventorySlots[slotIndex].GetComponent<InventorySlotController>();


        foreach (InventoryItemTemplate item in allItemsList)
        {
            if (item.itemID == itemID)
            {
                inventorySlotController.PlaceItemInSlot(itemID, item.itemImage);
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


        // Here Point of Entry
        // Find a Free Slot, the items slots *should* be sorted before this (after previous update loop), but maybe pre-sort them to make sure too

        int freeSlotIndex = FindFreeSlot(itemID);

        // First, check if the Inventory already has this item:


        /*

       for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlotController inventorySlotController = GetInventorySlotController(i);

            Debug.Log("Current item in slot: " + i + " : " + inventorySlotController.GetCurrentItemID());

            if(inventorySlotController.GetCurrentItemID() == itemID)
            {

                Debug.Log("Item Already Held");
                break;
            }


        }*/

        // -2 Is the 'Add Duplicate' State - maybe find more elegant fix

        if (freeSlotIndex != -1 && freeSlotIndex != -2 && hasFreeSlot)
        {
            AddNewItem(itemID, freeSlotIndex);
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


    int FindFreeSlot(string newItemID)
    {

        hasFreeSlot = false;

        int freeSlotIndex = -1;

        for (int i = 0;i < inventorySlots.Length;i++)
        {
            InventorySlotController inventorySlotController = inventorySlots[i].GetComponent<InventorySlotController>();

            string heldItemID = inventorySlotController.GetCurrentItemID();

            Debug.Log("ITEM ID: " + newItemID + ", IN SLOT OF " + heldItemID);

            // IF the existing itemID (Including Empty) does not match the new itemID, e.g. 'potion' vs 'empty' > Check if that slot IS Empty, if so, break the loop and return that slot index
            if(heldItemID != newItemID)
            {
                if (inventorySlotController.CheckSlotEmpty())
                {
                    hasFreeSlot = true;
                    freeSlotIndex = i;
                    break;
                }
            }
            // Else, if the existing itemID DOES match the new itemID > it is already held > Add a duplicate (increment quantity) and return -2 instead of an actual slot index
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
