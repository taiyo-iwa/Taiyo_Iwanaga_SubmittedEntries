using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEventManager : MonoBehaviour
{
    public static TutorialEventManager Instance { get; private set; }

    public enum EventType
    {
        DriftEvent,
        DashEvent,    
        ItemBoxEvent,
        ItemCrashEvent,
        FrameEvent,
        ShieldEvent,
        FretherEvent,
        SoarEvent,
    }

    [SerializeField] TutorialManager _tutorialManager;

    #region 一度きり用のフラグ
    //最初の一回かぎりなのでTureにしておく
    private bool _isDriftEventTrigger = true;
    private bool _isDashEventTrigger = true;
    private bool _isItemBoxEventTrigger = true;
    private bool _isCrashBoxEventTrigger = true;
    private bool _isFrameEventTrigger = true;
    private bool _isShieldEventTrigger = true;
    private bool _isFretherEventTrigger = true;
    private bool _isSoarEventTrigger = true;
    private bool _tutorialCompleteTrigger = true;
    private bool _isCoolTimeEventTrigger = true;
    #endregion

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

    public void ActivateEvent(EventType type)
    {
        switch (type)
        {
            case EventType.DashEvent:
                if (_isDriftEventTrigger)
                {
                    _isDriftEventTrigger = false;
                    _tutorialManager._dashEventExplanation = true;
                }             
                break;
            case EventType.DriftEvent:
                if (_isDashEventTrigger)
                {
                    _isDashEventTrigger = false;
                    _tutorialManager._driftExplanation = true;
                }
                break;

            case EventType.ItemBoxEvent:
                if (_isItemBoxEventTrigger)
                {
                    _isItemBoxEventTrigger = false;
                    _tutorialManager._itemBoxExplanation = true;
                }
                break;

            case EventType.ItemCrashEvent:
                if (_isCrashBoxEventTrigger)
                {
                    _isCrashBoxEventTrigger = false;
                    _tutorialManager._crashBoxExplanation = true;
                }               
                break;

            case EventType.FrameEvent:
                if (_isFrameEventTrigger)
                {
                    _isFrameEventTrigger = false;
                    _tutorialManager._frameExplanation = true;
                }           
                break;

            case EventType.ShieldEvent:
                if (_isShieldEventTrigger)
                {
                    _isShieldEventTrigger = false;
                    _tutorialManager._shieldExplanation = true;
                }
                break;

            case EventType.FretherEvent:
                if (_isFretherEventTrigger)
                {
                    _isFretherEventTrigger = false;
                    _tutorialManager._fretherExplanation = true;
                }                
                break;

            case EventType.SoarEvent:
                if (_isSoarEventTrigger)
                {
                    _isSoarEventTrigger = false;
                    _tutorialManager._soarExplanation = true;
                }              
                break;
        }
    }
}
