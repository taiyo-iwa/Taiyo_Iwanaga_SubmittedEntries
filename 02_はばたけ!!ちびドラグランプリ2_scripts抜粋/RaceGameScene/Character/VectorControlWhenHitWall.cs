using UnityEngine;

public class VectorControlWhenHitWall : MonoBehaviour
{
    [SerializeField] LayerMask _WallLayerMask;

    private const float SPHERE_SIZE = 1.0f;
    private const float MAX_CHECK_DISTANCE = 1.5f;

    public Vector3 AdjustDirection(Vector3 moveDir)
    {
        (bool isHitWall, Vector3 normal) = IsHitWallPlayer(moveDir);

        if (!isHitWall)
        {
            return moveDir;
        }

        if (Vector3.Dot(moveDir, normal) < 0)
        {
            return moveDir - Vector3.Project(moveDir, normal);
        }

        return moveDir;
    }

    private (bool, Vector3) IsHitWallPlayer(Vector3 moveDirection)
    {
        bool isStageCheck = Physics.SphereCast(transform.position, SPHERE_SIZE * 0.5f, moveDirection, out RaycastHit hit, MAX_CHECK_DISTANCE, _WallLayerMask);

        return (isStageCheck, hit.normal);
    }
}
