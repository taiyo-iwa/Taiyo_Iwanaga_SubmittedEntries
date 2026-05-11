using UnityEngine;
using UniRx;

public class CPUAudioController : MonoBehaviour
{
    [SerializeField] private CPUStatus _cpuStatus = default;
    [SerializeField] private AudioSource _audioSource = default;
    [SerializeField] private AudioClip _inChargeSE = default;
    [SerializeField] private AudioClip _chargeDashSE = default;

    private bool _previousSouthButtonState = false;
    private bool _currentSouthButtonState = false;

    public void StartCPUAudioController()
    {
        _cpuStatus.OnStartChargeDash
        .Subscribe(_ => { OnChargeDashStarted(); })
        .AddTo(this);
    }

    public void UpdateCPUAudioController()
    {
        ChargeEffectControl();
    }

    private void ChargeEffectControl()
    {
        _currentSouthButtonState = _cpuStatus.IsSouthButton;

        if (_cpuStatus.IsSouthButton)
        {
            //先程までボタンを離していた時
            if (_currentSouthButtonState != _previousSouthButtonState)
            {
                _audioSource.clip = _inChargeSE;
                //_audioSource.Play();
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

        _previousSouthButtonState = _cpuStatus.IsSouthButton;
    }

    //チャージダッシュした時の処理
    private void OnChargeDashStarted()
    {
        _audioSource.clip = _chargeDashSE;
        //_audioSource.Play();
    }
}
