using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : ScriptableObject
{
    public int ItemIndex => _itemIndex;
    [SerializeField] private int _itemIndex;
    public abstract void Use(GameObject user, PlayerItem.ItemUseType itemUseType);
}
