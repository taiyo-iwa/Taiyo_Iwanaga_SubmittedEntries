using UnityEngine;
using UniRx;

public class PlayerAudioController : MonoBehaviour
{
    [SerializeField] private PlayerStatus _playerStatus = default;
    [SerializeField] private AudioSource _audioSource = default;
    [SerializeField] private AudioClip _inChargeSE = default;
    [SerializeField] private AudioClip _chargeDashSE = default;

    private bool _previousSouthButtonState = false;
    private bool _currentSouthButtonState = false;

    public void StartPlayerAudioController()
    {
        _playerStatus.OnStartChargeDash
        .Subscribe(_ => { OnChargeDashStarted(); })
        .AddTo(this);
    }

    public void UpdatePlayerAudioController()
    {
        ChargeEffectControl();
    }

    private void ChargeEffectControl()
    {
        _currentSouthButtonState = _playerStatus.IsSouthButton;

        if (_playerStatus.IsSouthButton)
        {
            //先程までボタンを離していた時
            if (_currentSouthButtonState != _previousSouthButtonState)
            {
                _audioSource.clip = _inChargeSE;
                _audioSource.Play();
            }
        }
        else
        {
            //先程までボタンを押していた時
            if (_currentSouthButtonState != _previousSouthButtonState)
            {
                //_audioSource.Stop();
            }
        }

        _previousSouthButtonState = _playerStatus.IsSouthButton;
    }

    //チャージダッシュした時の処理
    private void OnChargeDashStarted()
    {
        _audioSource.clip = _chargeDashSE;
        _audioSource.Play();
    }
}
