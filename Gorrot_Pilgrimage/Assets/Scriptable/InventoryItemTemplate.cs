using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItemTemplate", menuName = "Scriptable Objects/InventoryItemTemplate")]
public class InventoryItemTemplate : ScriptableObject
{
    public string itemID;
    public string itemName;
    public Sprite itemImage;
    public int amountHeld;

}
