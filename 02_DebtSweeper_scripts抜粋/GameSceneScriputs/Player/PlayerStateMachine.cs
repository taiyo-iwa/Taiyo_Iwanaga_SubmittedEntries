using UnityEngine;

public enum PlayerState
{
    Idle,
    Walk,
    Run,
    Crouch,
    Jump,
    Stun,
    Death,
}

public class PlayerStateMachine : MonoBehaviour
{
    PlayerInput _playerInput;
    PlayerStateTracker _playerStateTracker;

    [SerializeField] private PlayerState _currentState = PlayerState.Idle;

    public PlayerState PlayerState => _currentState;

    private void Update()
    {
        PlayerStateManagement();
        PlayerStateChangeManagement();
    }

    public void PlayerStateMachineStart(PlayerInput playerInput, PlayerStateTracker playerStateTracker)
    {
        _playerInput = playerInput;
        _playerStateTracker = playerStateTracker;
    }

    private void ChangeState(PlayerState nextState)
    {
        if (_currentState == nextState) return;
        // 必要なら状態終了時の処理
        ExitState(_currentState);
        _currentState = nextState;
        // 状態開始時の処理
        EnterState(_currentState);
    }
    
    /// <summary>
    /// ステート遷移時の処理
    /// </summary>
    /// <param name="state"></param>
    private void EnterState(PlayerState state)
    {
        
    }

    /// <summary>
    /// 状態終了時のクリーンアップなど
    /// </summary>
    /// <param name="state"></param>
    private void ExitState(PlayerState state)
    {
        
    }

    private void PlayerStateManagement()
    {
        switch (_currentState)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Walk:
                break;
            case PlayerState.Run:
                break;
            case PlayerState.Crouch:
                break;
            case PlayerState.Jump:
                break;
            case PlayerState.Stun:
                break;
            case PlayerState.Death:
                _playerInput.PlayerDeadController();
                break;   
        }
    }

    private void PlayerStateChangeManagement()
    {
        if (!_playerStateTracker.IsGroundedPlayer())
        {
            ChangeState(PlayerState.Jump);
        }
        else if (_playerStateTracker.IsCrouchPlayer())
        {
            ChangeState(PlayerState.Crouch);
        }
        else if (_playerStateTracker.IsRunningPlayer())
        {
            ChangeState(PlayerState.Run);
        }
        else if (_playerStateTracker.IsWalkingPlayer())
        {
            ChangeState(PlayerState.Walk);
        }
        else
        {
            ChangeState(PlayerState.Idle);
        }
        if (_playerStateTracker.IsDamegePlayer())
        {
            ChangeState(PlayerState.Stun);
        }
        if (_playerStateTracker.IsDeathPlayer())
        {
            ChangeState(PlayerState.Death);
        } 
    }
}
