using UnityEngine;

public class EnemyPlayerAttack : MonoBehaviour
{
    private const float ATTACK_COOLTIME = 2.0f;
    private const int ENEMY_ATTACK_VALUE = 35;

    [SerializeField] EnemyAnimationEvent _enemyAnimationEvent;
    [SerializeField] EnemyPlayerCheck _enemyPlayerCheck;
    [SerializeField] PlayerHitPoint _playerHitPoint;
    private Animator _enemyAnimator = default;
    private float _attackCoolTimer = 0.0f;

    public void EnemyPlayerAttackStart(Animator enemyAnimator)
    {
        _enemyAnimator = enemyAnimator;
        _enemyAnimationEvent.EnemyAnimationEventStart(_enemyPlayerCheck, _playerHitPoint, ENEMY_ATTACK_VALUE);
    }

    public void PlayerAttack()
    {
        if (_enemyPlayerCheck.IsPlayerCheckEnemy())
        {
            if (_attackCoolTimer <= 0.0f)
            {
                _attackCoolTimer = ATTACK_COOLTIME;
                _enemyAnimator.SetTrigger("Attack");
            }
        }
        if (_attackCoolTimer > 0.0f)
        {
            _attackCoolTimer -= Time.deltaTime;
            if (_attackCoolTimer <= 0)
            {
                _attackCoolTimer = 0.0f;
            }
        }
    }
}
