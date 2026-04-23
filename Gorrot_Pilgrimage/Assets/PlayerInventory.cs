using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

  

    public GameObject[] inventorySlots = new GameObject[4];
    List<GameObject> emptySlots = new List<GameObject>();
    GameObject[] emptySlotsArray = new GameObject[4];
    List<GameObject> takenSlots = new List<GameObject>();

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
                int amountToAdd = item.AmountAddedOnPickup;
                inventorySlotController.PlaceItemInSlot(itemID, item.itemImage, amountToAdd);
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
        InventorySlotController firstEmpty = null;
        int firstEmptyIndex = SlotNotFound;

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            var slot = inventorySlots[i].GetComponent<InventorySlotController>();
            string held = slot.CurrentItemID;

            if (!slot.SlotEmpty && held == newItemID)
                return new SlotSearchResult { SlotIndex = i, SlotController = slot };

            if (firstEmpty == null && slot.SlotEmpty)
            {
                firstEmpty = slot;
                firstEmptyIndex = i;
            }
        }

        if (firstEmpty != null)
            return new SlotSearchResult { SlotIndex = firstEmptyIndex, SlotController = firstEmpty };

        return new SlotSearchResult { SlotIndex = SlotNotFound, SlotController = null };
    }


    /*
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
    */

    void BuildItemsList()
    {
        allItemsList = itemCatalogue.GetAllItems();
    }
}
