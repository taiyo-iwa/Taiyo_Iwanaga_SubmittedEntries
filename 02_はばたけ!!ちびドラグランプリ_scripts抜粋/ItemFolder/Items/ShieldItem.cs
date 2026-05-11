using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/ShieldItem")]
public class ShieldItem : ItemBase
{
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private GameObject FlamePrefab;
    [SerializeField] private float duration = 5f;

    public override void Use(GameObject player, PlayerItem.ItemUseType ItemType)
    {
        var shieldManager = player.GetComponent<PlayerShield>();
        if (shieldManager != null)
        {
            shieldManager.ActivateShield(shieldPrefab, FlamePrefab, duration);
        }
    }
}
