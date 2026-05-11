using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSoundController : MonoBehaviour
{
    [SerializeField] private TutorialGameManager _gameManager = default;

    [SerializeField] private AudioSource _runningAudio = default;
    [SerializeField] private AudioSource _driftAudio = default;
    [SerializeField] private AudioSource _flyAudio = default;
    [SerializeField] private AudioSource _dashAudio = default;
    [SerializeField] private AudioSource _fireballAudio = default;
    [SerializeField] private AudioSource _fireballWarningAudio = default;
    [SerializeField] private AudioSource _shieldAudio = default;
    [SerializeField] private AudioSource _itemRouletteAudio = default;
    [SerializeField] private AudioSource _wingChargeAudio = default;
    [SerializeField] private AudioSource _fireballHitAudio = default;


    private TutorialPlayerController _playerController = default;

    private bool _isRunning = false;
    private bool _isDriftingSound = false;
    private bool _isAirSound = false;

    private void Start()
    {
        _playerController = GetComponent<TutorialPlayerController>();
    }

    public void UpdateSound(bool onGround, bool checkAir, float speed, bool isDrifting)
    {
        // 滑空音
        if (!_isAirSound && checkAir)
        {
            _flyAudio.Play();
            _isAirSound = true;
        }
        else if (_isAirSound && !checkAir)
        {
            _flyAudio.Stop();
            _isAirSound = false;
        }

        // 走行音
        if (!_isRunning && speed > 4 && onGround && !isDrifting)
        {
            _runningAudio.Play();
            _isRunning = true;
        }
        else if (_isRunning && (speed <= 4 || !onGround || isDrifting))
        {
            _runningAudio.Stop();
            _isRunning = false;
        }

        // ドリフト音
        if (_isDriftingSound && !isDrifting)
        {
            StopDriftSound();
        }
    }

    public void DriftSound()
    {
        _driftAudio.Play();
        _isDriftingSound = true;
    }

    public void StopDriftSound()
    {
        _driftAudio.Stop();
        _isDriftingSound = false;
    }

    public void PlayDashSound()
    {
        _dashAudio.Play();
    }

    public void PlayFireballSound()
    {
        _fireballAudio.Play();
    }

    public void PlayWarningSound()
    {
        _fireballWarningAudio.Play();
    }
    public void StopWarningSound()
    {
        _fireballWarningAudio.Stop();
    }
    public void PlayShieldSound()
    {
        _shieldAudio.Play();
    }

    public void PlayItemRouletteSound()
    {
        _itemRouletteAudio.Play();
    }

    public void StopItemRouletteSound()
    {
        _itemRouletteAudio.Stop();
    }

    public void WingChargeSound()
    {
        _wingChargeAudio.Play();
    }
    public void FireballHitSound()
    {
        _fireballHitAudio.Play();
    }
}
