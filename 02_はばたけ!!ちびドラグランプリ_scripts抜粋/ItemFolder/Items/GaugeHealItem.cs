using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/GaugeHealItem")]
public class GaugeHealItem : ItemBase
{
    [SerializeField] private float _gaugeAmount = 2f;

    public override void Use(GameObject user, PlayerItem.ItemUseType ItemType)
    {
        if (user.TryGetComponent(out PlayerItemReceiver receiver))
        {
            receiver.RecoverGauge(_gaugeAmount);
        }
    }
}
