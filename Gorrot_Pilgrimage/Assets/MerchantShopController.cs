using System.Collections.Generic;
using UnityEngine;

public class MerchantShopController : MonoBehaviour
{
    public InventoryItemTemplate[] Items;

    [SerializeField] ItemSlotController itemSlot1;
    [SerializeField] ItemSlotController itemSlot2;
    [SerializeField] ItemSlotController itemSlot3;
    [SerializeField] ItemSlotController itemSlot4;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Choose4Items();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Choose4Items()
    {
        if (Items.Length < 4)
        {
            Debug.LogError("Not enough items to choose from.");
            return;
        }

        // Copy items into a temporary list
        List<InventoryItemTemplate> pool = new List<InventoryItemTemplate>(Items);

        // Fisher–Yates shuffle
        for (int i = pool.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (pool[i], pool[j]) = (pool[j], pool[i]);
        }

        // Take first 4
        itemSlot1.SetItem(pool[0]);
        itemSlot2.SetItem(pool[1]);
        itemSlot3.SetItem(pool[2]);
        itemSlot4.SetItem(pool[3]);
    }


}
