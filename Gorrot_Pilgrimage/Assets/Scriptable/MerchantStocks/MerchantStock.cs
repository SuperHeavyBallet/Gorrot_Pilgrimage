using UnityEngine;

[CreateAssetMenu(fileName = "MerchantStock", menuName = "Scriptable Objects/MerchantStock")]
public class MerchantStock : ScriptableObject
{

   [SerializeField] InventoryItemTemplate[] merchantInventory;

    public InventoryItemTemplate[] GetInventory()
    {

        return merchantInventory;

    }
    
}
