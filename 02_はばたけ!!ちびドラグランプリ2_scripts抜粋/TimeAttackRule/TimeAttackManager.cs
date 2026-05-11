using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

public class TimeAttackManager : MonoBehaviour
{
    [SerializeField] private RaceStatus _raceStatus = default;
    [SerializeField] private RaceProgressTracker _raceProgressTracker = default;
    [SerializeField] private TimerView _timerView = default;
    [SerializeField] private TimeAttackResultManager _resultManager = default;

    private int _currentLap = 0;
    private float _raceTimer = 0.0f;
    private float _lapTimer = 0.0f;
    private bool _isRaceStart = false;   

    public void Start()
    {
        _raceStatus.OnStateChanged
        .Where(state => state == RaceState.Running)
        .Subscribe(state =>
        {
            _isRaceStart = true;
        })
        .AddTo(this);

        _raceStatus.OnStateChanged
        .Where(state => state == RaceState.Finish)
        .Subscribe(state =>
        {
            _isRaceStart = false;
            FinishProcess();
        })
        .AddTo(this);

        _raceProgressTracker.OnLapCompleted
        .Subscribe(lap => 
        {
            LapTimeManager(lap);
        })
        .AddTo(this);

        _currentLap = _raceProgressTracker.LapCount;
    }

    public void Update()
    {
        if (_isRaceStart)
        {
            _raceTimer += Time.deltaTime;
            _lapTimer += Time.deltaTime;
            _timerView.TotalTimeTextUpdate(_raceTimer);
            _timerView.LapTimeTextUpdate(_currentLap, _lapTimer);
        }
        _resultManager.UpdateResultCameraFollow();
    }

    private void LapTimeManager(int lap)
    {
        if(_currentLap != lap)
        {
            _currentLap = lap;
            _lapTimer = 0.0f;
        }
    }

    //ÉSÅ[ÉãÇµÇΩå„ÇÃèàóù
    private async void FinishProcess()
    {
        await UniTask.WaitForSeconds(2.0f);
        _resultManager.CameraFollow();
        _resultManager.RecordTimeText(_raceTimer);
    }
}
