using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlManager : MonoBehaviour
{
    private const string _lowColorCode = "#FF5010";
    private const string _midColorCode = "#FF46FF";
    private const string _highColorCode = "#003CFF";

    //プレイヤーのドラゴンは何番か調べるのに必要
    [SerializeField] private GameManager _gameManager = default;

    [SerializeField] private GameObject[] _hyperdrives = default;
    [SerializeField] private GameObject[] _leftSparks = default;
    [SerializeField] private GameObject[] _rightSparks = default;
    [SerializeField] private GameObject[] _driftBoosts = default;
    [SerializeField] private ParticleSystem[] _leftParticals = default;
    [SerializeField] private ParticleSystem[] _rightParticals = default;
    [SerializeField] private ParticleSystem[] _airDashParticals = default;

    private ParticleSystem.MainModule _leftMainModule = default;
    private ParticleSystem.MainModule _rightMainModule = default;
    private ParticleSystem.MainModule _twoPlayerLeftMainModule = default;
    private ParticleSystem.MainModule _twoPlayerRightMainModule = default;

    private void Start()
    {
        _leftMainModule = _leftParticals[_gameManager.SelectDragon].main;
        _rightMainModule = _rightParticals[_gameManager.SelectDragon].main;
        if(_gameManager._twoPlayerSelectDragon >= 0)
        {
            _twoPlayerLeftMainModule = _leftParticals[_gameManager._twoPlayerSelectDragon].main;
            _twoPlayerRightMainModule = _rightParticals[_gameManager._twoPlayerSelectDragon].main;
        }
    }

    //加速エフェクト　引数にプレイヤーナンバーもしくはドラゴンナンバーを入れる
    //プレイヤーナンバーを入れたなら２つ目の引数はtrue、ドラゴンナンバーならfalse
    //バグ修正の応急処置
    public void StartHyperDrive(int playerNumber, bool usePlayerNumber)
    {
        // コルーチンの起動
        StartCoroutine(DriveCoroutin(playerNumber ,usePlayerNumber));
    }
    
    // コルーチン本体
    private IEnumerator DriveCoroutin(int playerNumber,bool usePlayerNumber)
    {
        int dragonNumber = -1;
        if (usePlayerNumber)
        {
            if (playerNumber == 0)
            {
                dragonNumber = _gameManager.SelectDragon;
            }
            else if (playerNumber == 1)
            {
                dragonNumber = _gameManager._twoPlayerSelectDragon;
            }
            else
            {
                Debug.LogError("無効なプレイヤー番号");
            }
        }
        else
        {
            dragonNumber = playerNumber;
        }

        //Partic起動
        _hyperdrives[dragonNumber].SetActive(true);

        //一定時間待つ
        yield return new WaitForSeconds(1.2f);

        //一定時間後に停止
        _hyperdrives[dragonNumber].SetActive(false);
    }

    #region スパークをOn、Offする
    //1P用
    public void PlayerOneSparksOn()
    {
        _leftSparks[_gameManager.SelectDragon].SetActive(true);
        _rightSparks[_gameManager.SelectDragon].SetActive(true);
    }
    public void PlayerOneSparksOff()
    {
        _leftSparks[_gameManager.SelectDragon].SetActive(false);
        _rightSparks[_gameManager.SelectDragon].SetActive(false);
    }
    //2P用
    public void PlayerTwoSparksOn()
    {
        _leftSparks[_gameManager._twoPlayerSelectDragon].SetActive(true);
        _rightSparks[_gameManager._twoPlayerSelectDragon].SetActive(true);
    }
    public void PlayerTwoSparksOff()
    {
        _leftSparks[_gameManager._twoPlayerSelectDragon].SetActive(false);
        _rightSparks[_gameManager._twoPlayerSelectDragon].SetActive(false);
    }
    #endregion

    #region 時間で段階的に色が変わるメソッド
    //1Player用
    public void OneStageSpark()
    {
        ColorUtility.TryParseHtmlString(_lowColorCode, out Color blue);
        _leftMainModule.startColor = blue;
        _rightMainModule.startColor = blue;
    }

    public void TwoStageSpark()
    {
        ColorUtility.TryParseHtmlString(_midColorCode, out Color red);
        _leftMainModule.startColor = red;
        _rightMainModule.startColor = red;
    }

    public void ThreeStageSpark()
    {
        ColorUtility.TryParseHtmlString(_highColorCode, out Color purple);
        _leftMainModule.startColor = purple;
        _rightMainModule.startColor = purple;
    }

    //2Player用
    public void TwoPlayerOneStageSpark()
    {
        ColorUtility.TryParseHtmlString(_lowColorCode, out Color blue);
        _twoPlayerLeftMainModule.startColor = blue;
        _twoPlayerRightMainModule.startColor = blue;
    }

    public void TwpPlayerTwoStageSpark()
    {
        ColorUtility.TryParseHtmlString(_midColorCode, out Color red);
        _twoPlayerLeftMainModule.startColor = red;
        _twoPlayerRightMainModule.startColor = red;
    }

    public void TwoPlayerThreeStageSpark()
    {
        ColorUtility.TryParseHtmlString(_highColorCode, out Color purple);
        _twoPlayerLeftMainModule.startColor = purple;
        _twoPlayerRightMainModule.startColor = purple;
    }
    #endregion

    public void AirDashSpark()
    {
        _airDashParticals[_gameManager.SelectDragon].Play();
    }
    public void TwoPlayerAirDashSpark()
    {
        _airDashParticals[_gameManager._twoPlayerSelectDragon].Play();
    }

    public void DriftBoost()
    {
        print("DriftBoost");
        //コルーチンの起動
        StartCoroutine(BoostCoroutin());
    }
    private IEnumerator BoostCoroutin()
    {
        //Partic起動
        _driftBoosts[_gameManager.SelectDragon].SetActive(true);

        //一定時間待つ
        yield return new WaitForSeconds(0.7f);

        //一定時間後に停止
        _driftBoosts[_gameManager.SelectDragon].SetActive(false);
    }

    //2Player用
    public void TwoPlayerDriftBoost()
    {
        // コルーチンの起動
        StartCoroutine(TwoPlayerBoostCoroutin());
    }

    private IEnumerator TwoPlayerBoostCoroutin()
    {
        //Partic起動
        _driftBoosts[_gameManager._twoPlayerSelectDragon].SetActive(true);

        //一定時間待つ
        yield return new WaitForSeconds(0.7f);

        //一定時間後に停止
        _driftBoosts[_gameManager._twoPlayerSelectDragon].SetActive(false);
    }
}
