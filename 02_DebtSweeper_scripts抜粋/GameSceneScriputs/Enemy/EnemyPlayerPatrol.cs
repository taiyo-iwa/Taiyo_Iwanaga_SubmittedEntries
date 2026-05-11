using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

[System.Serializable]
public class WayPointDistance
{
    public Transform wayPoint;
    public float distance;
}

public class EnemyPlayerPatrol : MonoBehaviour
{
    private const int CANDIDATELIMIT = 3;
    private const int ACTION_PROBABILITY = 10;
    private const int FIRST_ACTION_PROBABILITY = 7;
    private const float RANGE_ADMISSIBLE_VALUE_DESTINATION = 0.5f;
    private const float AFTER_FIXED_TIME = 1.0f;
    private const string STATENAME = "NeckSwing";
    
    [SerializeField] private Transform waypointsParent = default;

    private NavMeshAgent _navMeshAgent = default;
    private Transform[] waypoints = default;
    private Transform _playerTransform = default;
    private Animator _enemyAnimator = default;
    private WayPointDistance _wayPointDistance = new WayPointDistance();
    private List<Vector3> remainingWaypoints = new List<Vector3>();
    private WayPointDistance[] playerNearOrderWayPointList = default;
    private int _nextWayPoint = 0;
    private bool _isNeckSwingTrigger = true;
    private bool _candrow = true;
    private bool _isEnemyMoving = true;

    public void EnemyPlayerPatrolStart(NavMeshAgent navMeshAgent, Transform playerTransform, Animator enemyAnimator)
    {
        _navMeshAgent = navMeshAgent;
        _playerTransform = playerTransform;
        _enemyAnimator = enemyAnimator;
        WayPointSetUp();
    }

    public void EnemyPlayerPatrolUpdate()
    {
        if (_navMeshAgent.remainingDistance >= RANGE_ADMISSIBLE_VALUE_DESTINATION + 2.0f)
        {
            _isEnemyMoving = true;
        }

        if (!_isEnemyMoving)
        {
            return;
        }
        if (_navMeshAgent.pathPending)
        {
            return;
        }
        if (_navMeshAgent.remainingDistance < RANGE_ADMISSIBLE_VALUE_DESTINATION)
        {
            _isEnemyMoving= false;
            DestinationArrivalProcess().Forget();
        }
    }
    
    public void WayPointSetUp()
    {
        int count = waypointsParent.childCount;
        waypoints = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            Transform wayPointChild = waypointsParent.GetChild(i);
            waypoints[i] = wayPointChild;
        }

        foreach(Transform waypoint in waypoints)
        {
            remainingWaypoints.Add(waypoint.position);
        }

        int actionNumber = 0;
        actionNumber = Random.Range(0, ACTION_PROBABILITY);
        if (actionNumber < FIRST_ACTION_PROBABILITY)
        {
            MoveToAllWayPoint();
        }
        else
        {
            PlayerNearGroupWayPointMove();
        }
    }

    //目的地に到着したら処理するメソッド
    private async UniTask DestinationArrivalProcess()
    {
        if (_isNeckSwingTrigger)
        {
            _isNeckSwingTrigger = false;
            _enemyAnimator.SetTrigger(STATENAME);
        }
        await ConfirmationWait(STATENAME);
        _isNeckSwingTrigger = true;
        int actionNumber = 0;
        actionNumber = Random.Range(0, ACTION_PROBABILITY);
        if(actionNumber < FIRST_ACTION_PROBABILITY)
        {
            MoveToAllWayPoint();
        }
        else
        {
            PlayerNearGroupWayPointMove();
        }
    }

    //目的地に到着したときに周りを確認するアニメーションを再生する
    private async UniTask ConfirmationWait(string stateName)
    {
        await UniTask.WaitUntil(() =>
        {
            return _enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
        });

        await UniTask.WaitUntil(() =>
        {
            return !_enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
        });
    }

    //プレイヤー近くのWayPointを選びランダムでその場所に行く
    private void PlayerNearGroupWayPointMove()
    {
        if (waypoints.Length == 0)
        {
            return;
        }
        playerNearOrderWayPointList = new WayPointDistance[waypoints.Length];
        for(int i = 0; i < waypoints.Length; i++)
        {
            WayPointDistance wayPointDistance =  new WayPointDistance();
            float distance = (waypoints[i].position - _playerTransform.position).sqrMagnitude;
            wayPointDistance.wayPoint = waypoints[i];
            wayPointDistance.distance = distance;
            playerNearOrderWayPointList[i] = wayPointDistance;
        }
        //昇順にソート
        for (int i = 0; i < playerNearOrderWayPointList.Length; i++)
        {
            for (int j = i + 1; j < playerNearOrderWayPointList.Length; j++)
            {
                if (playerNearOrderWayPointList[i].distance > playerNearOrderWayPointList[j].distance)
                {
                    WayPointDistance wayPointDistance = playerNearOrderWayPointList[i];
                    playerNearOrderWayPointList[i] = playerNearOrderWayPointList[j];
                    playerNearOrderWayPointList[j] = wayPointDistance;
                }
            }
        }

        if(playerNearOrderWayPointList.Length > CANDIDATELIMIT)
        {
            _nextWayPoint = Random.Range(0, CANDIDATELIMIT);
        }
        else
        {
            _nextWayPoint = Random.Range(0, playerNearOrderWayPointList.Length);
        }

        _navMeshAgent.SetDestination(playerNearOrderWayPointList[_nextWayPoint].wayPoint.position);
    }

    //訪れていないWayPointをランダムに移動する
    private void MoveToAllWayPoint()
    {
        if (remainingWaypoints.Count == 0)
        {
            foreach (Transform waypoint in waypoints)
            {
                remainingWaypoints.Add(waypoint.position);
            }
        }
        if (!_candrow)
        {
            return;
        }
        _candrow = false;
        int nextWaypointIndex = Random.Range(0, remainingWaypoints.Count);
        _navMeshAgent.SetDestination(remainingWaypoints[nextWaypointIndex]);
        remainingWaypoints[nextWaypointIndex] = remainingWaypoints[remainingWaypoints.Count - 1];
        remainingWaypoints.RemoveAt(remainingWaypoints.Count - 1);
        
        makeMoveMode();
    }

    private async void makeMoveMode()
    {
        await MakeMoveModeWait();
    }

    private async UniTask MakeMoveModeWait()
    {
        await UniTask.WaitForSeconds(AFTER_FIXED_TIME);

        _candrow = true;
    }
}
