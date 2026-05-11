using UnityEngine;

public class EnemyAnimationEvent : MonoBehaviour
{
    private EnemyPlayerCheck _enemyPlayerCheck = default;
    private PlayerHitPoint _playerHitPoint;
    private int _enemyAttackValue = 0;
    private bool _isAttackEvent = false;

    public void EnemyAnimationEventStart(EnemyPlayerCheck enemyPlayerCheck, PlayerHitPoint playerHitPoint, int enemyAttackValue)
    {
        _enemyPlayerCheck = enemyPlayerCheck;
        _playerHitPoint = playerHitPoint;
        _enemyAttackValue = enemyAttackValue;
    }

    public void EnemyAttackEventStart()
    {
        if (_enemyPlayerCheck.IsPlayerCheckEnemy())
        {
            _isAttackEvent = true;
            _playerHitPoint.PlayerDamage(_enemyAttackValue);
        }
    }

    public void EnemyAttackEventEnd()
    {
        _isAttackEvent = false;
    }

    public bool IsAttackEvent()
    {
        return _isAttackEvent;
    }
}
