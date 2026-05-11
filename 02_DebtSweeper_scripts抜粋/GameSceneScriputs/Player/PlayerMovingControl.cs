using UnityEngine;

public class PlayerMovingControl : MonoBehaviour
{
    [SerializeField] LayerMask _StageLayerMask;

    private readonly Vector3 BOX_CHECK_SIZE = new Vector3(1.0f, 2.0f, 0.1f);
    private const float BOX_CHECK_DISTANCE = 0.5f;
    private Vector3 _moveDirection = new Vector3();
    private bool _isHitWall = false;
    
    public (bool, Vector3) IsHitWallPlayer(Vector3 moveDirection)
    {
        bool isStageCheck = Physics.BoxCast(transform.position, BOX_CHECK_SIZE * 0.5f, moveDirection, out RaycastHit hit, Quaternion.LookRotation(moveDirection), BOX_CHECK_DISTANCE, _StageLayerMask);

        _moveDirection = moveDirection;
        _isHitWall = isStageCheck;
        return (isStageCheck, hit.normal);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(_moveDirection .sqrMagnitude < 0.1f)
        {
            return;
        }

        Gizmos.color = _isHitWall ? Color.green : Color.red;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position + _moveDirection * BOX_CHECK_DISTANCE, Quaternion.LookRotation(_moveDirection), Vector3.one);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, BOX_CHECK_SIZE);
    }
#endif
}
