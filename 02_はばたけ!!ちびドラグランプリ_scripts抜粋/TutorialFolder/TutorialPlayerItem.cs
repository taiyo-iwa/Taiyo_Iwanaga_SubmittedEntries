using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPlayerItem : MonoBehaviour
{
    [SerializeField] private TutorialItemRoulette _itemRoulette;
    [SerializeField] private ItemRouletteScript _twoPlayerItemRoulette;
    [SerializeField] private Image _itemSlotImage;
    [SerializeField] private Image _twoPlayerItemSlotImage;
    [SerializeField] private TutorialSoundController _soundController = default;

    public int _playerNumber { get; set; } = 0;
    public TutorialItemBase CurrentItem { get; private set; }
    public bool _isPlayer { get; set; } = false;
    public bool IsWaitingForItem => _rouletteCoroutine != null;

    private Coroutine _rouletteCoroutine;

    public enum ItemUseType
    {
        Attack,
        Defense,
    }

    public void StartItemRoulette(List<TutorialItemBase> itemPool, float duration = 0.5f)
    {
        if (_rouletteCoroutine != null || !_isPlayer || CurrentItem != null) return;
        _rouletteCoroutine = StartCoroutine(RouletteCoroutine(itemPool, duration));
        _soundController.PlayItemRouletteSound();
    }

    private IEnumerator RouletteCoroutine(List<TutorialItemBase> itemPool, float duration)
    {
        float timer = 0f;
        if (_playerNumber == 0)
        {
            _itemRoulette.RouletteItemUI();
        }
        if (_playerNumber == 1)
        {
            _twoPlayerItemRoulette.RouletteItemUI();
        }

        while (timer < duration)
        {
            CurrentItem = itemPool[Random.Range(0, itemPool.Count)];
            timer += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        _rouletteCoroutine = null;
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
