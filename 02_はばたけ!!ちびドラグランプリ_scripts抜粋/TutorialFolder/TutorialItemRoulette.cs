using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialItemRoulette : MonoBehaviour
{
    [SerializeField] Sprite[] _itemSprite;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] TutorialSoundController _soundController;

    Image _thisImage;
    Animator _thisAnimator;

    public bool _rouletteFlag = false;

    public int _item = 1;
    private int _currentItem = 0;
    private float _scrollTime = 1.0f;
    private float _currentScrollTime = 0;

    /* èâä˙ê›íË */
    private void Start()
    {
        _thisAnimator = GetComponent<Animator>();
        _thisImage = GetComponent<Image>();

        _thisImage.sprite = _itemSprite[0];
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            UseItemUI();
        }

        /* âÒì]éûä‘ÇÃÉJÉEÉìÉg */
        if (_rouletteFlag)
        {
            _currentScrollTime += Time.deltaTime;
        }
    }
    public void RouletteItemUI()
    {
        _rouletteFlag = !_rouletteFlag;
        _thisAnimator.SetBool("roulette", _rouletteFlag);
    }


    public void ChangeItem()
    {
        _currentItem++;

        if (_itemSprite.Length > _currentItem)
        {
            _thisImage.sprite = _itemSprite[_currentItem];
        }
        else
        {
            _currentItem = 1;
            _thisImage.sprite = _itemSprite[_currentItem];
        }

        /* é~Ç‹Ç¡ÇƒÇŸÇµÇ¢SpriteÇ∆ìØÇ∂ */          /* âÒì]ÇµÇƒÇ¢ÇÈéûä‘Ç™âÒì]ê›íËéûä‘à»è„Ç…Ç»Ç¡ÇΩÇÁ */
        if (_thisImage.sprite == _itemSprite[_item] && _currentScrollTime >= _scrollTime)
        {
            _rouletteFlag = !_rouletteFlag;
            _thisAnimator.SetBool("roulette", _rouletteFlag);
            _currentScrollTime = 0.0f;
            _soundController.StopItemRouletteSound();
            _audioSource.Play();
        }
    }

    public void UseItemUI()
    {
        _currentItem = 0;
        _thisImage.sprite = _itemSprite[_currentItem];
    }
}
