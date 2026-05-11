using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private const float MOVE_THRESHOLD = 0.5f;
    private const float PLAYER_MOVE_SPEED = 8.0f;
    private const float PLAYER_DASH_SPEED = 12.0f;
    private const float CROUCHING_MOVE_SPEED = 5.0f;
    private const float CROUCH_DOWN_SPEED = 5.0f;
    private const int PLAYER_MAX_STAMINA = 40;
    private const float STAMINA_DECREASE_SPEED = 15.0f;
    private const float STAMINA_RECOVERY_SPEED = 0.8f;
    private const float CROUCH_STAMINA_HEAL_SPEED = 2.5f;
    private const float MAX_CAMERA_VERTICAL_ANGLE = 80.0f;
    private const float MIN_CAMERA_VERTICAL_ANGLE = -80.0f;

    [SerializeField] private Transform _cameraTransform = default;
    [SerializeField] private PlayerMovingControl _playerMovingControl = default;

    private Rigidbody _rigidbody = default;
    private PlayerStateTracker _playerStateTracker = default;
    private PlayerStamina _playerStamina = default;

    private Vector3 _inputVector = Vector3.zero;
    private Vector3 _basicCameraPosition = new Vector3(0.0f, 1.0f, 0.0f);
    private Vector3 _crouchCameraPosition = new Vector3(0.0f, -0.3f, 0.0f);
    private float _dashSpeedAddend = 0.0f;
    private float _currentStamina = PLAYER_MAX_STAMINA;
    private float _mouseSensitivity = 0.25f;
    private float _cameraVerticalAngle = 0.0f;
    private float _crouchTimer = 0.0f;
    private bool _isDashPlayer = false;
    private bool _isCrouchPlayer = false;

    //マウスをロックする
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        DashSpeedController();
        CrouchTimer();
        _playerStamina.PlayerStaminaController(_currentStamina, PLAYER_MAX_STAMINA);
    }

    private void FixedUpdate()
    {
        MoveCharacter();
        MoveStatePlayer();
    }

    public void PlayerMoveStart(PlayerStateTracker playerStateTracker, PlayerStamina playerStamina, Rigidbody rigidbody)
    {
        _playerStateTracker = playerStateTracker;
        _playerStamina = playerStamina;
        _rigidbody = rigidbody;
    }

    public void MoveInput(float horizontal, float vertical)
    {
        _inputVector = new Vector3(horizontal, 0.0f, vertical).normalized;
    }
    public void MouseInput(float mouseHorizontal, float mouseVertical)
    {
        ConvertCameraOrientation(mouseHorizontal, mouseVertical);
    }

    /// <summary>
    /// ダッシュパラメータを変えやすいように、通常のスピードに値を加えていく方式に
    /// ダッシュボタンを押していなっかたらゼロ。
    /// </summary>
    /// <param name="isDashButtonPressed"></param>
    public void DashInput(bool isDashButtonPressed)
    {
        if (isDashButtonPressed)
        {
            if (_inputVector.sqrMagnitude > MOVE_THRESHOLD)
            {
                _isDashPlayer = true;
                _currentStamina -= STAMINA_DECREASE_SPEED * Time.deltaTime;
                _currentStamina = Mathf.Clamp(_currentStamina, 0.0f, PLAYER_MAX_STAMINA);
            }
            if (_currentStamina <= 0.0f)
            {
                _isDashPlayer = false;
            }
        }
        else
        {
            _isDashPlayer = false;
            StaminaHealController();
        }
    }

    /// <summary>
    /// 通常時としゃがみ時でスタミナの回復スピードを変える
    /// </summary>
    private void StaminaHealController()
    {
        if (_isCrouchPlayer)
        {
            _currentStamina += CROUCH_STAMINA_HEAL_SPEED * Time.deltaTime;
            _currentStamina = Mathf.Clamp(_currentStamina, 0.0f, PLAYER_MAX_STAMINA);
        }
        else
        {
            _currentStamina += STAMINA_RECOVERY_SPEED * Time.deltaTime;
            _currentStamina = Mathf.Clamp(_currentStamina, 0.0f, PLAYER_MAX_STAMINA);
        }
    }

    public void CrouchInput(bool isCrouchButtonPressed)
    {
        if (isCrouchButtonPressed)
        {
            _isCrouchPlayer = true;
            _cameraTransform.localPosition = Vector3.Lerp(_basicCameraPosition, _crouchCameraPosition, _crouchTimer);     
        }
        else
        {
            _isCrouchPlayer = false;
            //カメラの位置を戻す時はタイマーの値をマイナスする
            _cameraTransform.localPosition = Vector3.Lerp(_basicCameraPosition, _crouchCameraPosition, _crouchTimer);
        }
    }

    private void CrouchTimer()
    {
        if (_isCrouchPlayer)
        {
            _crouchTimer += CROUCH_DOWN_SPEED * Time.deltaTime;
            if (_crouchTimer > 1.0f)
            {
                _crouchTimer = 1.0f;
            }
        }
        else
        {
            _crouchTimer -= CROUCH_DOWN_SPEED * Time.deltaTime;
            if (_crouchTimer < 0.0f)
            {
                _crouchTimer = 0.0f;
            }
        }
    }

    private void DashSpeedController()
    {
        if (_isDashPlayer)
        {
            _dashSpeedAddend = PLAYER_DASH_SPEED;
        }
        else
        {
            if (_isCrouchPlayer)
            {
                //通常のスピードに加算する方式なので、マイナスの値を入れてあげることでスピードを遅くする
                _dashSpeedAddend = -(CROUCHING_MOVE_SPEED);
            }
            else
            {
                //ダッシュの加算値は通常時(ダッシュ、しゃがみではない)は何も加算しない
                _dashSpeedAddend = 0.0f;
            }           
        }
    }

    private void MoveCharacter()
    {
        if (_inputVector.sqrMagnitude < MOVE_THRESHOLD)
        {
            return;
        }

        Vector3 moveDirection = ChangeToOrientationCamera(_inputVector);

        float totalSpeed = PLAYER_MOVE_SPEED + _dashSpeedAddend;

        Vector3 moveValue = MovingControl(moveDirection, totalSpeed);

        _rigidbody.MovePosition(_rigidbody.position + moveValue);
    }

    private void ConvertCameraOrientation(float mouseHorizontal, float mouseVertical)
    {
        float mouseHorizontalValue = mouseHorizontal * _mouseSensitivity;
        float mouseVerticalValue = mouseVertical * _mouseSensitivity;

        transform.Rotate(Vector3.up * mouseHorizontalValue);

        _cameraVerticalAngle -= mouseVerticalValue;
        _cameraVerticalAngle = Mathf.Clamp(_cameraVerticalAngle, MIN_CAMERA_VERTICAL_ANGLE, MAX_CAMERA_VERTICAL_ANGLE);
        _cameraTransform.localRotation = Quaternion.Euler(_cameraVerticalAngle, 0.0f, 0.0f);
    }

    private Vector3 ChangeToOrientationCamera(Vector3 inputValue)
    {
        Vector3 moveDirection = _cameraTransform.right * inputValue.x +
                                _cameraTransform.forward * inputValue.z;
        moveDirection.y = 0.0f;
        moveDirection.Normalize();

        return moveDirection;
    }

    /// <summary>
    /// 壁方向に移動しようとした時その方向の移動量を減らす
    /// </summary>
    /// <param name="moveDirection"></param>
    /// <param name="totalSpeed"></param>
    /// <returns></returns>
    private Vector3 MovingControl(Vector3 moveDirection, float totalSpeed)
    {
        (bool isStageCheck, Vector3 wallNormal) = _playerMovingControl.IsHitWallPlayer(moveDirection);

        Vector3 moveValue;
        if (isStageCheck)
        {
            if (Vector3.Dot(moveDirection, wallNormal) < 0)
            {
                Vector3 limitDirection = moveDirection - Vector3.Project(moveDirection, wallNormal);
                moveValue = limitDirection * totalSpeed * Time.fixedDeltaTime;
            }
            else
            {
                moveValue = moveDirection * totalSpeed * Time.fixedDeltaTime;
            }
        }
        else
        {
            moveValue = moveDirection * totalSpeed * Time.fixedDeltaTime;
        }

        return moveValue;
    }

    /// <summary>
    /// 走っているか、歩いているか、止まっているか判断する
    /// 外部に今のステータスを渡すメゾッド
    /// </summary>
    private void MoveStatePlayer()
    {
        if (_isDashPlayer)
        {
            _playerStateTracker.RunStatePlayer(true);
        }
        else
        {
            _playerStateTracker.RunStatePlayer(false);
            if (_inputVector.sqrMagnitude > MOVE_THRESHOLD)
            {
                _playerStateTracker.WalkStatePlayer(true);
            }
            else
            {
                _playerStateTracker.WalkStatePlayer(false);
            }
        }
        if (_isCrouchPlayer)
        {
            _playerStateTracker.CrouchStatePlayer(true);
        }
        else
        {
            _playerStateTracker.CrouchStatePlayer(false);
        }
    }
}
