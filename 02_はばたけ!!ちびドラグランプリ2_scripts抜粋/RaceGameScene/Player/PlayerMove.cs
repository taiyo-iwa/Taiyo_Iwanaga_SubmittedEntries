using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    //プレイヤーの最高速度
    private const float MAX_RUN_SPEED = 20.0f;
    //プレイヤーの加速力
    private const float ACCELERATION_FORCE = 10.0f;
    //ハンドル操作の方向を変えるスピード
    private const float HANDLE_ROTATE_SPEED = 3.5f;
    //ステック操作の曲がりやすさ
    private const float STICK_ROTATE_SPEED = 180.0f;
    //入力からプレイヤーの角度の補正角度
    private const int CORRECTION_ANGLE = 180;
    //通常時の前方向の減衰力
    private const float BASE_FORWARD_ATTENUATION = 1.0f;
    //通常時の横方向の減衰力
    private const float BASE_SIDE_ATTENUATION = 1.3f;
    //ブレーキ時の前方向の減衰力
    private const float BRAKE_FORWARD_ATTENUATION = 0.5f;
    //ブレーキ時の横方向の減衰力
    private const float BRAKE_SIDE_ATTENUATION = 0.5f; 

    [SerializeField] private Rigidbody _rigidbody = default;
    [SerializeField] private PlayerStatus _playerStatus = default;
    [SerializeField] private VectorControlWhenHitWall _vectorControlHitWall = default;

    private Vector2 _playerMoveValue = Vector2.zero;
    private Vector2 _inputDirection = Vector2.zero;
    private bool _isPlayerBrake = false;

    public void PlayerMoveInput(Vector2 moveInput)
    {
        _playerMoveValue = moveInput;
    }

    public void PlayerSouthButtonInput(bool southButtonInput)
    {
        _isPlayerBrake = southButtonInput;
    }

    public void UpdatePlayerMove()
    {
        InputMoveControl();
        //PlayerStateにSpeedの値を渡す
        PassVelocityPlayer();
    }

    public void FixedUpdatePlayerMove()
    {
        if (!_playerStatus.CanMove)
        {
            return;
        }
        PhysicsMoveControl();
    }

    private void InputMoveControl()
    {
        _inputDirection = new Vector3(_playerMoveValue.x, _playerMoveValue.y, 0.0f);
    }

    //プレイヤーの移動制御
    private void PhysicsMoveControl()
    {
        Vector3 horizontalVelocity = _rigidbody.linearVelocity;
        horizontalVelocity.y = 0;

        //前方向の力
        if (horizontalVelocity.magnitude < MAX_RUN_SPEED)
        {
            if (!_isPlayerBrake)
            {
                _rigidbody.AddForce(transform.forward * ACCELERATION_FORCE, ForceMode.Acceleration);
            }
        }

        //プレイヤーの向き
        //ハンドルタイプ
        //_rigidbody.MoveRotation(Quaternion.Euler(new Vector3(0, _inputDirection.x * HANDLE_ROTATE_SPEED, 0)) * _rigidbody.rotation);

        // スティック方向タイプ
        if (_inputDirection.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(_inputDirection.x, _inputDirection.y) * Mathf.Rad2Deg + CORRECTION_ANGLE;
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            Quaternion newRotation = Quaternion.RotateTowards(_rigidbody.rotation,targetRotation,STICK_ROTATE_SPEED * Time.fixedDeltaTime);

            _rigidbody.MoveRotation(newRotation);
        }

        //前に進むスピードを取得
        Vector3 forwardVelocity = transform.forward * Vector3.Dot(_rigidbody.linearVelocity, transform.forward);
        //横に進むスピードを取得
        Vector3 sideVelocity = transform.right * Vector3.Dot(_rigidbody.linearVelocity, transform.right);

        //通常時とドリフト時で減衰のさせ方を変える
        if (_isPlayerBrake)
        {
            forwardVelocity = forwardVelocity * Mathf.Exp(-BRAKE_FORWARD_ATTENUATION * Time.fixedDeltaTime);
            sideVelocity = sideVelocity * Mathf.Exp(-BRAKE_SIDE_ATTENUATION * Time.fixedDeltaTime);
        }
        else
        {
            forwardVelocity = forwardVelocity * Mathf.Exp(-BASE_FORWARD_ATTENUATION * Time.fixedDeltaTime);
            sideVelocity = sideVelocity * Mathf.Exp(-BASE_SIDE_ATTENUATION * Time.fixedDeltaTime);
        }
        //前方向の力と横方向の力を合成する
        Vector3 moveVelocity = forwardVelocity + sideVelocity;

        //壁方向の移動量をなくす
        Vector3 moveDirection = _vectorControlHitWall.AdjustDirection(moveVelocity);

        _rigidbody.linearVelocity = new Vector3(moveDirection.x, _rigidbody.linearVelocity.y, moveDirection.z);
    }

    private void PassVelocityPlayer()
    {
        _playerStatus.UpdateRunSpeed(_rigidbody.linearVelocity.magnitude);
    }
}
