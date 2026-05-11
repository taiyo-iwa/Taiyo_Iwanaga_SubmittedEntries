using UnityEngine;
using UniRx;

public class RaceFlowController : MonoBehaviour
{
    [SerializeField] private RaceRule _raceRule = default;
    [SerializeField] private RaceStatus _raceStatus = default;
    [SerializeField] private RaceCountdownView _raceCountdownView = default;
    [SerializeField] private RaceProgressTracker _raceProgressTracker = default;
    [SerializeField] private StartPerformance _startPerformance = default;

    public void StartRaceFlowController()
    {
        switch (_raceRule.SelectedRule)
        {
            case RaceRuleState.Tutorial:
                TutorialMode();
                break;
            case RaceRuleState.Race:
                RaceMode();
                break;
            case RaceRuleState.TimeAttack:
                TimeAttackmMode();
                break;
        }
    }

    private void TutorialMode()
    {
        _raceStatus.ChangeState(RaceState.Running);
    }

    private void RaceMode()
    {
        _startPerformance.IsCameraFollow = true;

        //スタート演出完了の通知を受け取ったらRaceStateをCountDownにする
        _startPerformance.OnStartPerformance
        .Subscribe(_ => { _raceStatus.ChangeState(RaceState.Countdown); })
        .AddTo(this);

        //カウントダウン完了の通知を受け取ったらRaceStateをRunningにする
        _raceCountdownView.OnCountDownCompleted
        .Subscribe(_ => { _raceStatus.ChangeState(RaceState.Running); })
        .AddTo(this);

        //ラップの周回数通知を受け取ってRaceStateをFinishにする
        _raceProgressTracker.OnLapCompleted
        .Where(lap => lap > _raceRule.MaxLap)
        .Subscribe(_ =>
        {
            _raceStatus.ChangeState(RaceState.Finish);
        })
        .AddTo(this);
    }

    private void TimeAttackmMode()
    {
        _startPerformance.IsCameraFollow = true;

        //スタート演出完了の通知を受け取ったらRaceStateをCountDownにする
        _startPerformance.OnStartPerformance
        .Subscribe(_ => { _raceStatus.ChangeState(RaceState.Countdown); })
        .AddTo(this);

        //カウントダウン完了の通知を受け取ったらRaceStateをRunningにする
        _raceCountdownView.OnCountDownCompleted
        .Subscribe(_ => { _raceStatus.ChangeState(RaceState.Running); })
        .AddTo(this);

        //ラップの周回数通知を受け取ってRaceStateをFinishにする
        _raceProgressTracker.OnLapCompleted
        .Where(lap => lap > _raceRule.MaxLap)
        .Subscribe(_ =>
        {
            _raceStatus.ChangeState(RaceState.Finish);
        })
        .AddTo(this);
    }
}
