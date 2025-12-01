using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class ItemCatalogue : MonoBehaviour
{
    public InventoryItemTemplate[] allItems;

    public List<InventoryItemTemplate> allItemsList = new List<InventoryItemTemplate>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ListAllItems();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ListAllItems()
    {
      
    }

    public List<InventoryItemTemplate> GetAllItems()
    {
        return allItemsList;
    }
}
