using UniRx;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class RaceManager : MonoBehaviour
{
    private const float SCENE_CHANGE_WAIT_TIME = 5.0f;
    public const string SCENE_CHANGE_NAME = "RaceRusultScene";

    [SerializeField] private RaceStatus _raceStatus = default;
    [SerializeField] private TimerView _timerView = default;
    [SerializeField] private RaceResultSO _raceResultSO = default;

    private List<RaceProgressTracker> _trackers = default;
    private float _raceTimer = 0.0f;
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

        _trackers = FindObjectsByType<RaceProgressTracker>(FindObjectsSortMode.None).ToList();
        UpdateRanking();
    }

    public void Update()
    {
        if (_isRaceStart)
        {
            _raceTimer += Time.deltaTime;
            _timerView.TotalTimeTextUpdate(_raceTimer);
            UpdateRanking();
        }
    }

    private void UpdateRanking()
    {
        List<RaceProgressTracker> ranking = _trackers
            .OrderByDescending(tracker =>tracker.LapCount * tracker.TotalRaceProgress + tracker.RawDistance).ToList();

        RaceUIManager.Instance.UpdateRanking(ranking);
    }

    //プレイヤーのゴールした後の処理
    //RaceFlowControllerがプレイヤーのRaceProgressTrackerを取得しているため
    private async void FinishProcess()
    {
        List<RaceProgressTracker> ranking = _trackers.
            OrderByDescending(tracker => tracker.LapCount * tracker.TotalRaceProgress + tracker.RawDistance).ToList();

        List<RaceResultData> results = new List<RaceResultData>();

        for(int i = 0; i < ranking.Count; i++)
        {
            results.Add(new RaceResultData
            {
                RaceId = ranking[i].RacerId,
                FinalRank = i + 1,
                FinishTime = _raceTimer
            });
        }

        _raceResultSO.SetResult(results);

        await SceneChangeWait();
    }

    //移動するシーンがロードされるまで待つ
    private async UniTask SceneChangeWait()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SCENE_CHANGE_NAME);

        asyncLoad.allowSceneActivation = false;

        //移動するシーンがロードし終わるまで待つ
        while (asyncLoad.progress < 0.9f)
        {
            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        await UniTask.WaitForSeconds(SCENE_CHANGE_WAIT_TIME);

        asyncLoad.allowSceneActivation = true;

        await asyncLoad.ToUniTask();
    }
}
