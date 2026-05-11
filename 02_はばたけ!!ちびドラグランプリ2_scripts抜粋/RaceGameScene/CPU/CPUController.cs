using SWS;
using UniRx;
using UnityEngine;

public class CPUController : MonoBehaviour
{
    [SerializeField] private RaceStatus _raceStatus = default;
    [SerializeField] private CPUStatus _cpuStatus = default;
    [SerializeField] private CPUMove _cpuMove = default;
    [SerializeField] private CPUChargeDash _cpuChargeDash = default;
    [SerializeField] private BezierPathManager _bezierPathManager = default;
    [SerializeField] private Transform _targetTransform = default;
    [SerializeField] private float _brakeThresholdValue = 50.0f;

    private int _nearestSegmentIndex = 0;
    private float _brakeInputTime = 0.0f;
    private Vector3[] trackWaypoints = default;

    public void CPUControllerStart()
    {
        _raceStatus.OnStateChanged
        .Where(state => state == RaceState.Countdown)
        .Subscribe(state =>
        {
            CPUStartDash();
        })
        .AddTo(this);

        trackWaypoints = _bezierPathManager.pathPoints;
    }

    public void CPUControllerUpdate()
    {
        CPUBrakeController();
        CPUBrekeCharge();
        UpdateRawDistance();
    }

    private void CPUBrakeController()
    {
        //ベクトルを求める
        int lookAheadIndex = Mathf.Min(_nearestSegmentIndex, trackWaypoints.Length - 1);
        Vector3 targetPositionToNowPosition = transform.position - trackWaypoints[lookAheadIndex];
        Vector3 targetPositionToNextTargetPosition = trackWaypoints[lookAheadIndex + 1] - trackWaypoints[lookAheadIndex];
        targetPositionToNowPosition = targetPositionToNowPosition.normalized;
        targetPositionToNextTargetPosition = targetPositionToNextTargetPosition.normalized;

        //３点を結んだ角度を求める
        float predictionDot = Vector3.Dot(targetPositionToNowPosition, targetPositionToNextTargetPosition);
        float predictionAngle = Mathf.Acos(predictionDot) * Mathf.Rad2Deg;

        if(predictionAngle < _brakeThresholdValue)
        {
            BrakeInputTime(0.5f);
        }
    }

    private void CPUStartDash()
    {
        BrakeInputTime(3.0f);
    }

    private void BrakeInputTime(float brakeInputTime)
    {
        _brakeInputTime = brakeInputTime;
    }

    //CPUStatusに下方向のボタンの入力を受け渡す
    private void PassChargeInput(bool chargeInput)
    {
        _cpuStatus.UpdateChargeInput(chargeInput);
    }

    private void CPUBrekeCharge()
    {
        _brakeInputTime -= Time.deltaTime;
        if (_brakeInputTime > 0.0f)
        {
            _cpuMove.CPUBrakeInput(true);
            _cpuChargeDash.CPUChargeInput(true);
            PassChargeInput(true);
            return;
        }

        _brakeInputTime = 0.0f;
        _cpuMove.CPUBrakeInput(false);
        _cpuChargeDash.CPUChargeInput(false);
        PassChargeInput(false);
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
}
