using UnityEngine;
using UnityEngine.AI;

public enum EnemyStatus
{
    Idle,
    Patrol,
    Chase,
    Lost,
}

public class EnemyController : MonoBehaviour
{
    private const float TRACKING_CANCELLATION_TIME = 8.0f;

    [SerializeField] private NavMeshAgent _navMeshAgent = default;
    [SerializeField] private Animator _enemyAnimator = default;
    [SerializeField] private Transform _playerTransform = default;
    [SerializeField] private EnemyPlayerSearch _enemyPlayerSearch = default;
    [SerializeField] private EnemyPlayerPatrol _enemyPlayerPatrol = default;
    [SerializeField] private EnemyPlayerChase _enemyPlayerChase = default;
    [SerializeField] private EnemyPlayerAttack _enemyPlayerAttack = default;

    private EnemyStatus _currentState = EnemyStatus.Patrol;
    private float _isPlayerLostTimer = 0;
    private bool _isPlayerFound = false;

    private void Start()
    {
        _enemyPlayerPatrol.EnemyPlayerPatrolStart(_navMeshAgent, _playerTransform, _enemyAnimator);
        _enemyPlayerChase.EnemyPlayerChaseStart(_navMeshAgent);
        _enemyPlayerAttack.EnemyPlayerAttackStart(_enemyAnimator);
    }

    private void Update()
    {
        EnemyStatusChangeManagement();
        _enemyPlayerSearch.EnemyPlayerSearchUpdate();        

        switch (_currentState)
        {
            case EnemyStatus.Idle:
                break;
            case EnemyStatus.Patrol:
                _enemyPlayerPatrol.EnemyPlayerPatrolUpdate();
                break;
            case EnemyStatus.Chase:
                _enemyPlayerChase.EnemyPlayerChaseUpdate(_playerTransform.position);
                _enemyPlayerAttack.PlayerAttack();
                break;
            case EnemyStatus.Lost:
                break;
        }    
    }

    private void EnemyStatusChangeManagement()
    {
        if (_enemyPlayerSearch.IsEnemyPlayerDiscovery())
        {
            _isPlayerLostTimer = TRACKING_CANCELLATION_TIME;
            _isPlayerFound = true;
            ChangeState(EnemyStatus.Chase);
        }
        if (_isPlayerFound)
        {
            _isPlayerLostTimer -= Time.deltaTime;
            if(_isPlayerLostTimer <= 0)
            {
                _isPlayerLostTimer = 0.0f;
                _isPlayerFound = false;
                ChangeState(EnemyStatus.Patrol);
            }
        }
    }

    private void ChangeState(EnemyStatus nextState)
    {
        if (_currentState == nextState)
        {
            return;
        }
        // 必要なら状態終了時の処理
        ExitState(_currentState);
        _currentState = nextState;

        // 状態開始時の処理
        EnterState(_currentState);
    }

    private void EnterState(EnemyStatus state)
    {
        switch (state)
        {
            case EnemyStatus.Idle:
                break;
            case EnemyStatus.Patrol:
                _navMeshAgent.speed = 3.5f;
                break;
            case EnemyStatus.Chase:
                _navMeshAgent.speed = 6.0f;
                break;
            case EnemyStatus.Lost:
                break;
        }
    }
    private void ExitState(EnemyStatus state)
    {
        switch (state)
        {
            case EnemyStatus.Idle:
                break;
            case EnemyStatus.Patrol:
                break;
            case EnemyStatus.Chase:
                break;
            case EnemyStatus.Lost:
                break;
        }
    }
}
