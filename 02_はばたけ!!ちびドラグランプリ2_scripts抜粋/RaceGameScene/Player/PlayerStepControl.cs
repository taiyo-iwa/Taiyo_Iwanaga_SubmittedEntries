using UnityEngine;

public class PlayerStepControl : MonoBehaviour
{
    //プレイヤーを地面から浮かせる距離
    private const float FLOATING_DISTANCE = 0.5f;
    //プレイヤーを上に押し上げる力
    private const float LIFTING_FORCE = 30.0f;
    //上下に動きすぎないように減衰させる量
    private const float FORCE_TO_ATTENUATE = 4.0f;

    private Vector3 ORIGIN_ADDITION = new Vector3(0.0f, -0.5f, 0.0f);

    [SerializeField] private LayerMask _groundLayerMask = default;
    [SerializeField] private Rigidbody _rigidbody = default;
 
    public void StepSolution()
    {
        Vector3 origin = transform.position + ORIGIN_ADDITION;
        if(Physics.Raycast(origin, -(transform.up), out RaycastHit hit, FLOATING_DISTANCE, _groundLayerMask))
        {
            _rigidbody.AddForce(new Vector3(0, (1.0f - (hit.distance / FLOATING_DISTANCE)) * LIFTING_FORCE - (_rigidbody.linearVelocity.y * FORCE_TO_ATTENUATE), 0), ForceMode.Acceleration);
        }
        Debug.DrawRay(origin, -(transform.up) * FLOATING_DISTANCE, Color.red);
    }
}
