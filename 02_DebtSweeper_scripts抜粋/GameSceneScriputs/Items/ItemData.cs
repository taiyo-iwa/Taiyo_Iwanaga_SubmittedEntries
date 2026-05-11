using UnityEngine;

public enum ItemType
{
    Weapon,
    HealingConsumable,
    Currency,
    StatBoostConsumable,
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemType itemType;
    public float price;
}
