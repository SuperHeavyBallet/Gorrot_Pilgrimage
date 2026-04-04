using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using GorrotGame;

public class ItemCatalogue : MonoBehaviour
{

    public static ItemCatalogue Instance { get; private set; }

    public List<InventoryItemTemplate> allItemsList = new List<InventoryItemTemplate>();
    private Dictionary<string, InventoryItemTemplate> itemLookup = new Dictionary<string, InventoryItemTemplate>();

    [SerializeField] InventoryItemTemplate flowerItem;
    public InventoryItemTemplate GetFlowerItem() => flowerItem;

    public InventoryItemTemplate[] uniqueItems;

    [SerializeField] List<InventoryItemTemplate> forest_inventoryItems = new List<InventoryItemTemplate>();
    [SerializeField] List<InventoryItemTemplate> swamp_inventoryItems = new List<InventoryItemTemplate>();
    [SerializeField] List<InventoryItemTemplate> steppe_inventoryItems = new List<InventoryItemTemplate>();
    [SerializeField] List<InventoryItemTemplate> mountain_inventoryItems = new List<InventoryItemTemplate>();

    private void Awake()
    {

        // Singleton enforcement
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        itemLookup.Clear();

        foreach (var item in allItemsList)
        {
            if (!itemLookup.ContainsKey(item.itemID))
                itemLookup.Add(item.itemID, item);
            else
                Debug.LogWarning($"Duplicate itemID detected: {item.itemID}");
        }
    }

    public bool CheckItemIDInUniqueItems(string itemID)
    {
        for (int i = 0; i < uniqueItems.Length; i++)
        {
            if(uniqueItems[i].itemID == itemID) return true;
        }

        return false;

    }

    public List<InventoryItemTemplate> GetAllItems()
    {
        return allItemsList;
    }

    public List<InventoryItemTemplate> GetMapItems(MapData mapData)
    {
        MapBiomeType mapBiome = mapData.GetMapBiomeType;

        return mapBiome switch
        {
            MapBiomeType.Forest => forest_inventoryItems,
            MapBiomeType.Swamp => swamp_inventoryItems,
            MapBiomeType.Steppe => steppe_inventoryItems,
            MapBiomeType.Mountain => mountain_inventoryItems,
            _ => allItemsList

        };
    }

    public string GetItemName(string itemID)
    {
        if (itemLookup.TryGetValue(itemID, out var item))
            return item.itemName;

        return ""; 

    }

    public string GetItemStatEffected(string itemID)
    {
        if (itemLookup.TryGetValue(itemID, out var item))
        {
            return item.statEffected.ToString();
        }
        return "";
    }

    public int GetItemEffectDelta(string itemID)
    {
        if(itemLookup.TryGetValue(itemID,out var item))
        {
            return item.effectDelta;
        }

        return 0;
    }
}
