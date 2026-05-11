using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventProgressManager : MonoBehaviour
{
    public static EventProgressManager Instance { get; private set; }

    //ドラゴンのTransform
    [SerializeField] private Transform _dragonTransform = default;
    [SerializeField] private Transform _dragonModelTransform = default;

    [Header("各イベント位置")]
    //各イベントの初期位置に飛ばす場所のオブジェクト
    [SerializeField] private Transform _startTransform = default;
    [SerializeField] private Transform _driftTransform = default;
    [SerializeField] private Transform _dashTransform = default;   
    [SerializeField] private Transform _itemBoxTransform = default;
    //アイテムボックスを取れなかった時のアイテムボックスの手前の場所
    [SerializeField] private Transform _flameTransform = default;
    [SerializeField] private Transform _shieldTransform = default;
    [SerializeField] private Transform _fretherTransform = default;

    //各イベント進行フラグ
    private bool _driftEventFlag = false;
    private bool _dashEventFlag = false;
    private bool _itemEventFlag = false;
    private bool _itemCrashFlag = false;
    private bool _flemeEventFlag = false;
    private bool _shieldEventFlag = false;
    private bool _fretherEventFlag = false;

    //初回は移動させないためのTrigger
    private bool _driftFlagTriggr = false;
    private bool _dashEventTriggr = false;
    private bool _itemEventTriggr = false;
    private bool _itemCrashTriggr = false;
    private bool _flemeEventTriggr = false;
    private bool _shieldEventTriggr = false;
    private bool _fretherEventTriggr = false;

    public enum ProgressType
    {
        StartProgress,
        DriftProgress,
        DashProgress,   
        ItemBoxProgress,
        ItemCrashProgress,
        FrameProgress,
        ShieldProgress,
        FretherProgress,
        SoarProgress,
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ActivateProgress(ProgressType type)
    {
        switch (type)
        {
            case ProgressType.StartProgress:
                if (!_driftEventFlag)
                {
                    _dragonTransform.position = _startTransform.position;
                    _dragonTransform.rotation = _startTransform.rotation;
                }
                break;
            case ProgressType.DriftProgress:
                if (!_dashEventFlag)
                {
                    if (_dashEventTriggr)
                    {
                        _dragonTransform.position = _driftTransform.position;
                        _dragonTransform.rotation = _driftTransform.rotation;
                    }
                    else
                    {
                        _dashEventTriggr = true;
                    }
                }
                break;

            case ProgressType.DashProgress:
                if (!_itemEventFlag)
                {
                    if (_itemEventTriggr)
                    {
                        _dragonTransform.position = _dashTransform.position;
                        _dragonTransform.rotation = _dashTransform.rotation;
                    }
                    else
                    {
                        _itemEventTriggr = true;
                    }
                }
                break;

            case ProgressType.ItemBoxProgress:
                if (!_shieldEventFlag)
                {
                    if (_shieldEventTriggr)
                    {
                        _dragonTransform.position = _itemBoxTransform.position;
                        _dragonTransform.rotation = _itemBoxTransform.rotation;
                    }
                    else
                    {
                        _shieldEventTriggr = true;
                    }
                    
                }
                break;

            case ProgressType.ItemCrashProgress:
                if (!_flemeEventFlag)
                {
                    _dragonTransform.position = _flameTransform.position;
                    _dragonTransform.rotation = _flameTransform.rotation;
                }
                break;

            case ProgressType.FrameProgress:
                //ファイアが上手く当たらなかったとき
                break;
            case ProgressType.ShieldProgress:
                if (!_fretherEventFlag)
                {
                    if (_fretherEventTriggr)
                    {
                        _dragonTransform.position = _shieldTransform.position;
                        _dragonTransform.rotation = _shieldTransform.rotation;
                    }
                    else
                    {
                        _fretherEventTriggr = true;
                    }
                }
                break;
        }
    }

    public void EventProgress(bool driftEvent, bool dashEvent, bool itemEvent, bool flameEvent, bool shieldEvent, bool fretherEventFlag)
    {
        _driftEventFlag = driftEvent;
        _dashEventFlag = dashEvent;
        _itemEventFlag = itemEvent;
        _flemeEventFlag = flameEvent;
        _shieldEventFlag = shieldEvent;
        _fretherEventFlag = fretherEventFlag;
    }
}
