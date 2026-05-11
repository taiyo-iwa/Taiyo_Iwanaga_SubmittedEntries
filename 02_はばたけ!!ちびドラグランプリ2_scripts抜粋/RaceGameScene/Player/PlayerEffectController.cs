using UnityEngine;
using UniRx;

public class PlayerEffectController : MonoBehaviour
{
    [SerializeField] private PlayerStatus _playerStatus = default;
    [SerializeField] private ParticleSystem[] _chargeParticle = default;
    [SerializeField] private ParticleSystem[] _afterChageParticle = default;

    private bool _previousSouthButtonState = false;
    private bool _currentSouthButtonState = false;

    public void StartPlayerEffectController()
    {
        _playerStatus.OnStartChargeDash
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

    public void UpdatePlayerEffectController()
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

        _previousSouthButtonState = _playerStatus.IsSouthButton;
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
