using UnityEngine;
using UniRx;
using System;
using Unity.Cinemachine;
using Cysharp.Threading.Tasks;

public class StartPerformance : MonoBehaviour
{
    private const float FOLLOW_ADMISSIBLE_RATIO = 1.0f;
    private const float FINISH_PERFORMANCE_WAIT = 1.5f;

    [SerializeField] private CinemachineOrbitalFollow _playerFollowCamera = default;
    [SerializeField] private Vector3 _targetOffset = new Vector3(0.0f, 17.0f, 20.0f);

    private bool _isCameraFollow = false;
    private float _cameraFollowSpeed = 1.0f;

    public bool IsCameraFollow
    {
        get { return _isCameraFollow; }
        set { _isCameraFollow = value; }
    }

    private Subject<Unit> startPerformanceSubject = new Subject<Unit>();

    public IObservable<Unit> OnStartPerformance
    {
        get { return startPerformanceSubject; }
    }

    public void UpdateStartPerformance()
    {
        if (!_isCameraFollow)
        {
            return;
        }

        _playerFollowCamera.TargetOffset = Vector3.Lerp(_playerFollowCamera.TargetOffset, _targetOffset, _cameraFollowSpeed * Time.deltaTime);     
        if (Mathf.Abs(_targetOffset.x - _playerFollowCamera.TargetOffset.x) <= FOLLOW_ADMISSIBLE_RATIO)
        {
            if (Mathf.Abs(_targetOffset.y - _playerFollowCamera.TargetOffset.y) <= FOLLOW_ADMISSIBLE_RATIO)
            {
                _isCameraFollow = false;
                StartPerformanCecomplete();
            }
        }
    }

    private async void StartPerformanCecomplete()
    {
        await UniTask.WaitForSeconds(FINISH_PERFORMANCE_WAIT);
        startPerformanceSubject.OnNext(Unit.Default);
    }
}
