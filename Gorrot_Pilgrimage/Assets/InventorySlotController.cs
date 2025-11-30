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



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStatsController = GameObject.Find("Player").GetComponent<PlayerStatsController>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UseItemClick()
    {
        Debug.Log("CLICKED USE ITEM!: " + itemText.text);
        

        if(itemQuantity >= 1)
        {
            playerStatsController.UseItem(itemText.text);
           

        }

        itemQuantity -= 1;
        UpdateItemQuantityText();

        if (itemQuantity < 1)
        {
            RemoveItemFromSlot();
        }

    }

    public void UpdateItemText(string newItemText)
    {
        itemText.text = newItemText;
    }

    public void PlaceItemInSlot(string newItem)
    {
        slotIsEmpty = false;
        itemQuantity += 1;
        itemQuantityText.text = itemQuantity.ToString();
        UpdateItemText(newItem);
    }

    public string GetCurrentItemName()
    {
        return itemText.text;
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


        UpdateItemText("Empty");
    }

    public bool CheckSlotEmpty()
    {
        return slotIsEmpty;
    }
}
