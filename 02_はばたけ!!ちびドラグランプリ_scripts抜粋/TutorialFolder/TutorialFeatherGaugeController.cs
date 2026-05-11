using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialFeatherGaugeController : MonoBehaviour
{
    //プレイヤー番号を調べ、該当のフェザーゲージを調べるために使う
    [SerializeField] TutorialGameManager _gameManager = default;
    [SerializeField] PlayerFlightGauge[] playerFlightGauges;

    //内側のゲージ
    [SerializeField] Image[] gauges;

    private float _maxGauge;        /* MAXのゲージ */
    private float _currentGauge;    /* 現在のゲージ量 */
    private float _twoPlayerMaxGauge;        /* MAXのゲージ */
    private float _twoPlayerCurrentGauge;    /* 現在のゲージ量 */
    private bool canFly;

    public float Gauge => _currentGauge;

    public bool CanFly => canFly;
    public bool twoPlayerCanFly { get; private set; } = false;

    private void Start()
    {
        _maxGauge = playerFlightGauges[_gameManager._selectDragon].Max;
        if (_gameManager._twoPlayerSelectDragon >= 0)
            _twoPlayerMaxGauge = playerFlightGauges[_gameManager._twoPlayerSelectDragon].Max;
    }

    private void Update()
    {
        _currentGauge = playerFlightGauges[_gameManager._selectDragon].Current;
        if (_gameManager._twoPlayerSelectDragon >= 0)
            _twoPlayerCurrentGauge = playerFlightGauges[_gameManager._twoPlayerSelectDragon].Current;
        SpriteColor();
    }

    /* ゲージの色の変化 */
    private void SpriteColor()
    {
        float gaugeValue = 0;
        if (playerFlightGauges[_gameManager._selectDragon].IsFlying)
        {
            canFly = true;
            for (int i = 0; i <= 2; i++)
            {
                if (i + 1 <= playerFlightGauges[_gameManager._selectDragon].UseNum)
                {
                    gaugeValue = Mathf.Clamp01(((_currentGauge - playerFlightGauges[_gameManager._selectDragon].UsedGauge) - _maxGauge / 3 * i) / (_maxGauge / 3));
                    UpdateGauge(i, gaugeValue, true);
                }
                else if (i == playerFlightGauges[_gameManager._selectDragon].UseNum)
                {
                    gaugeValue = Mathf.Clamp01(playerFlightGauges[_gameManager._selectDragon].UsedGauge / (_maxGauge / 3));
                    UpdateGauge(i, gaugeValue, false);
                }
                else
                {
                    gaugeValue = 0;
                    UpdateGauge(i, gaugeValue, false);
                }
            }
            return;
        }
        for (int i = 0; i <= 2; i++)
        {
            gaugeValue = Mathf.Clamp01((_currentGauge - _maxGauge / 3 * i) / (_maxGauge / 3));
            if (i == 0)
            {
                if (gaugeValue >= 1)
                {
                    canFly = true;
                }
                else
                {
                    canFly = false;
                }
            }
            UpdateGauge(i, gaugeValue, false);
        }
    }

    /* 表示の更新 */
    private void UpdateGauge(int index, float value, bool useGauge)
    {
        if (gauges[index] != null)
        {
            gauges[index].fillAmount = value;
            if (useGauge)
            {
                gauges[index].color = Color.cyan;
                return;
            }
            if (value < 0.5f)
            {
                gauges[index].color = Color.Lerp(Color.red, Color.yellow, value / 0.5f);
            }
            else if (value < 1f)
            {
                value -= 0.5f;
                gauges[index].color = Color.Lerp(Color.yellow, Color.green, value / 0.5f);
            }
            else
            {
                gauges[index].color = Color.cyan;
            }
        }
    }
}
