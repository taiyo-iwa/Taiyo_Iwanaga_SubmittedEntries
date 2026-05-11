using UnityEngine;
using UniRx;

public class CPUEffectController : MonoBehaviour
{
    [SerializeField] private CPUStatus _cpuStatus = default;
    [SerializeField] private ParticleSystem[] _chargeParticle = default;
    [SerializeField] private ParticleSystem[] _afterChageParticle = default;

    private bool _previousSouthButtonState = false;
    private bool _currentSouthButtonState = false;

    public void StartCPUEffectController()
    {
        _cpuStatus.OnStartChargeDash
        .Subscribe(_ => { OnChargeDashStarted(); })
        .AddTo(this);

        foreach (ParticleSystem chargeParticle in _chargeParticle)
        {
            chargeParticle.Stop();
        }
        foreach (ParticleSystem afterChageParticle in _afterChageParticle)
        {
            afterChageParticle.Stop();
        }
    }

    public void UpdateCPUEffectController()
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
                foreach (ParticleSystem chargeParticle in _chargeParticle)
                {
                    chargeParticle.Play();
                }
            }
        }
        else
        {
            //先程までボタンを押していた時
            if (_currentSouthButtonState != _previousSouthButtonState)
            {
                foreach (ParticleSystem chargeParticle in _chargeParticle)
                {
                    chargeParticle.Stop();
                }
            }
        }

        _previousSouthButtonState = _cpuStatus.IsSouthButton;
    }

    //チャージダッシュした時の処理
    private void OnChargeDashStarted()
    {
        foreach (ParticleSystem afterChageParticle in _afterChageParticle)
        {
            afterChageParticle.Play();
        }
    }
}
