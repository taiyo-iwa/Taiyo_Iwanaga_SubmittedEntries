using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWS;
using Cinemachine;

public class TutorialPlayerMove : MonoBehaviour
{
    //滑空スピード
    private const float _glideSpeed = 10f;
    //滑空中の落下速度
    [SerializeField] private float _glideFallSpeed = 5f;
    //落下加速度
    [SerializeField] private float _fallAcceleration = 1f;
    //上下入力速度
    private const float _pitchSpeed = 50f;
    //最大ロール角
    private const float _rollAmount = 30f;
    //傾きの反応速度
    private const float _rollSpeed = 3f;
    //空中の回転速度
    private const float _changePathRotationSpeed = 5;
    //空中の遷移時間
    [SerializeField] private float _airChangePathTime = 1.2f;

    [SerializeField] private TutorialManager _tutorialManager = default;
    [SerializeField] private TutorialPlayerController _playerController;
    [SerializeField] private PlayerGroundCheck _groundCheck;
    [SerializeField] private Transform _modelTransform;
    [SerializeField] private TutorialParticlManager _particlManager;
    [SerializeField] private PlayerInputHandler _inputHandler;
    [SerializeField] private TutorialPlayerMovement _playerMovement;
    [SerializeField] private PlayerAnimationController _animationController;
    [SerializeField] private TutorialSoundController _soundController;
    [SerializeField] private Rigidbody _rb;
    public int _playerNumber { get; set; } = -1;
    public CinemachineBrain _visualCamera { get; set; } = default;

    //空中遷移用オブジェクト
    [SerializeField] private pathMove airPoint;
    //地上遷移用オブジェクト
    [SerializeField] private pathMove groundPoint;
    //Player空中速度
    [SerializeField] private float airSpeed = 10;
    //Player空中回転速度
    [SerializeField] private float _airRotationSpeed = 100;
    private splineMove _splineMove;

    //空中の道の色(見える状態)
    [SerializeField] private Color _roadColor = new Color(1, 1, 1, 0.5f);
    //空中の道の色(見えない状態)
    private Color _cantSeeRoadColor = new Color(1, 1, 1, 0);

    //地上中状態
    public bool _groundPath { get; private set; } = true;
    //地上と空中の遷移状態
    public bool _changePath { get; private set; } = false;
    //滑空中
    private bool _isGliding = false;
    // 現在のロール角
    private float _currentRoll = 0f;
    //総合落下速度
    private float _fallVelocity = 0f;
    //空中のデフォルトの速度
    private float _airSpeedDefault;
    //遷移時間を保存する値
    private float _changePathTime = 0;
    //遷移中の目標の向き
    private Vector3 _targetDir = default;
    //遷移する最初の位置
    private Vector3 _changePathStartPos = default;
    //遷移中の目標の位置
    private Vector3 _changePathTargetPos = default;
    //遷移中の目標の向きを入れる
    private Vector3 _dirPos = default;
    //遷移中の次の目標の向きを入れる
    private Vector3 _dirPosNext = default;

    //空中遷移したときに道のマテリアルの色を変えるのに必要
    public Material _roadMaterial { get; set; } = default;

    public bool GroundPath => _groundPath;
    public bool IsGliding => _isGliding;

    public bool GetChangePath => _changePath;

    void Start()
    {
        _airSpeedDefault = airSpeed;
        _splineMove = GetComponent<splineMove>();
        //Application.targetFrameRate = 30;
        if (_visualCamera)
            _visualCamera.m_UpdateMethod = CinemachineBrain.UpdateMethod.SmartUpdate;
    }
    public void PlayUpdate()
    {
        UpdateInput();
        //遷移中で上昇の場合は1 空中で自由になったら0 遷移中で下降する場合は-1
        float upDown = 0;
        if (_changePath)
        {
            if (_groundPath)
            {
                upDown = 0.8f;
            }
            else
            {
                upDown = -0.8f;
            }
        }
        //上下の向きを動かすupDownの値が必要なため遷移中の処理の上に書く
        _playerMovement.ModelMove(_playerController.SteerInput, _playerController.IsDrifting, _isGliding);
        _animationController.UpdateAnimation(
            _horizontal,
            _rb.velocity.magnitude,
            _playerMovement._airTime,
            _rb.velocity.y,
            !_groundPath || _changePath,
            _isGliding,
            upDown);
        //遷移中の処理
        if (_changePath)
        {
            ChangePathUpdate();
            return;
        }

        //動作処理
        if (_groundPath)
        {
            if (!_isGliding)
            {
                GroundMove();
            }
            if (_tutorialManager._isBbtnEvent)
            {
                if (_Bbtn)
                {
                    if (_playerController._flightGauge.SetUseGauge())
                    {
                        ChangePath();
                    }
                }
            }
        }
        else
        {
            AirMove();
        }
        //滑空

        _soundController.UpdateSound(
            !_isGliding && _groundPath && !_changePath,
            CheckAir(),
            _rb.velocity.magnitude,
            _playerController.IsDrifting);
    }
    public bool CheckAir()
    {
        return _isGliding || _changePath || !_groundPath;
    }
    //地上動作
    public void PlayFixedUpdate()
    {
        if (_isGliding)
        {
            Glide();
            return;
        }
        if (!_changePath && _groundPath)
        {
            _playerMovement.MovePlayer(
            inputHorizontal: _horizontal,
            isAccel: _Abtn,
            isDrifting: _playerController.IsDrifting,
            steerInput: _playerController.SteerInput,
            isPlayer: _playerController.IsPlayer
            );

        }
    }

    #region 入手処理
    //入力値の保存用
    private float _horizontal = 0;
    private float _vertical = 0;
    private float _leftTrigger = 0;
    private float _rightTrigger = 0;
    private bool _Abtn = false;
    private bool _Bbtn = false;
    private bool _RBbtn = false;
    private bool _LBbtn = false;
    /// <summary>
    /// PlayerInputHandlerの入手値を参照する
    /// </summary>
    private void UpdateInput()
    {
        _horizontal = _inputHandler.Horizontal;
        _vertical = _inputHandler.Vertical;
        _leftTrigger = _inputHandler.LeftTriggerAxis;
        _rightTrigger = _inputHandler.RightTriggerAxis;
        _Abtn = _inputHandler.IsAccelPressed;
        _Bbtn = _inputHandler.IsFlyPressed;
        _RBbtn = _inputHandler.IsDriftPressed;
        _LBbtn = _inputHandler.IsItemPressed;
    }
    #endregion

    public void GroundMove()
    {
        _playerController.Controller();
    }

    //空中動作
    public void AirMove()
    {
        // 通常の空中移動処理
        transform.rotation = Quaternion.Euler(new Vector3(
            transform.rotation.eulerAngles.x,
            transform.eulerAngles.y + _horizontal * Time.deltaTime * _airRotationSpeed,
            transform.rotation.eulerAngles.z));

        airSpeed = Mathf.Lerp(airSpeed, _airSpeedDefault, 1 - Mathf.Exp(-5 * Time.deltaTime));

        #region 左右の壁の処理
        //飛ばす場所　プレイヤーの位置
        Vector3 origin = transform.position;
        //飛ばす長さ 進む距離を入れる
        float rayLength = 1;
        //飛ばす方向　プレイヤーの向き
        Vector3 dir = transform.forward * (rayLength + 0.5f);

        Debug.DrawRay(origin, dir, Color.red);
        RaycastHit hit;

        if (Physics.SphereCast(origin, 0.1f, dir, out hit, rayLength, 1 << 9))
        {
            transform.position -= transform.forward * (rayLength - hit.distance);
            transform.position += Vector3.ProjectOnPlane(transform.forward * Time.deltaTime * airSpeed, hit.normal);
        }
        else
        {
            transform.position += transform.forward * Time.deltaTime * airSpeed;
        }
        #endregion

        #region 地面に沿わせる処理
        //飛ばす場所　現在の位置より少し上
        origin = transform.position + Vector3.up * 0.5f;
        //飛ばす長さ 
        rayLength = 4.5f;
        //飛ばす方向　
        dir = Vector3.down * rayLength;

        Debug.DrawRay(origin, dir, Color.red);

        if (Physics.Raycast(origin, dir, out hit, rayLength, 1 << 9))
        {
            Quaternion current = transform.rotation;

            // 目標回転を作る
            Vector3 up = hit.normal;
            Vector3 right = transform.right;
            Vector3 forward = Vector3.Cross(right, up).normalized;
            Quaternion target = Quaternion.LookRotation(forward, up);

            // 減衰付き Slerp
            float t = 1f - Mathf.Exp(-3 * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(current, target, t);

            transform.position = hit.point + Vector3.up * 3.5f;
        }
        else
        {
            transform.position += Vector3.down * 5 * Time.deltaTime;
        }

        #endregion

        //ゲージを消費させて、0になったら地上に戻る
        if (_playerController._flightGauge.Deplete(Time.deltaTime))
        {
            ChangePath();
        }
    }

    private void Glide()
    {
        // --- 現在の角度を取得 ---
        float pitch = transform.eulerAngles.x;
        if (pitch > 180f) pitch -= 360f;

        // --- 垂直入力を取得 ---
        float inputPitch = _vertical; // 上が正で下が負

        // --- 上下移動を入力に応じて角度を変える
        pitch += inputPitch * Time.deltaTime * -_pitchSpeed;

        // --- 上下の角度を制限（上、下）---
        pitch = Mathf.Clamp(pitch, -20f, 20f);

        // --- 左右移動を入力に応じて角度を変える
        float yaw = transform.eulerAngles.y + _horizontal * Time.deltaTime * 50f;

        // --- ロール角（Z軸）を入力に応じて変化させる ---
        float targetRoll = -_horizontal * _rollAmount;
        _currentRoll = Mathf.Lerp(_currentRoll, targetRoll, Time.deltaTime * _rollSpeed);

        transform.rotation = Quaternion.Euler(pitch, yaw, _currentRoll);

        float speedModifier = (pitch > 0f) ? Mathf.Lerp(1f, 0.5f, pitch / 30f) : 1f;

        // 前進
        transform.position += transform.forward * _glideSpeed * speedModifier * Time.deltaTime;

        // 落下処理
        _fallVelocity = Mathf.Lerp(_fallVelocity, -_glideFallSpeed, Time.deltaTime * _fallAcceleration);
        transform.position += new Vector3(0f, _fallVelocity, 0f) * Time.deltaTime;


        ///<summary> 着地チェック </summary>///
        if (_groundCheck.IsGrounded)
        {
            _rb.useGravity = true;
            _rb.isKinematic = false;
            _isGliding = false;
            _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            Landing();
        }
    }

    //空中と地上の遷移
    public void ChangePath()
    {
        ///<summary> PlayerControllerの変数を戻す </summary>
        _modelTransform.localRotation = _modelTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        if (_playerController.IsPlayer)
        {
            OffSparksEffect();
        }
        _playerController.IsDrifting = false;
        airSpeed = _airSpeedDefault;
        //滑空判定を消す
        _isGliding = false;
        _rb.useGravity = true;
        //物理エンジンを解除
        _rb.isKinematic = true;
        if (_visualCamera)
            _visualCamera.m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;

        //スタート地点、目標地点、向きを設定
        _changePathStartPos = transform.position;
        if (_groundPath)
        {
            _changePathTargetPos = airPoint.transform.position;
            _dirPos = airPoint.transform.position;
            _dirPosNext = airPoint.GetNextCurrentPoint();
        }
        else
        {
            _changePathTargetPos = groundPoint.transform.position;
            _dirPos = groundPoint.transform.position;
            _dirPosNext = groundPoint.GetNextCurrentPoint();
        }
        _targetDir = (_dirPosNext - _dirPos).normalized;

        _changePathTime = 0;
        _changePath = true;
    }

    //遷移中のupdateの処理
    public void ChangePathUpdate()
    {
        _changePathTime += Time.deltaTime;
        if (_changePathTime <= _airChangePathTime)
        {
            //角度を調べる
            float targetAngle = Mathf.Atan2(_targetDir.x, _targetDir.z) * Mathf.Rad2Deg;
            Vector3 currentEulerAngle = transform.eulerAngles;
            //指定の角度に向けて指数関数的に減衰させる
            float angleDisY = Mathf.LerpAngle(currentEulerAngle.y, targetAngle, 1 - Mathf.Exp(-_changePathRotationSpeed * Time.deltaTime));
            float angleDisX = Mathf.LerpAngle(currentEulerAngle.x, 0, 1 - Mathf.Exp(-_changePathRotationSpeed * Time.deltaTime));
            //方向を適用する
            transform.rotation = Quaternion.Euler(angleDisX, angleDisY, 0);
            //移動処理をする
            transform.position = Vector3.Lerp(_changePathStartPos, _changePathTargetPos, _changePathTime / _airChangePathTime);
            //マテリアルがついてるかどうか（１ｐ、２ｐそれぞれ指定のマテリアルをつける　ＡＩならつけない　で判断)
            if (_roadMaterial)
            {
                //ついているなら遷移するときに徐々に色を変えていく
                if (_groundPath)
                {
                    _roadMaterial.color = Color.Lerp
                        (_cantSeeRoadColor, _roadColor, _changePathTime / _airChangePathTime);
                }
                else
                {
                    _roadMaterial.color = Color.Lerp
                        (_roadColor, _cantSeeRoadColor, _changePathTime / _airChangePathTime);
                }
            }
            return;
        }
        if (_roadMaterial)
        {
            if (_groundPath)
            {
                _roadMaterial.color = _roadColor;
            }
            else
            {
                _roadMaterial.color = _cantSeeRoadColor;
            }
        }

        transform.position = _changePathTargetPos;
        _changePath = false;
        _groundPath = !_groundPath;
        if (_groundPath)
        {
            Landing();
        }
        return;
    }

    public void Landing()
    {
        _tutorialManager._isBbtnEvent = false;
        _changePath = false;
        _groundPath = true;
        //X(縦の回転を0にする)
        transform.rotation = Quaternion.Euler(new Vector3(
            0, transform.eulerAngles.y, 0));
        _rb.isKinematic = false;
        if (_visualCamera)
            _visualCamera.m_UpdateMethod = CinemachineBrain.UpdateMethod.SmartUpdate;
        _rb.velocity = transform.forward * 10;
    }

    public void StartGlide()
    {
        if (_isGliding || _changePath) return;

        ///<summary> 初期状態にする </summary>///
        _isGliding = true;
        _rb.isKinematic = true;
        _rb.useGravity = false;
        _fallVelocity = 0f;
        _modelTransform.localRotation = _modelTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        if (_playerController.IsPlayer)
        {
            OffSparksEffect();
        }
        ///<summay> FreezeRotationXの解除 + 姿勢を直す </summay>
        _rb.freezeRotation = false;

        // 初速でほんの少しだけ下げて滑空感を出す
        Vector3 v = _rb.velocity;
        v.y = -1f;
        _rb.velocity = v;
    }

    public void AirDash()
    {
        airSpeed += 50;
        _animationController.AirDashAnimation();
        if (_particlManager != null)
        {
            _particlManager.StartHyperDrive();
            if (_playerController.IsPlayer)
                _particlManager.AirDashSpark();
        }
    }

    private void OffSparksEffect()
    {
        if (_playerNumber == 0)
        {
            _particlManager.PlayerOneSparksOff();
        }
        if (_playerNumber == 1)
        {
            _particlManager.PlayerTwoSparksOff();
        }
    }
}
