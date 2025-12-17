using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotController : MonoBehaviour
{
    
    string itemName;
    int itemPrice;

    public TextMeshProUGUI itemNameDisplay;
    public TextMeshProUGUI itemPriceDisplay;
    public Image itemSprite;

    GameObject player;
    PlayerInventory playerInventory;
    PlayerStatsController playerStatsController;
    InventoryItemTemplate thisItem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player");
        if(player != null )
        {
            playerInventory = player.GetComponent<PlayerInventory>();
            playerStatsController = player.GetComponent<PlayerStatsController>();
        }
        else
        {
            Debug.LogError("Player not found by Merchant Item Slot, " + this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetItem(InventoryItemTemplate item)
    {
        thisItem = item;

        itemName = item.itemName;
        itemPrice = item.price;

        itemNameDisplay.text = item.name;
        itemPriceDisplay.text = itemPrice.ToString();
        itemSprite.sprite = item.itemImage;

    }

    public void TryAndBuyItem()
    {
        Debug.Log("Try and Buy: " + itemName + " for " + itemPrice);

        bool playerHasEmptySlot = false;

        
        if (playerInventory != null && playerStatsController != null)
        {
            int playerTotalMoney = playerStatsController.GetPlayerCurrentMoney();

            if(playerTotalMoney >= itemPrice)
            {
                if (playerInventory.TryToAddItem(thisItem.itemID))
                {
                    playerStatsController.AlterMoney(-itemPrice);
                }

            }
            else
            {
                Debug.Log("NOT ENOUGH MONEY TO BUY: " + itemName);
            }

            
        }
    }
}
