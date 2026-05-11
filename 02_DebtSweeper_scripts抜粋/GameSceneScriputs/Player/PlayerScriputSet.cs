using UnityEngine;

public class PlayerScriputSet : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody = default;
    [SerializeField] private Camera _playerCamera = default;
    [SerializeField] private Camera _playerSubCamera = default;

    [SerializeField] private PlayerInput _playerInput = default;
    [SerializeField] private PlayerController _playerController = default;
    [SerializeField] private PlayerMove _playerMove = default;
    [SerializeField] private PlayerJump _playerJump = default;
    [SerializeField] private PlayerObjectGrab _playerObjectGrab = default;
    [SerializeField] private PlayerCrouch _playerCrouch = default;
    [SerializeField] private PlayerStamina _playerStamina = default;
    [SerializeField] private PlayerGroundCheck _playerGroundCheck = default;
    [SerializeField] private PlayerStaminaText _playerStaminaText = default;
    [SerializeField] private PlayerStateMachine _playerStateMachine = default;
    [SerializeField] private PlayerStateTracker _playerStateTracker = default;
    [SerializeField] private PlayerDeath _playerDeath;
    [SerializeField] private PlayerMousePointerChange _mousePointerChange = default;
    [SerializeField] private PlayerGrabLineVisuali _lineVisuali = default;
    [SerializeField] private SceneChangeController _sceneChangeController;
    [SerializeField] private PlayerHitPoint _playerHitPoint = default;
    [SerializeField] private PlayerHitPointText _playerHitPointText = default;
    [SerializeField] private PlayerAudioController _audioController = default;

    private void Awake()
    {
        
    }

    private void Start()
    {
        _playerInput.PlayerInputStart(_playerController);
        _playerController.PlayerControllerStart(_playerMove, _playerJump, _playerObjectGrab, _playerCrouch);
        _playerMove.PlayerMoveStart(_playerStateTracker, _playerStamina, _rigidbody);
        _playerJump.PlayerJumpStart(_playerGroundCheck, _rigidbody);
        _playerStamina.PlayerStaminaStart(_playerStaminaText);
        _playerStateMachine.PlayerStateMachineStart(_playerInput, _playerStateTracker);
        _playerStateTracker.PlayerStateTrackerStart(_playerMove, _playerGroundCheck, _playerDeath);
        _playerObjectGrab.PlayerObjectGrabStart(_mousePointerChange, _lineVisuali, _playerCamera);
        _playerDeath.PlayerDeathStart(_sceneChangeController, _playerCamera, _playerSubCamera);
        _playerHitPoint.PlauerHitPointStart(_playerDeath, _playerHitPointText);
    }

    private void Update()
    {
        _playerInput.PlayerInputUpdate();
        _audioController.PlayerAudioControllerUpdate();
    }

    private void LateUpdate()
    {
        
    }
}
