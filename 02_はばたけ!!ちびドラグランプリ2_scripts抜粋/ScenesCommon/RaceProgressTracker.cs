using SWS;
using UniRx;
using System;
using UnityEngine;

public class RaceProgressTracker : MonoBehaviour
{
    //ゴール誤差のしきい値
    private const float LAP_THRESHOLD_RATIO = 0.9f;
    //1フレームで進める最大のpathPoint数
    private const int MAX_FORWARD_STEP = 10;

    [SerializeField] private BezierPathManager _bezierPathManager = default;
    [SerializeField] private Transform _characterTransform = default;

    public bool IsReverse { get; private set; }

    //CharacterModelManagerからRacerIdの値を流し込む
    public int RacerId { get; set; }
    public float RawDistance { get; private set; }
    public float TotalRaceProgress { get; private set; }
    public int LapCount { get; private set; }

    private int _lapCount = 1;
    private int _nearestSegmentIndex = 0;
    private int _progressPathIndex = 0;

    private float _totalPathLength = 0.0f;
    //位置ベースの距離
    private float _rawDistance = 0.0f;
    //直前まで走っていた距離
    private float previousRawDistance = 0.0f;
    //前進時のみ更新の進捗距離
    private float _progressDistance = 0.0f;
    //線形補間率
    private float _nearestT = 0.0f;
    private float[] cumulativeDistances = default;

    private bool _canCountLap = true;
    private bool _wentAround = false;

    private Vector3[] trackWaypoints = default;

    private Subject<int> lapCompletedSubject = new Subject<int>();

    public IObservable<int> OnLapCompleted
    {
        get { return lapCompletedSubject; }
    }

    public void StartRaceProgressTracker()
    {
        trackWaypoints = _bezierPathManager.pathPoints;
        BuildCumulativeDistances(trackWaypoints);

        for (int i = 0; i < trackWaypoints.Length - 1; i++)
        {
            _totalPathLength += Vector3.Distance(trackWaypoints[i], trackWaypoints[i + 1]);
        }

        LapCount = _lapCount;
        TotalRaceProgress = _totalPathLength;
    }

    public void UpdateRaceProgressTracker()
    {
        //現在位置の更新
        UpdateRawDistance();
        //周回・順位用進捗距離
        UpdateProgress();
        //逆走判定
        UpdateReverse();
        //周回判定
        UpdateLapFlag();

        RawDistance = _rawDistance;
    }

    private void BuildCumulativeDistances(Vector3[] points)
    {
        cumulativeDistances = new float[points.Length];
        cumulativeDistances[0] = 0.0f;

        for (int i = 1; i < points.Length; i++)
        {
            float segmentLength = Vector3.Distance(points[i - 1], points[i]);
            cumulativeDistances[i] = cumulativeDistances[i - 1] + segmentLength;
        }
    }

    //プレイヤーの現在の距離
    private void UpdateRawDistance()
    {
        float minSqrDist = float.MaxValue;

        for (int i = 0; i < trackWaypoints.Length - 1; i++)
        {
            Vector3 a = trackWaypoints[i];
            Vector3 b = trackWaypoints[i + 1];

            Vector3 projected = ProjectPointOnSegment(_characterTransform.position, a, b, out float t);

            float sqrDist = (_characterTransform.position - projected).sqrMagnitude;

            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                _nearestSegmentIndex = i;
                _nearestT = t;
            }
        }

        float segmentLength = Vector3.Distance(trackWaypoints[_nearestSegmentIndex], trackWaypoints[_nearestSegmentIndex + 1]);

        _rawDistance = cumulativeDistances[_nearestSegmentIndex] + segmentLength * _nearestT;
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

    //前進時のみ更新の距離（周回・順位用）
    private void UpdateProgress()
    {
        int diff = _nearestSegmentIndex - _progressPathIndex;

        if (diff < 0)
        {
            diff += trackWaypoints.Length;
        }

        // 前進 & 小さな差分だけ許可
        if (diff > 0 && diff <= MAX_FORWARD_STEP)
        {
            _progressPathIndex = _nearestSegmentIndex;
            _progressDistance = cumulativeDistances[_progressPathIndex];
        }
    }

    //逆走判定
    private void UpdateReverse()
    {
        float delta = _rawDistance - previousRawDistance;

        if (delta < -0.05f)
        {
            IsReverse = true;
        }     
        else if (delta > 0.02f)
        {
            IsReverse = false;
        }

        previousRawDistance = _rawDistance;
    }

    private void UpdateLapFlag()
    {
        if (_progressDistance >= _totalPathLength * LAP_THRESHOLD_RATIO)
        {  
            _wentAround = true;
        }
        if (_rawDistance < _totalPathLength * 0.2)
        {
            _canCountLap = true;
        }
    }

    //キャラクターがゴールラインに触れた時の処理
    //GoalTriggerからイベントを発火
    public void OnGoalLineTouched()
    {
        if (IsReverse)
        {
            return;
        }

        if (!_canCountLap)
        {
            return;
        }

        //一周していたなら
        if (!_wentAround)
        {
            return;
        }

        _lapCount++;
        lapCompletedSubject.OnNext(_lapCount);
        LapCount = _lapCount;
        _wentAround = false;
        _canCountLap = false;
    }
}