using UnityEngine;
using UnityEngine.AI;

public class EnemyPlayerChase : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent = default;
    public void EnemyPlayerChaseStart(NavMeshAgent navMeshAgen)
    {
        _navMeshAgent = navMeshAgen;
    }
    public void EnemyPlayerChaseUpdate(Vector3 playerPosition)
    {
        _navMeshAgent.SetDestination(playerPosition);
    }
}
