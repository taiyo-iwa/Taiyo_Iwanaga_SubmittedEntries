using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/HeatItem")]
public class HeatItem : ItemBase
{
    [SerializeField] private GameObject shieldPrefab;
    public override void Use(GameObject player, PlayerItem.ItemUseType itemType)
    {
        var HeatManager = player.GetComponent<PlayerHeat>();
        if (HeatManager != null)
        {
            HeatManager.ActivateHeat(player.transform, shieldPrefab, itemType);
        }
    }
}
