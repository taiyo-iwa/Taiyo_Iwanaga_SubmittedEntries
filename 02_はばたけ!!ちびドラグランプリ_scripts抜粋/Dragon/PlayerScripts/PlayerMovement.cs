using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //ダッシュ時の加速の力
    private const float _dashForce = 20f;
    //ドリフト時の曲がりやすさ
    private const float _driftPower = 0.6f;
    //尻振り角度
    private const float _driftTiltAmount = 40f;
    //横に傾く角度
    private const float _bankAmount = 0f;
    //傾くまでの速さ
    private float _modelRotateSpeed = 10f;
    //プレイヤーが敵に対して反応する距離
    private const  float _minDistance = 3f;
    //プレイヤーが敵に対して反応する最低角度
    private const float _minAngle = 80f;
    //プレイヤーが敵に対して反応する最大角度
    private const float _maxAngle = 120f;
    //クラッシュ時のノックバックの力
    private const float _knockbackPower = 3f;
    //クラッシュ操作無効時間
    private const float _crashDisableTime = 0.7f;
    // ボーナスが発生し始める時間
    private const float _minDriftForBoost = 0.5f;
    // 最大ブーストに達するまでの時間
    private const float _maxDriftForMaxBoost = 3f;
    // 最大ミニターボの力
    private const float _maxMiniTurboForce = 20f;
    //ダメージの効果時間
    private const float _fireballDamageTime = 2.0f;
    private const float _damageTime = 0.5f;
    //ダメージの減速速度
    private const float _damageDecay = 1.2f;

    //プレイヤーの走る速さ
    public float _runForce = 20f;//初期設定20
    //プレイヤーの最高速度
    public float _maxRunSpeed = 12;//初期設定12
    //プレイヤーの曲がりやすさ
    public float _rotateSpeed = 1.5f;//初期設定1.5f

    //スクリプタブルオブジェクト
    [SerializeField] private KubonStatus     _kubonStatus;
    [SerializeField] private ToyaStatus      _toyaStatus;
    [SerializeField] private HanabiStatus    _hanabiStatus;
    [SerializeField] private MegunoriaStatus _megunoriaStatus;
    [SerializeField] private MoruruStatus    _moruruStatus;

    [SerializeField] private Rigidbody _rb = default;
    [SerializeField] private ParticlManager _particlManager = default;
    [SerializeField] public PlayerFlightGauge _flightGauge = default;
    [SerializeField] private Transform modelTransform = default;
    [SerializeField] private PlayerAnimationController _animationController = default;
    [SerializeField] private PlayerSoundController _soundController = default;
    
    private PlayerController _playerController = default;
    private GameObject ridingObj;//乗っていたオブジェクトを保存しておく
    private merryGoRoundInfo _merryGoRoundInfo;//乗っているメリーゴーランドを保存しておく
    private WingChargeFloorInfo _wingChargeFloorInfo;
    private float _prevSpeed = 0f;//クラッシュ用のスピード変数
    private bool _crashCandidate = false;//クラッシュしているか
    private bool _isCrashing = false;
    private float _driftDuration = 0f;// 現在のドリフト継続時間
    private bool _wasDrifting = false; // 前フレームでドリフト中だったか
    private float _damageTimer = 0f;
    private float _invincibleTimer = 0f;

    public int _dragonIdentificationNumber;

    public float _modelY { get; private set; }
    public float _airTime { get; private set; }
    public int _playerNumber { get; set; } = -1;
    public bool IsDamage => _damageTimer > 0;
    public bool IsInvincible => _invincibleTimer > 0;
    private void Start()
    {
        /***スクリプタブルオブジェクトからの値の取得***/
        if (_dragonIdentificationNumber == 0)
        {
            _runForce    = _kubonStatus.RunForce;
            _maxRunSpeed = _kubonStatus.MaxRunSpeed;
            _rotateSpeed = _kubonStatus.RotateSpeed;
        }
        else if (_dragonIdentificationNumber == 1)
        {
            _runForce    = _toyaStatus.RunForce;
            _maxRunSpeed = _toyaStatus.MaxRunSpeed;
            _rotateSpeed = _toyaStatus.RotateSpeed;
        }
        else if (_dragonIdentificationNumber == 2)
        {
            _runForce    = _hanabiStatus.RunForce;
            _maxRunSpeed = _hanabiStatus.MaxRunSpeed;
            _rotateSpeed = _hanabiStatus.RotateSpeed;
        }
        else if (_dragonIdentificationNumber == 3)
        {
            _runForce    = _megunoriaStatus.RunForce;
            _maxRunSpeed = _megunoriaStatus.MaxRunSpeed;
            _rotateSpeed = _megunoriaStatus.RotateSpeed;
        }
        else if (_dragonIdentificationNumber == 4)
        {
            _runForce    = _moruruStatus.RunForce;
            _maxRunSpeed = _moruruStatus.MaxRunSpeed;
            _rotateSpeed = _moruruStatus.RotateSpeed;
        }

        _playerController = GetComponent<PlayerController>();
    }
    /// <summary>
    /// プレイヤーの移動、ドリフト、ダッシュ、姿勢を操作するメソッド
    /// </summary>
    /// <param name="inputHorizontal">プレイヤーの左右の入力</param>
    /// <param name="isAccel">プレイヤーのAボタンの入力</param>
    /// <param name="isDashing">プレイヤーのTriggerボタンの入力</param>
    /// <param name="isDrifting">プレイヤーのRBボタンの入力</param>
    /// <param name="steerInput">ドリフト中の左右の入力を制限するための変数</param>
    public void MovePlayer(float inputHorizontal, bool isAccel, bool isDrifting, float steerInput, bool isPlayer)
    {
        _invincibleTimer -= Time.fixedDeltaTime;
        _damageTimer -= Time.fixedDeltaTime;
        //velocityYは個別に変化させるため、保存しておく
        float velocityY = _rb.velocity.y;
        //勝手に回転されないように回転のvelocityは0にする
        _rb.angularVelocity = Vector3.zero;

        #region　移動
        if (_rb.velocity.magnitude < _maxRunSpeed)
        {
            float AccelPress = 0;
            if((isAccel || _animationController.IsDrifting) && _damageTimer <= 0)
            {
                AccelPress = 1;
            }
            _rb.AddForce(transform.forward * AccelPress * _runForce);
        }
        #endregion

        #region ドリフト
        //float steerSpeed = Mathf.Lerp(0, _rotateSpeed, _rb.velocity.magnitude / _maxRunSpeed);
        float steerSpeed = _rotateSpeed;
        if (_animationController.IsDrifting && new Vector3(_rb.velocity.x, 0, _rb.velocity.z).sqrMagnitude > 70
            && !IsDamage && _modelY >= 0)
        {
            //steerSpeed *= 2f;
            if (isPlayer)
            {
                OnSparksEffect();
                SparksEffectColor();
            }
            //_flightGauge.Charge(Time.deltaTime);
            _driftDuration += Time.deltaTime;
        }
        else
        {
            //ドリフトボタンを離した以外で解除になった場合(ぶつかったり速度が下がった場合)
            //ドリフトのターボはしない
            if (_animationController.IsDrifting)
            {
                _driftDuration = 0;
            }
            if (isPlayer)
            {
                OffSparksEffect();
            }
            // 直前までドリフトしていたならミニターボ解放
            if (_wasDrifting)
            {
                TryReleaseMiniTurbo();
            }
            _driftDuration = 0f;
        }
        _wasDrifting = _animationController.IsDrifting && !IsDamage;

        ///絶対値の大きい方をsteerSpeedに入れる///
        //if(Mathf.Abs(steerInput * steerSpeed) > Mathf.Abs(steerInput))
        //{
        //    steerSpeed = steerInput * steerSpeed;
        //}
        //else
        //{
        //    steerSpeed = steerInput;
        //}
        steerSpeed = steerInput * steerSpeed;
        //止まってても曲がれるようにした　ただしスピード出てるときよりも遅めに
        _rb.MoveRotation(Quaternion.Euler(new Vector3(0, steerSpeed, 0)) * _rb.rotation);

        //前に進むスピードを取得
        Vector3 forwardVelocity = transform.forward * Vector3.Dot(_rb.velocity, transform.forward);
        //横に進むスピードを取得
        Vector3 sideVelocity = transform.right * Vector3.Dot(_rb.velocity, transform.right);

        //ドリフト中かつダメージを受けた後でなければ、横方向に対しての減速をさせる
        if(!_animationController.IsDrifting && !IsDamage)
            sideVelocity *= 0.9f;
        //合成
        Vector3 vel = forwardVelocity + sideVelocity;
        //指数関数的に速度を減衰させる
        //ドリフトしてたら多めに減衰させる
        float decayRate = _animationController.IsDrifting ? 1.5f : 1f;
        if(IsDamage)
        {
            decayRate = _damageDecay;
        }
        float decayFactor = 1 - Mathf.Exp(-decayRate * Time.deltaTime);
        vel = vel.normalized * Mathf.Lerp(vel.magnitude, 0, decayFactor);
        
        //横に進むスピードにドリフトの力を加えて前方向に進むスピードと合体させる
        //（ドリフトしていなければ横方向のスピードは0になる）
        //Vector3 driftVel = forwardVelocity + sideVelocity * driftPower;

        _rb.velocity = new Vector3(vel.x, velocityY, vel.z);
        if(_animationController.IsDrifting && _damageTimer <= 0)
            _rb.AddForce(transform.right * inputHorizontal * 10);

        #endregion

        #region 段差対策とメリーゴーランド、チャージ床
        ///<summary>
        ///Playerを地面の高さによって浮かす
        ///</summary>
        //飛ばす場所
        Vector3 origin = transform.position + new Vector3(0, 0.25f, 0);
        //飛ばす方向
        Vector3 dir = new Vector3(0, -0.4f, 0);
        //飛ばす長さ
        float rayLength = 0.4f;

        //上方向に力を加える処理
        //加えて、メリーゴーランドの上にいる場合は回転をする
        Debug.DrawRay(origin, dir, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(origin, dir, out hit, rayLength, 1 << 8, QueryTriggerInteraction.Ignore))
        {
            _rb.AddForce(new Vector3(0, (1.0f - (hit.distance / rayLength)) * 30 - (_rb.velocity.y * 4.0f), 0), ForceMode.Acceleration);
            _modelY = hit.distance;
            _airTime = 0;
            //乗っているオブジェクトの情報を更新
            if (hit.collider.gameObject != ridingObj)
            {
                _merryGoRoundInfo = hit.collider.GetComponent<merryGoRoundInfo>();
                _wingChargeFloorInfo = hit.collider.GetComponent<WingChargeFloorInfo>();
                ridingObj = hit.collider.gameObject;
            }
            //メリーゴーランドの場合、velocityを回転方向へ加算
            if (_merryGoRoundInfo)
            {
                Vector3 angularVelocity = _merryGoRoundInfo.RotationAxis * _merryGoRoundInfo.RotationSpeed; //回転軸 * 回転速度
                Vector3 relativePos = transform.position - _merryGoRoundInfo.transform.position; //プレイヤー位置 - 回転中心
                relativePos.y = 0; //高さを考慮しない(angularVelocityとrelativePosを垂直にする)
                //外積を求める(両方のvectorに垂直になるvectorを求める)
                Vector3 tangentialVelocity = Vector3.Cross(angularVelocity, relativePos);
                Vector3 setPosition = (transform.position + tangentialVelocity);

                //飛ばす場所　プレイヤーの位置
                origin = transform.position;
                //飛ばす方向　プレイヤーの向き
                dir = (setPosition - origin);
                //飛ばす長さ 進む距離を入れる
                rayLength = dir.magnitude;

                Debug.DrawRay(origin, dir, Color.red);

                if (Physics.SphereCast(origin, 0.1f, dir, out hit, rayLength, 1 << 7, QueryTriggerInteraction.Ignore))
                {
                    setPosition = transform.position;
                }
                //回転方向に位置を加算
                _rb.MovePosition(setPosition);
            }
            //チャージ床の場合、ウィングゲージを加算する
            if (_wingChargeFloorInfo)
            {
                _flightGauge.Charge(_wingChargeFloorInfo.ChargeValue * Time.fixedDeltaTime);
            }
        }
        else
        {
            _modelY = -1;
            _airTime += Time.deltaTime;
        }
        #endregion
    }

    public void ModelMove(float horizontal, bool isDrifting, bool isGliding)
    {

        #region モデル姿勢
        //平面上のvelocityの向きを取得（Yを考慮しない）
        Vector3 moveDir = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        //vlocityの速度が0.01fより大きいことが条件
        if (moveDir.sqrMagnitude > 0.01f)
        {
            //平面上で向いている方向を調べる
            Quaternion targetRot = default;
            if (_damageTimer <= 0)
            {
                targetRot = Quaternion.LookRotation(moveDir);
            }
            else
            {
                targetRot = transform.rotation;
            }

            // -------- Y軸：進行方向 + ドリフトの尻振り演出 --------
            if (_animationController.IsDrifting && !IsDamage)
            {
                float driftAngleY = horizontal * _driftTiltAmount + Mathf.Sign(horizontal) * 20; // 例：±25度
                targetRot *= Quaternion.Euler(0f, driftAngleY, 0f);
            }

            // -------- Z軸：ドリフト時の“傾き（バンク）”演出 --------
            float bankAngleZ = (_animationController.IsDrifting && _damageTimer <= 0) ? - horizontal * _bankAmount : 0f; // 左に曲がると右に傾く
            Quaternion bankRot = Quaternion.Euler(0f, 0f, bankAngleZ);

            // -------- 最終合成：Y向きに回転しつつ、Z軸にバンクさせる --------
            Quaternion finalRot = targetRot * bankRot;

            modelTransform.rotation = Quaternion.Lerp(modelTransform.rotation, finalRot, Time.deltaTime * _modelRotateSpeed);

            ////下に飛ばしたrayの距離に応じてモデルの位置の高さを変化させる
            ////（めり込んで見えないようにさせる。ただしやりすぎると凹凸や切れ目の地形で違和感が出るから範囲指定）
            if (_modelY >= 0)
            {
                modelTransform.localPosition = new Vector3(0, Mathf.Clamp(0.25f - _modelY, -0.05f, 0.05f), 0);
            }
        }
        #endregion

        if (!isDrifting || !isGliding)
        {
            //敵が近くにいれば、敵の方を向かせる
            if (ShouldLookAtEnemy(out Transform enemy))
            {
                _animationController.PlayNeckBending();
            }
            else
            {
                //敵が近くにいない時の処理
                _animationController.StopNeckBending();
            }
        }
    }

    private bool ShouldLookAtEnemy(out Transform nearestEnemy)
    {
        nearestEnemy = null;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            //プレイヤーが敵に対して向いている角度
            Vector3 dir = enemy.transform.position - transform.position;
            float distance = dir.magnitude;
            float angle = Vector3.Angle(transform.forward, dir);
            //プレイヤーが敵の左右どちらにいるか
            Vector3 cross = Vector3.Cross(this.transform.forward, dir.normalized);
            float direction = cross.y;

            if (distance < _minDistance && angle > _minAngle && angle < _maxAngle)
            {
                _animationController.EnemyPosition(direction * 2);
                nearestEnemy = enemy.transform;
                return true;
            }
        }
        return false;
    }

    public void DashPlayer()
    {
        if (_playerNumber == 0)
        {
            _particlManager.DriftBoost();
            _playerController.CameraFovUp();
        }
        if (_playerNumber == 1)
        {
            _particlManager.TwoPlayerDriftBoost();
            _playerController.CameraFovUp();
        }
        _rb.AddForce(transform.forward * _dashForce, ForceMode.Impulse);
        _soundController.PlayDashSound();
    }

    /// <summary>
    /// ミニターボを解放したときの処理のメソッド
    /// </summary>
    private void TryReleaseMiniTurbo()
    {
        // ドリフトが短すぎたら何もなし
        if (_driftDuration < _minDriftForBoost) return;

        // ドリフト時間に応じてブースト量を線形補間
        float time = Mathf.InverseLerp(_minDriftForBoost, _maxDriftForMaxBoost, _driftDuration);
        float boostAmount = Mathf.Lerp(0f, _maxMiniTurboForce, time);

        // 加速を与える（進行方向に一瞬だけインパルス）
        _rb.AddForce(transform.forward.normalized * boostAmount, ForceMode.Impulse);

        //ブーストパティークル
        if(_playerNumber == 0 || _playerNumber == 1)
        {
            _playerController.CameraFovUp();
            _particlManager.StartHyperDrive(_playerNumber, true);
        }
    }

    public void FireballDamage()
    {
        if(!IsDamage && !IsInvincible)
        {
            _animationController.PlayFireballDamage();
            _driftDuration = 0f;
            _damageTimer = _fireballDamageTime;
        }
    }
    
    public void Damage()
    {
        if(!IsDamage)
        {
            _animationController.PlayDamage();
            _driftDuration = 0f;
            _damageTimer = _damageTime;
        }
    }

    public void TlexDamage()
    {
        if (!IsDamage)
        {
            _driftDuration = 0f;
            _damageTimer = _damageTime;
            _animationController.StartSpin();
        }
    }

    public void Hammer()
    {
        if(_damageTimer <= 0)
        {
            //_animationController
        }
    }

    private void OnSparksEffect()
    {
        if (_playerNumber == 0)
        {
            _particlManager.PlayerOneSparksOn();
        }
        if (_playerNumber == 1)
        {
            _particlManager.PlayerTwoSparksOn();
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

    private void SparksEffectColor()
    {
        if (_playerNumber == 0)
        {
            // チャージ量によってスパークの色が変わる条件
            if (_driftDuration < 1f)
            {
                _particlManager.OneStageSpark();
            }
            else if (_driftDuration >= 1f && _driftDuration < 2f)
            {
                _particlManager.TwoStageSpark();
            }
            else if (_driftDuration >= 2f)
            {
                _particlManager.ThreeStageSpark();
            }
        }
        if (_playerNumber == 1)
        {
            // チャージ量によってスパークの色が変わる条件
            if (_driftDuration < 1f)
            {
                _particlManager.TwoPlayerOneStageSpark();
            }
            else if (_driftDuration >= 1f && _driftDuration < 2f)
            {
                _particlManager.TwpPlayerTwoStageSpark();
            }
            else if (_driftDuration >= 2f)
            {
                _particlManager.TwoPlayerThreeStageSpark();
            }
        }
    }

    //無敵時間を引数の秒数分設定する
    public void SetInvincibleTime(float setValue)
    {
        _invincibleTimer = setValue;
    }
}
