using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    [SerializeField] LayerMask _groundLayerMask;

    private readonly Vector3 BOX_CHECK_SIZE = new Vector3(1.0f, 0.1f, 1.0f);
    private const float BOX_CHECK_DISTANCE = 1.0f;

    public bool GroundCheckPlayer()
    {
        bool isGroundCheck = Physics.BoxCast(transform.position, BOX_CHECK_SIZE * 0.5f, Vector3.down, Quaternion.identity, BOX_CHECK_DISTANCE, _groundLayerMask);

        return isGroundCheck;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = GroundCheckPlayer() ? Color.green : Color.red;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position + Vector3.down * BOX_CHECK_DISTANCE, Quaternion.identity, Vector3.one);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, BOX_CHECK_SIZE);
    }
#endif
}
