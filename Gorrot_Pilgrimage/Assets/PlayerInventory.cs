using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

  

    public GameObject[] inventorySlots = new GameObject[4];

    List<InventoryItemTemplate> allItemsList = new List<InventoryItemTemplate>();

    public ItemCatalogue itemCatalogue;

    private const int SlotNotFound = -1;
    private const int DuplicateItem = -2;

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

    public bool TryToAddItem(string itemID)
    {

        var slotResult = CheckFreeSlot(itemID);

        if (slotResult.SlotController == null) return false;

        if (slotResult.SlotIndex == DuplicateItem)
        {
            if (slotResult.SlotController == null) return false;
            slotResult.SlotController.PlaceDuplcateItemInSlot();
            return true;
        }
        else if(slotResult.SlotIndex >= 0)
        {
            AddNewItem(itemID, slotResult.SlotIndex);
            return true;
        }
    
        return false;

    }

    struct SlotSearchResult
    {
        public int SlotIndex;
        public InventorySlotController SlotController;
    }


    SlotSearchResult CheckFreeSlot(string newItemID)
    {

        for (int i = 0;i < inventorySlots.Length;i++)
        {
            InventorySlotController inventorySlotController = inventorySlots[i].GetComponent<InventorySlotController>();
            string heldItemID = inventorySlotController.GetCurrentItemID();

            if(heldItemID != newItemID)
            {
                if (inventorySlotController.CheckSlotEmpty())
                {
                    return new SlotSearchResult { SlotIndex = i, SlotController = inventorySlotController };
                }
            }
            else
            {
                return new SlotSearchResult { SlotIndex = DuplicateItem, SlotController = inventorySlotController };
               
            }
        }

        return new SlotSearchResult { SlotIndex = SlotNotFound, SlotController = null };


    }


    void BuildItemsList()
    {
        allItemsList = itemCatalogue.GetAllItems();
    }
}
