using UnityEngine;

public class PlayerChargeDash : MonoBehaviour
{
    //最低限のチャージ時間
    private const float MINIMUM_CHARGE_TIME = 0.1f;
    //最大チャージ時間
    private const float MAX_CHARGE_TIME = 2.0f;
    //チャージスピード
    private const float CHARGE_SPEED = 1.5f;
    //チャージダッシュの加速力
    private const float CHARGE_ACCELERATION_FORCE = 15.0f;
    //進行方向をチャージダッシュ方向によせる率
    private const float CORRECTION_RATE_DASH_DIRECTION = 1.0f;

    [SerializeField] private Rigidbody _rigidbody = default;
    [SerializeField] private ChargeCurve _chargeCurve = default;
    [SerializeField] private PlayerStatus _playerStatus = default;
    [SerializeField] private PlayerChargeDashView _playerChargeDashView = default;

    private bool _isPlayerBrake = false;
    private bool _wasCharging = false;
    private float _chargeTime = 0;

    public void PlayerSouthButtonInput(bool southButtonInput)
    {
        _isPlayerBrake = southButtonInput;
    }

    public void UpdatePlayerChargDash()
    {
        PlayerChargeStart();
    }

    private void PlayerChargeStart()
    {
        if(_isPlayerBrake)
        {
            _chargeTime += Time.deltaTime * CHARGE_SPEED;
            _wasCharging = true;
        }
        else
        {
            //直前までチャージしていたら
            if (_wasCharging)
            {
                TryMiniTurbo();
            }
            _chargeTime = 0;
            _wasCharging = false;
        }

        if (!_playerStatus.CanMove)
        {
            return;
        }
        _playerChargeDashView.DashChargeBerController(Mathf.InverseLerp(MINIMUM_CHARGE_TIME, MAX_CHARGE_TIME, _chargeTime));
    }

    //Aボタンを離したらする処理
    private void TryMiniTurbo()
    {
        //カウントダウン中はチャージはできるがダッシュはできない
        if (!_playerStatus.CanReleaseCharge)
        {
            return;
        }

        // ドリフトが短すぎたら何もなし
        if (_chargeTime < MINIMUM_CHARGE_TIME)
        {
            return;
        }

        // ドリフト時間に応じてブースト量を線形補間
        float time = Mathf.InverseLerp(MINIMUM_CHARGE_TIME, MAX_CHARGE_TIME, _chargeTime);
        float boost = _chargeCurve.GetValue(time);

        Vector3 currentVelocity = _rigidbody.linearVelocity;
        currentVelocity.y = 0;

        //現在のスピードは変えないように向きとは別に保持しておく
        float currentSpeed = currentVelocity.magnitude;
        if(currentSpeed > 0.01f)
        {
            Vector3 currentDirection = currentVelocity.normalized;
            Vector3 targetDirection = transform.forward;

            //進行方向をチャージダッシュ方向に寄せる
            Vector3 newDirection = Vector3.Lerp(currentDirection, targetDirection, CORRECTION_RATE_DASH_DIRECTION).normalized;
            currentVelocity = newDirection * currentSpeed;
        }

        _rigidbody.linearVelocity = new Vector3(currentVelocity.x, _rigidbody.linearVelocity.y, currentVelocity.z);
        _rigidbody.AddForce(transform.forward * boost * CHARGE_ACCELERATION_FORCE, ForceMode.VelocityChange);
        //チャージダッシュしたことを通知
        _playerStatus.NotifyStartChargeDash();
    }
}
