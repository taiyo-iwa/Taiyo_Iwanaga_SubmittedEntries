using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItem : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager = default;
    [SerializeField] private pathRanking _rankingSys = default;
    [SerializeField] private ItemRouletteScript _itemRoulette;
    [SerializeField] private ItemRouletteScript _twoPlayerItemRoulette;
    [SerializeField] private Image _itemSlotImage;
    [SerializeField] private Image _twoPlayerItemSlotImage;
    [SerializeField] private PlayerSoundController _soundController = default;

    public int _playerNumber { get; set; } = -1;
    public ItemBase CurrentItem { get; private set; }
    public bool _isPlayer { get; set; } = false;

    public enum ItemUseType
    {
        Attack,
        Defense,
    }

    public void StartItemRoulette(List<ItemBase> itemPool, float duration = 0f)
    {
        if (!_isPlayer || CurrentItem != null) return;
        RouletteCoroutine(itemPool, duration);
        _soundController.PlayItemRouletteSound();
    }

    private void RouletteCoroutine(List<ItemBase> itemPool, float duration)
    {
        if (_playerNumber == 0)
        {
            _itemRoulette.RouletteItemUI();
        }
        if (_playerNumber == 1)
        {
            _twoPlayerItemRoulette.RouletteItemUI();
        }
        if (itemPool.Count == 2)
        {
            float rankingRatio = 0;
            if(_playerNumber == 0)
            {
                rankingRatio = ((float)_rankingSys.GetRanking(_gameManager.SelectDragon)) / 4;
            }
            else if(_playerNumber == 1)
            {
                rankingRatio = ((float)_rankingSys.GetRanking(_gameManager._twoPlayerSelectDragon)) / 4;
            }
            float itemRatio = (float)Random.Range(0, 100) / 100;
            if(itemRatio < rankingRatio)
            {
                CurrentItem = itemPool[0];
            }
            else
            {
                CurrentItem = itemPool[1];
            }
        }
        else
        {
            CurrentItem = itemPool[Random.Range(0, itemPool.Count)];
        }

        if (_playerNumber == 0)
        {
            if (_itemRoulette != null)
            {
                _itemRoulette._item = CurrentItem.ItemIndex;
                print(CurrentItem.ItemIndex);
            }
        }
        if (_playerNumber == 1)
        {
            if (_twoPlayerItemRoulette != null)
            {
                _twoPlayerItemRoulette._item = CurrentItem.ItemIndex;
                print(CurrentItem.ItemIndex);
            }
        }
        Debug.Log($"アイテム確定: {CurrentItem.name}");
    }
    private void Update()
    {
        UpdateSlotDisplay();
    }

    public void UseItem(ItemUseType useType)
    {
        if (_playerNumber == 0)
        {
            if (CurrentItem != null && _itemRoulette._rouletteFlag == false)
            {
                CurrentItem.Use(gameObject, useType);
                _itemRoulette.UseItemUI();
                CurrentItem = null;
            }
        }
        if (_playerNumber == 1)
        {
            if (CurrentItem != null && _twoPlayerItemRoulette._rouletteFlag == false)
            {
                CurrentItem.Use(gameObject, useType);
                _twoPlayerItemRoulette.UseItemUI();
                CurrentItem = null;
            }
        }
    }

    private void UpdateSlotDisplay()
    {
        if (_playerNumber == 0)
        {
            if (_itemSlotImage == null) return;

            _itemSlotImage.color = (CurrentItem != null && _itemRoulette._rouletteFlag == false)
                ? Color.white // 暗く見せる
                : new Color(0.5f, 0.5f, 0.5f, 1.0f);                     // 元の明るさに戻す
        }
        if (_playerNumber == 1)
        {
            if (_twoPlayerItemSlotImage == null) return;

            _twoPlayerItemSlotImage.color = (CurrentItem != null && _twoPlayerItemRoulette._rouletteFlag == false)
                ? Color.white // 暗く見せる
                : new Color(0.5f, 0.5f, 0.5f, 1.0f);                     // 元の明るさに戻す
        }
    }
}
