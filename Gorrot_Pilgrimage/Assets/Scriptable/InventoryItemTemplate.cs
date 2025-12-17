using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItemTemplate", menuName = "Scriptable Objects/InventoryItemTemplate")]
public class InventoryItemTemplate : ScriptableObject
{
    public string itemID;
    public string itemName;
    public Sprite itemImage;
    public int amountHeld;

    public enum statsToEffect { health, suffering, attack};
    public statsToEffect statEffected = statsToEffect.health;
    public int effectDelta = 1; // +1, -1 to adjust stat

    public int price;

    public int GetItemPrice()
    {
        return price;
    }

}
