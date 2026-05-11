using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemyPlayerSearch : MonoBehaviour
{
    private const float VIEW_RANGE_DISTANCE = 10.0f;
    private const float VALID_FIELD_OF_VIEW = 120.0f;

    [SerializeField] private LayerMask _playerLayerMask = default;

    Vector3 _playerDirection = Vector3.zero;
    private bool _isPlayerFound = false;

    public void EnemyPlayerSearchUpdate()
    {
        EnemyViewRange();
    }

    public bool IsEnemyPlayerDiscovery()
    {
        return _isPlayerFound;
    }

    private void EnemyViewRange()
    {
        Vector3 origin = transform.position;
        origin.y = transform.position.y / 2;
        Collider[] hitColliders = Physics.OverlapSphere(origin, VIEW_RANGE_DISTANCE, _playerLayerMask);
        foreach (Collider collider in hitColliders)
        {
            if (collider != null)
            {
                InPlayerFieldOfView(collider.gameObject.transform);
            }
            else
            {
                _isPlayerFound = false;
            }
        }
    }

    private void InPlayerFieldOfView(Transform playerTransform)
    {
        Vector3 playerDirection = (playerTransform.position - transform.position).normalized;
        _playerDirection = playerDirection;
        if (Vector3.Angle(transform.forward, playerDirection) < VALID_FIELD_OF_VIEW / 2)
        {
            EnemyEyeLine(playerDirection);
        }
    }

    private void EnemyEyeLine(Vector3 playerDirection)
    {
        if (Physics.Raycast(transform.position, playerDirection, VIEW_RANGE_DISTANCE, _playerLayerMask))
        {
            _isPlayerFound = true;
        }
        else
        {
            _isPlayerFound = false;
        }
    }

//#if UNITY_EDITOR
//    private void OnDrawGizmos()
//    {
//        Vector3 origin = transform.position;
//        origin.y = transform.position.y / 2;
//        Gizmos.color = Color.green;
//        Gizmos.DrawWireSphere(origin, VIEW_RANGE_DISTANCE);

//        Handles.color = Color.blue;
//        ³–Ê•ûŒü
//       Vector3 forward = transform.forward;
//        îŒ^‚ð•`‚­i’†S“G‚ÌˆÊ’uj
//        Handles.DrawSolidArc(
//            transform.position,
//            Vector3.up,                   // ‰ñ“]Ž²iyŽ² = …•½‚ÉîŒ^‚ð•`‚«‚½‚¢j
//            Quaternion.Euler(0, -VALID_FIELD_OF_VIEW / 2, 0) * forward,  // ¶’[‚ÌŠp“x
//            VALID_FIELD_OF_VIEW,                    // ‰~ŒÊ‚Ì‘å‚«‚³
//            VIEW_RANGE_DISTANCE                  // î‚Ì”¼Œa
//        );

//        Gizmos.color = Color.red;
//        Vector3 eyeLine = transform.position;
//        eyeLine.y = eyeLine.y + eyeLine.y / 2;
//        Gizmos.DrawRay(eyeLine, _playerDirection * VIEW_RANGE_DISTANCE);
//    }
//#endif
}
