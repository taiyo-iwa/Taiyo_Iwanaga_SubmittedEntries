using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

public class RaceAudioController : MonoBehaviour
{
    private const float START_RACEBGM_WAIT_TIME = 2.0f;

    [SerializeField] private RaceStatus _raceStatus = default;
    [SerializeField] private RaceProgressTracker _raceProgressTracker = default;
    [SerializeField] private RaceRule _raceRule = default;
    [SerializeField] private AudioSource _audioSource = default;
    [SerializeField] private AudioClip _raceBGM = default;
    [SerializeField] private AudioClip _finalLapBGM = default;

    public void Start()
    {
        _raceStatus.OnStateChanged
        .Where(state => state == RaceState.Running)
        .Subscribe(state =>
        {
            StartRaceBGM();
        })
        .AddTo(this);

        //ファイナルラップに鳴ったら音を変える
        _raceProgressTracker.OnLapCompleted
        .Where(lap => lap == _raceRule.MaxLap)
        .Subscribe(_ =>
        {
            StartFinalLapBGM();
        })
        .AddTo(this);
    }

    private async void StartRaceBGM()
    {
        await StartRaceBGMWait(); 
    }

    private async UniTask StartRaceBGMWait()
    {
        await UniTask.WaitForSeconds(START_RACEBGM_WAIT_TIME);

        _audioSource.Stop();
        _audioSource.clip = _raceBGM;
        _audioSource.Play();
    }

    private void StartFinalLapBGM()
    {
        _audioSource.Stop();
        _audioSource.clip = _finalLapBGM;
        _audioSource.Play();
    }
}
