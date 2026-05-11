using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //ドリフト開始時のジャンプの高さ
    private const float _driftJumpPower = 3f;
    //再度ダッシュが使えるようになるまでの時間
    private const float _dashCooldown = 8f;

    [SerializeField] public PlayerFlightGauge _flightGauge = default;
    [SerializeField] private PlayerItem _item = default;
    [SerializeField] private PlayerGroundCheck _groundCheck = default;
    [SerializeField] private PlayerInputHandler _inputHandler = default;
    [SerializeField] private PlayerAnimationController _animationController = default;
    [SerializeField] private PlayerMovement _playerMovement = default;
    [SerializeField] private Rigidbody _rb = default;
    [SerializeField] private bool _isPlayer = false;
    //Playerが進む速さ
    [SerializeField] private float _forwardForce = 5f;
    //最大速度
    [SerializeField] private float _maxSpeed = 10f;
    //曲がりやすさ
    [SerializeField] private float _rotateSpeed = 2f;
    //ドリフト時入力無しの掛ける値
    private float _driftWeight = 0.8f;
    private float _driftInputWeight = 0.5f;

    private float _steerInput = 0;
    private float _dashCooldownTimer = 0f;
    private bool _pendingDrift = false;
    private bool _isDrifting = false;
    private bool _canDashing = true;
    private bool _isFlying = false;
    private float _prevSpeed = 0f;//クラッシュ用のスピード変数
    private bool _crashCandidate = false;//クラッシュしているか
    private bool _isTrigger = false;
    private bool _isTriggerPressedUp = true;

    public bool _isFinish { get; set; } = false;

    #region 入力用変数
    private float _horizontal = 0;
    private float _vertical = 0;
    private float _leftTriggrt = 0;
    private float _rightTrigger = 0;
    private bool _Abtn = false;
    private bool _Bbtn = false;
    private bool _RBbtn = false;
    private bool _LBbtn = false;
    private bool _RBbtnUp = false;
    #endregion

    private bool _RBbtnDown = false;
    private bool _WasRBbtn = false;

    private float _isJumpTimer = 0;
    public enum DriftDirection
    {
        None,
        Left,
        Right,
    }

    private DriftDirection _driftDirection = DriftDirection.None;

    public bool IsDrifting
    {
        get => _isDrifting;
        set => _isDrifting = value;
    }
    public float SteerInput => _steerInput;
    public float DashCooldownTimer => _dashCooldownTimer;
    public float DashCoolDown => _dashCooldown;
    public PlayerCamera _playerCamera { get; set; } = default;

    public bool IsPlayer => _isPlayer;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        UpdateInput();
    }

    private void FixedUpdate()
    {
        DriftFix();
        Crash();
    }

    /// <summary>
    /// PlayerInputHandlerの入手値を参照する
    /// </summary>
    private void UpdateInput()
    {
        _horizontal = _inputHandler.Horizontal;
        _vertical = _inputHandler.Vertical;
        _leftTriggrt = _inputHandler.LeftTriggerAxis;
        _rightTrigger = _inputHandler.RightTriggerAxis;
        _Abtn = _inputHandler.IsAccelPressed;
        _Bbtn = _inputHandler.IsFlyPressed;
        _RBbtn = _inputHandler.IsDriftPressed;
        _LBbtn = _inputHandler.IsItemPressed;
        _RBbtnUp = _inputHandler.IsDriftReleased;
    }

    public void Controller()
    {
        #region コントローラー
        _RBbtnDown = false;
        if(_RBbtn && !_WasRBbtn)
        {
            _RBbtnDown = true;
        }
        _WasRBbtn = _RBbtn;

        _isJumpTimer = Mathf.Max(_isJumpTimer - Time.deltaTime, 0);

        if (_groundCheck.IsGrounded && 
            !_pendingDrift && !IsDrifting)
        {
            if (_RBbtnDown)
            {
                _animationController.PlayNonDriftStart();
                _isJumpTimer = 0.3f;
            }
            if (_RBbtn && Mathf.Abs(_horizontal) > 0.1f && _isJumpTimer > 0)
            {
                //ジャンプ中にドリフト待ち状態に
                _pendingDrift = true;
                if (_horizontal > 0)
                {
                    _animationController.PlayLeftDriftStart();
                }
                else
                {
                    _animationController.PlayRightDriftStart();
                }
                JumpForDrift();
            }
        }
        if (_RBbtnUp)
        {
            _animationController.StopDrift();
            _isDrifting = false;
        }
        if (_pendingDrift && !_isDrifting && _groundCheck.IsGrounded)
        {
            _isDrifting = true;
            _pendingDrift = false;
        }
        ///ドリフトした方向を取得///
        if (_isDrifting && _driftDirection == DriftDirection.None && _isPlayer)
        {
            if (_horizontal < -0.1f)
                _driftDirection = DriftDirection.Left;
            else if (_horizontal > 0.1f)
                _driftDirection = DriftDirection.Right;
        }
        else if (!_isDrifting)
        {
            _driftDirection = DriftDirection.None;
        }

        if (_rightTrigger > 0 && _isTriggerPressedUp)
        {
            _isTrigger = true;
            _isTriggerPressedUp = false;
        }
        else
        {
            _isTrigger = false;
            _isTriggerPressedUp = true;
        }
        if (_isTrigger && _canDashing && !_playerMovement.IsDamage)
        {
            _playerMovement.DashPlayer();
            _canDashing = false;
            _dashCooldownTimer = _dashCooldown;
        }
        else if (_dashCooldownTimer > 0f)
        {
            _dashCooldownTimer -= Time.deltaTime;
            if (_dashCooldownTimer < 0f)
            {
                _dashCooldownTimer = 0f;
                _canDashing = true;
            }
        }
        if (_Bbtn && !_isFlying && _flightGauge.IsFull)
        {
            _isFlying = true;
        }
        if (_isFlying)
        {
            if (_flightGauge.IsEmpty)
            {
                _isFlying = false;
            }
        }
        if (!_playerMovement.IsDamage)
        {
            if (_LBbtn)
            {
                _item.UseItem(PlayerItem.ItemUseType.Attack);
            }
            if (_leftTriggrt > 0)
            {
                _item.UseItem(PlayerItem.ItemUseType.Defense);
            }
        }
        #endregion
    }


    private void DriftFix()
    {
        ///ドリフト中の左右の入力を制限///
        _steerInput = _horizontal;
        if (_animationController.IsDrifting)
        {
            if (_driftDirection == DriftDirection.Left)
                _steerInput = _driftWeight * -1 + _driftInputWeight * _steerInput;
            else if (_driftDirection == DriftDirection.Right)
                _steerInput = _driftWeight * 1 + _driftInputWeight * _steerInput;
        }
    }

    private void JumpForDrift()
    {
        _rb.AddForce(Vector3.up * _driftJumpPower, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_crashCandidate) return;

        // 1. タグ確認
        if (!collision.gameObject.CompareTag("Wall")) return;

        //上方向の跳ねを抑える
        float MaxCrashBounce = 0f;

        Vector3 vel = _rb.velocity;
        vel.y = Mathf.Min(vel.y, MaxCrashBounce);
        _rb.velocity = vel;
        //_animationController.PlayDamage();
        _crashCandidate = false; // 忘れずリセット
    }

    private void Crash()
    {
        float currentSpeed = _rb.velocity.magnitude;
        float speedDelta = _prevSpeed - currentSpeed;
        float currentSpeedDifference = 6f;

        // 急減速したか
        if (speedDelta > currentSpeedDifference)
        {
            _crashCandidate = true;
        }

        _prevSpeed = currentSpeed;
    }
    public void CameraFovUp()
    {
        if(_playerCamera)
            _playerCamera.FovUp();
    }

    public void SetIsPlayer(bool setBool)
    {
        _isPlayer = setBool;
    }
}
