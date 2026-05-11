using UnityEngine;

public class PlayerStateTracker : MonoBehaviour
{
    private PlayerGroundCheck _playerGroundCheck;
    private PlayerDeath _playerDeath;
    [SerializeField] EnemyAnimationEvent _enemyAnimationEvent;

    private bool _isRunningPlayer = false;
    private bool _isWalkingPlayer = false;
    private bool _isCrouchPlayer = false;

    public void PlayerStateTrackerStart(PlayerMove playerMove, PlayerGroundCheck playerGroundCheck, PlayerDeath playerDeath)
    {
        _playerGroundCheck = playerGroundCheck;
        _playerDeath = playerDeath;
    }

    public bool IsGroundedPlayer()
    {
        bool isGrounded = _playerGroundCheck.GroundCheckPlayer();

        return isGrounded;
    }

    /// <summary>
    /// IsRunningPlayerに値を仲介するメソッド
    /// </summary>
    /// <param name="isRunning"></param>
    public void RunStatePlayer(bool isRunning)
    {
        _isRunningPlayer = isRunning;
    }
    /// <summary>
    /// IsWalkingPlayerに値を仲介するメソッド
    /// </summary>
    /// <param name="isWalking"></param>
    public void WalkStatePlayer(bool isWalking)
    {
        _isWalkingPlayer = isWalking;
    }
    public void CrouchStatePlayer(bool isCrouch)
    {
        _isCrouchPlayer = isCrouch;
    }

    /// <summary>
    /// True＝走っている、false＝歩いているまたは止まっている
    /// </summary>
    /// <returns></returns>
    public bool IsRunningPlayer()
    {
        return _isRunningPlayer;
    }
    /// <summary>
    /// True＝歩いている、false＝止まっている
    /// </summary>
    /// <returns></returns>
    public bool IsWalkingPlayer()
    {
        return _isWalkingPlayer;
    }
    public bool IsCrouchPlayer()
    {
        return _isCrouchPlayer;
    }

    public bool IsDeathPlayer()
    {
        bool isDeath = _playerDeath.IsPlayerDead();

        return isDeath;
    }

    public bool IsDamegePlayer()
    {
        bool isDamege = _enemyAnimationEvent.IsAttackEvent();

        return isDamege;
    }
}
