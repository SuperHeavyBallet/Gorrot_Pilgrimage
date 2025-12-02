using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotController : MonoBehaviour
{

    public TextMeshProUGUI itemText;
    public TextMeshProUGUI itemQuantityText;
    public GameObject itemQuantityGO;
    public int itemQuantity = 1;

    bool slotIsEmpty = true;

    public Button useItemButton;

    PlayerStatsController playerStatsController;

    public string itemID = "Empty";





    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStatsController = GameObject.Find("Player").GetComponent<PlayerStatsController>();
        UpdateItemID("Empty");

  

    }

    public void UseItemClick()
    {
        if(itemQuantity >= 1)
        {
            playerStatsController.UseItem(itemID);

        }

        itemQuantity -= 1;
        UpdateItemQuantityText();

        if (itemQuantity < 1)
        {
            RemoveItemFromSlot();
        }

    }

    public void UpdateItemText(string newItemID)
    {
        string itemName = ItemCatalogue.Instance.GetItemName(newItemID);
        itemText.text = itemName;
    }

    public void UpdateItemID(string newItemID)
    {
        itemID = newItemID;
    }

    public void PlaceItemInSlot(string itemID)
    {
        slotIsEmpty = false;
        itemQuantity += 1;
        itemQuantityText.text = itemQuantity.ToString();
        UpdateItemID(itemID);
        UpdateItemText(itemID);
    }

    public string GetCurrentItemID()
    {
        return itemID;
    }

    public void PlaceDuplcateItemInSlot()
    {
        itemQuantity += 1;
        UpdateItemQuantityText();
       
    }

    void UpdateItemQuantityText()
    {
        itemQuantityText.text = itemQuantity.ToString();
    }

    public void RemoveItemFromSlot()
    {
        slotIsEmpty = true;


        UpdateItemText("...");
        UpdateItemID("Empty");
    }

    public bool CheckSlotEmpty()
    {
        return slotIsEmpty;
    }
}
