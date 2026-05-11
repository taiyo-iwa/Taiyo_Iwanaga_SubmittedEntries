using UnityEngine;

public class EnemyPlayerCheck : MonoBehaviour
{
    [SerializeField] LayerMask _playerLayerMask;

    private readonly Vector3 BOX_CHECK_SIZE = new Vector3(3.0f, 4.0f, 3.5f);
    private readonly Vector3 BOX_POSITION_OFFSET = new Vector3(0.0f, -1.0f, 1.5f);
    
    public bool IsPlayerCheckEnemy()
    {
        Vector3 origin = transform.position + BOX_POSITION_OFFSET;
        Collider[] hitPlayers = Physics.OverlapBox(origin, BOX_CHECK_SIZE, transform.rotation, _playerLayerMask);
        if(hitPlayers.Length > 0)
        {
            return true;
        }
        return false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 origin = transform.position + BOX_POSITION_OFFSET;
        Gizmos.color = IsPlayerCheckEnemy() ? Color.green : Color.red;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(origin, transform.rotation, Vector3.one);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, BOX_CHECK_SIZE);
    }
#endif
}
