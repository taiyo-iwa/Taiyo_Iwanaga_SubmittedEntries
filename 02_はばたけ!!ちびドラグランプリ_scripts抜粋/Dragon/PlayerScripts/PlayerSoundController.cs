using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager = default;

    [SerializeField] private AudioSource _runningAudio = default;
    [SerializeField] private AudioSource _driftAudio = default;
    [SerializeField] private AudioSource _flyAudio = default;
    [SerializeField] private AudioSource _dashAudio = default;
    [SerializeField] private AudioSource _fireballAudio = default;
    [SerializeField] private AudioSource _fireballWarningAudio = default;
    [SerializeField] private AudioSource _shieldAudio = default;
    [SerializeField] private AudioSource _itemRouletteAudio = default;
    [SerializeField] private AudioSource _wingChargeAudio = default;
    [SerializeField] private AudioSource _fireballHitDamageVoiceAudio = default;
    [SerializeField] private AudioSource _fireballHitVoiceAudio = default;
    [SerializeField] private AudioSource _jumpVoiceAudio = default;
    [SerializeField] private AudioSource _dashVoiceAudio = default;

    [SerializeField] private AudioClip _damage;
    [SerializeField] private AudioClip _jump;
    [SerializeField] private AudioClip _dash;
    [SerializeField] private AudioClip _fireHit;

    [SerializeField] private AudioClip _tDamage;
    [SerializeField] private AudioClip _tJump;
    [SerializeField] private AudioClip _tDash;
    [SerializeField] private AudioClip _tFireHit;


    private PlayerController _playerController = default;
    private PlayerMove _playerMove = default;

    private bool _isRunning = false;
    private bool _isDriftingSound = false;
    private bool _isAirSound = false;

    private void Start()
    {
        _playerMove = GetComponent<PlayerMove>();
        _playerController = GetComponent<PlayerController>();
    }

    public void UpdateSound(bool onGround, bool checkAir, float speed, bool isDrifting)
    {
        if (_gameManager._twoPlayerSelectDragon >= 0 && !_playerController.IsPlayer)
            return;
        // ŠŠ‹ó‰¹
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

        // ‘–s‰¹
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

        if(_isDriftingSound && !isDrifting)
        {
            StopDriftSound();
        }
    }

    public void PlayDashSound()
    {
        if (_gameManager._twoPlayerSelectDragon >= 0 && !_playerController.IsPlayer)
            return;
        _dashAudio.Play();
    }

    public void PlayFireballSound()
    {
        if (_gameManager._twoPlayerSelectDragon >= 0 && !_playerController.IsPlayer)
            return;
        _fireballAudio.Play();
    }

    public void PlayWarningSound()
    {
        if (_gameManager._twoPlayerSelectDragon >= 0 && !_playerController.IsPlayer)
            return;
        _fireballWarningAudio.Play();
    }
    public void StopWarningSound()
    {
        if (_gameManager._twoPlayerSelectDragon >= 0 && !_playerController.IsPlayer)
            return;
        _fireballWarningAudio.Stop();
    }
    public void PlayShieldSound()
    {
        if (_gameManager._twoPlayerSelectDragon >= 0 && !_playerController.IsPlayer)
            return;
        _shieldAudio.Play();
    }

    public void PlayItemRouletteSound()
    {
        if (_gameManager._twoPlayerSelectDragon >= 0 && !_playerController.IsPlayer)
            return;
        _itemRouletteAudio.Play();
    }

    public void StopItemRouletteSound()
    {
        if (_gameManager._twoPlayerSelectDragon >= 0 && !_playerController.IsPlayer)
            return;
        _itemRouletteAudio.Stop();
    }

    public void WingChargeSound()
    {
        if (_gameManager._twoPlayerSelectDragon >= 0 && !_playerController.IsPlayer)
            return;
        _wingChargeAudio.Play();
    }

    public void FireballHitDamageSound()
    {
        if (_gameManager._twoPlayerSelectDragon >= 0 && !_playerController.IsPlayer)
            return;
        if (_playerMove._playerNumber == 0)
        {
            _dashVoiceAudio.PlayOneShot(_damage);
        }
        else if (_playerMove._playerNumber == 1)
        {
            _dashVoiceAudio.PlayOneShot(_tDamage,0.5f);
        }
    }

    public void FireballHitSound()
    {
        if (_gameManager._twoPlayerSelectDragon >= 0 && !_playerController.IsPlayer)
            return;
        if (_playerMove._playerNumber == 0)
        {
            _dashVoiceAudio.PlayOneShot(_fireHit);
        }
        else if (_playerMove._playerNumber == 1)
        {
            _dashVoiceAudio.PlayOneShot(_tFireHit, 0.5f);
        }
    }

    public void JumpSound()
    {
        if (_gameManager._twoPlayerSelectDragon >= 0 && !_playerController.IsPlayer)
            return;
        PlayDashSound();
        if (_playerMove._playerNumber == 0)
        {
            _dashVoiceAudio.PlayOneShot(_jump);
        }
        else if (_playerMove._playerNumber == 1)
        {
            _dashVoiceAudio.PlayOneShot(_tJump, 0.35f);
        }
    }

    public void DashSound()
    {
        if (!_playerController.IsPlayer)
            return;
        PlayDashSound();
        if (_playerMove._playerNumber == 0)
        {
            _dashVoiceAudio.PlayOneShot(_dash);
        }
        else if(_playerMove._playerNumber == 1)
        {
            _dashVoiceAudio.PlayOneShot(_tDash, 0.35f);
        }
    }

    public void DriftSound()
    {
        if (_gameManager._twoPlayerSelectDragon >= 0 && !_playerController.IsPlayer)
            return;
        _driftAudio.Play();
        _isDriftingSound = true;
    }

    public void StopDriftSound()
    {
        _driftAudio.Stop();
        _isDriftingSound = false;
    }
}
