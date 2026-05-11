using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private const string MOVE = "Move";
    private const string LOOK = "Look";
    private const string CROUCH = "Crouch";
    private const string JUMP = "Jump";
    private const string DASH = "Sprint";
    private const string CLICK = "Click";
    private const string WHEEL = "Wheel";
    
    private const float MOUSE_SCROLL_ADJUSTMENT = 0.5f;

    [SerializeField] private UnityEngine.InputSystem.PlayerInput _playerInput = default;
    private PlayerController _playerController = default;

    private Vector2 _moveInputValue = default;
    private Vector2 _mouseMoveInputValue = default;
    private Vector2 _mouseScrollValue = Vector2.zero;
    private bool _isDashButtonPressed = false;  
    private bool _isCrouchButtonPressed = false;
    private bool _isPlayerDead = false;

    private void OnEnable()
    {
        if (_playerInput == null)
        {
            return;
        }

        _playerInput.onActionTriggered += OnMove;
        _playerInput.onActionTriggered += OnLook;
        _playerInput.onActionTriggered += OnJump;
        _playerInput.onActionTriggered += OnDash;
        _playerInput.onActionTriggered += OnCrouch;
        _playerInput.onActionTriggered += OnClick;
        _playerInput.onActionTriggered += OnScroll;
    }

    private void OnDisable()
    {
        if (_playerInput == null)
        {
            return;
        }

        _playerInput.onActionTriggered -= OnMove;
        _playerInput.onActionTriggered -= OnLook;
        _playerInput.onActionTriggered -= OnJump;
        _playerInput.onActionTriggered -= OnDash;
        _playerInput.onActionTriggered -= OnCrouch;
        _playerInput.onActionTriggered -= OnClick;
        _playerInput.onActionTriggered -= OnScroll;
    }

    public void PlayerInputStart(PlayerController playerController)
    {
        _playerController = playerController;
    }

    public void PlayerInputUpdate()
    {
        if (_isPlayerDead)
        {
            return;
        }

        _playerController.UpdateMoveInput(_moveInputValue.x, _moveInputValue.y);
        _playerController.UpdateMouseInput(_mouseMoveInputValue.x, _mouseMoveInputValue.y);
        _playerController.DashController(_isDashButtonPressed);
        _playerController.CrouchController(_isCrouchButtonPressed);
    }

    public void PlayerDeadController()
    {
        _isPlayerDead = true;
        if (_playerInput == null)
        {
            return;
        }

        _playerController.UpdateMoveInput(0.0f, 0.0f);
        _playerInput.onActionTriggered -= OnMove;
        _playerInput.onActionTriggered -= OnLook;
        _playerInput.onActionTriggered -= OnJump;
        _playerInput.onActionTriggered -= OnDash;
        _playerInput.onActionTriggered -= OnCrouch;
        _playerInput.onActionTriggered -= OnClick;
        _playerInput.onActionTriggered -= OnScroll;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if(context.action.name != MOVE)
        {
            return;
        }

        _moveInputValue = context.ReadValue<Vector2>();
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        if (context.action.name != LOOK)
        {
            return;
        }

        _mouseMoveInputValue = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.action.name != JUMP)
        {
            return;
        }

        if (context.started)
        {
            _playerController.JumpController();
        }
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.action.name != DASH)
        {
            return;
        }

        if (context.started)
        {
            _isDashButtonPressed = true;
        }
        if (context.canceled)
        {
            _isDashButtonPressed = false;
        }
    }   
    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.action.name != CROUCH)
        {
            return;
        }

        if (context.started)
        {
            _isCrouchButtonPressed = true;
        }
        if (context.canceled)
        {
            _isCrouchButtonPressed = false;
        }
    }
    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.action.name != CLICK)
        {
            return;
        }

        if (context.started)
        {
            _playerController.GrabController(true);
        }
        if (context.canceled)
        {
            _playerController.GrabController(false);
        }
    }
    /// <summary>
    /// マウスホイールを一回ノッチすると、
    /// (1.1.0)、(-1.-1.0)になってしまうので0.5をかけてあげることで、
    /// 一回のノッチで±１の値になる
    /// </summary>
    /// <param name="context"></param>
    public void OnScroll(InputAction.CallbackContext context)
    {
        if (context.action.name != WHEEL)
        {
            return;
        }

        _mouseScrollValue = context.ReadValue<Vector2>();
        float scrollValue = _mouseScrollValue.y * MOUSE_SCROLL_ADJUSTMENT;
        _playerController.GrabDistanceController(scrollValue);
    }
}
