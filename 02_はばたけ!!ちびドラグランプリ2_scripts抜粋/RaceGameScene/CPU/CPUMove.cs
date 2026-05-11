using SWS;
using UnityEngine;

public class CPUMove : MonoBehaviour
{
    //最高速度
    private const float MAX_RUN_SPEED = 20.0f;
    //加速力
    private const float ACCELERATION_FORCE = 10.0f;
    //通常時の前方向の減衰力
    private const float BASE_FORWARD_ATTENUATION = 1.0f;
    //通常時の横方向の減衰力
    private const float BASE_SIDE_ATTENUATION = 1.3f;
    //ブレーキ時の前方向の減衰力
    private const float BRAKE_FORWARD_ATTENUATION = 0.5f;
    //ブレーキ時の横方向の減衰力
    private const float BRAKE_SIDE_ATTENUATION = 0.5f;
    //方向を変えるスピード
    private const float ROTATE_SPEED = 180.0f;
    //Waypointのどこまで先を向かせるか
    private const int OFFSET_TARGET_POINT = 8;

    [SerializeField] private Rigidbody _rigidbody = default;
    [SerializeField] private CPUStatus _cpuStatus = default;
    [SerializeField] private BezierPathManager _bezierPathManager = default;
    [SerializeField] private VectorControlWhenHitWall _vectorControlHitWall = default;
    [SerializeField] private Transform _targetTransform = default;

    private int _nearestSegmentIndex = 0;
    private bool _isCPUBreke = false;
    private Vector3[] trackWaypoints = default;

    public void CPUBrakeInput(bool southButtonInput)
    {
        _isCPUBreke = southButtonInput;
    }

    public void StartCPUMove()
    {
        trackWaypoints = _bezierPathManager.pathPoints;
    }

    public void UpdateCPUMove()
    {
        UpdateRawDistance();
        PassVelocityCPU();
    }

    public void FixedUpdateCPUMove()
    {
        if (!_cpuStatus.CanMove)
        {
            return;
        }
        PhysicsMoveControl();
    }

    private void PhysicsMoveControl()
    {
        Vector3 horizontalVelocity = _rigidbody.linearVelocity;
        horizontalVelocity.y = 0;

        //前方向の力
        if (horizontalVelocity.magnitude < MAX_RUN_SPEED)
        {
            if (!_isCPUBreke)
            {
                _rigidbody.AddForce(transform.forward * ACCELERATION_FORCE, ForceMode.Acceleration);
            }
        }

        //トラックの方向に向かせる
        int lookAheadIndex = Mathf.Min(_nearestSegmentIndex + OFFSET_TARGET_POINT, trackWaypoints.Length - 1);
        Vector3 targetDirection = trackWaypoints[lookAheadIndex] - transform.position;
        targetDirection.y = 0f;

        if (targetDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection.normalized);
            //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, ROTATE_SPEED * Time.fixedDeltaTime);
            Quaternion newRotation = Quaternion.RotateTowards(_rigidbody.rotation, targetRotation, ROTATE_SPEED * Time.fixedDeltaTime);

            _rigidbody.MoveRotation(newRotation);
        }

        //前に進むスピードを取得
        Vector3 forwardVelocity = transform.forward * Vector3.Dot(_rigidbody.linearVelocity, transform.forward);
        //横に進むスピードを取得
        Vector3 sideVelocity = transform.right * Vector3.Dot(_rigidbody.linearVelocity, transform.right);

        //通常時とドリフト時で減衰のさせ方を変える
        if (_isCPUBreke)
        {
            forwardVelocity = forwardVelocity * Mathf.Exp(-BRAKE_FORWARD_ATTENUATION * Time.fixedDeltaTime);
            sideVelocity = sideVelocity * Mathf.Exp(-BRAKE_SIDE_ATTENUATION * Time.fixedDeltaTime);
        }
        else
        {
            forwardVelocity = forwardVelocity * Mathf.Exp(-BASE_FORWARD_ATTENUATION * Time.fixedDeltaTime);
            sideVelocity = sideVelocity * Mathf.Exp(-BASE_SIDE_ATTENUATION * Time.fixedDeltaTime);
        }
        
        //前方向の力と横方向の力を合成する
        Vector3 moveVelocity = forwardVelocity + sideVelocity;

        //壁方向の移動量をなくす
        Vector3 moveDirection = _vectorControlHitWall.AdjustDirection(moveVelocity);

        _rigidbody.linearVelocity = new Vector3(moveDirection.x, _rigidbody.linearVelocity.y, moveDirection.z);
    }

    private void UpdateRawDistance()
    {
        float minSqrDist = float.MaxValue;

        for (int i = 0; i < trackWaypoints.Length - 1; i++)
        {
            Vector3 a = trackWaypoints[i];
            Vector3 b = trackWaypoints[i + 1];

            Vector3 projected = ProjectPointOnSegment(_targetTransform.position, a, b, out float t);

            float sqrDist = (_targetTransform.position - projected).sqrMagnitude;

            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                _nearestSegmentIndex = i;
            }
        }
    }

    //点の線形補正
    private Vector3 ProjectPointOnSegment(Vector3 point, Vector3 a, Vector3 b, out float t)
    {
        Vector3 ab = b - a;
        float abSqr = ab.sqrMagnitude;

        if (abSqr < Mathf.Epsilon)
        {
            t = 0f;
            return a;
        }

        t = Vector3.Dot(point - a, ab) / abSqr;
        t = Mathf.Clamp01(t);

        return a + ab * t;
    }

    private void PassVelocityCPU()
    {
        _cpuStatus.UpdateRunSpeed(_rigidbody.linearVelocity.magnitude);
    }
}
