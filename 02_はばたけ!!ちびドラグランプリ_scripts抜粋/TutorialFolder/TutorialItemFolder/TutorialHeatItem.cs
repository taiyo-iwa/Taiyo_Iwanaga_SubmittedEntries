using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TutorialItems/HeatItem")]
public class TutorialHeatItem : TutorialItemBase
{
    [SerializeField] private GameObject shieldPrefab;
    public override void Use(GameObject player, TutorialPlayerItem.ItemUseType itemType)
    {
        TutorialPlayerHeat HeatManager = player.GetComponent<TutorialPlayerHeat>();
        if (HeatManager != null)
        {
            HeatManager.ActivateHeat(player.transform, shieldPrefab, itemType);
        }
    }
}
