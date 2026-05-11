using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TutorialdashGauge : MonoBehaviour
{
    //プレイヤー番号を調べ、該当のフェザーゲージを調べるために使う
    [SerializeField] TutorialGameManager _gameManager;
    [SerializeField] TutorialPlayerController[] _playerControllers;

    //内側のゲージ
    [SerializeField] Image[] gauges;

    private float _maxGauge;        /* MAXのゲージ */
    private float _twoPlayerMaxGauge;
    private float _currentGauge;    /* 現在のゲージ量 */
    private float _twoPlayerCurrentGauge;
    private bool canFly;

    public float Gauge => _currentGauge;

    public bool CanFly => canFly;

    private void Start()
    {
        _maxGauge = _playerControllers[_gameManager._selectDragon].DashCoolDown;
        if (_gameManager._twoPlayerSelectDragon >= 0)
            _twoPlayerMaxGauge = _playerControllers[_gameManager._twoPlayerSelectDragon].DashCoolDown;
    }

    private void Update()
    {
        _currentGauge = _playerControllers[_gameManager._selectDragon].DashCooldownTimer;
        UpdateGauge(1 - _currentGauge / _maxGauge);
        if (_gameManager._twoPlayerSelectDragon >= 0)
        {
            _twoPlayerCurrentGauge = _playerControllers[_gameManager._twoPlayerSelectDragon].DashCooldownTimer;
            UpdateTwoPlayerGauge(1 - _twoPlayerCurrentGauge / _twoPlayerMaxGauge);
        }
    }

    private void UpdateGauge(float value)
    {
        if (gauges[0] != null)
        {
            gauges[0].fillAmount = value;
        }
    }

    private void UpdateTwoPlayerGauge(float value)
    {
        if (gauges[1] != null)
        {
            gauges[1].fillAmount = value;
        }
    }
}
