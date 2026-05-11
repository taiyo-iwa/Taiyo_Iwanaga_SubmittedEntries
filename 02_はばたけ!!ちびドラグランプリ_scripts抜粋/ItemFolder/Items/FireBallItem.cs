using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/FireBall")]
public class FireBallItem : ItemBase
{
    public override void Use(GameObject player, PlayerItem.ItemUseType ItemType)
    {
        var FireballManager = player.GetComponent<PlayerFireBall>();
        if (FireballManager != null)
        {
            FireballManager.ActivateFireBall(player.transform);
        }
    }
}
