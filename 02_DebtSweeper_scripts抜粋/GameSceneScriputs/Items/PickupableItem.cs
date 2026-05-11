using UnityEngine;

public class PickupableItem : MonoBehaviour
{
    [SerializeField] private ItemData itemData;

    public void OnPickUp()
    {
        Debug.Log("Picked up: " + itemData.itemName + "  price: " + itemData.price);
    }

    public float Redemption()
    {
        return itemData.price;
    }

    public bool IsRedemptionItem()
    {
        if(itemData.itemType == ItemType.Currency)
        {
            return true;
        }
        else
        {
            return false;
        }      
    }
}
