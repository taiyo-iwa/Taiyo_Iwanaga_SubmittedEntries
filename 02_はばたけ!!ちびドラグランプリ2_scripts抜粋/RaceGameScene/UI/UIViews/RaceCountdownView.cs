using UnityEngine;
using UniRx;
using System;

public class RaceCountdownView : MonoBehaviour
{
    private const string START_COUNTDOWN = "StartCountDown";

    [SerializeField] private RaceStatus _raceStatus = default;
    [SerializeField] private Animator _countDownAnimator = default;
    [SerializeField] private AudioSource _audioSource = default;
    [SerializeField] private AudioClip _countDownSE = default;

    private Subject<Unit> countdownCompletedSubject = new Subject<Unit>();

    public IObservable<Unit> OnCountDownCompleted
    {
        get { return countdownCompletedSubject; }
    }

    /*カウントダウン開始指示がStartメソッドにあるので
     * 購読の開始をAwakeでする
    */
    public void Awake()
    {
        _raceStatus.OnStateChanged
        .Where(state => state == RaceState.Countdown)
        .Subscribe(state => 
        {
            StartCountDown();
        })
        .AddTo(this);
    }

    //RaceStateがCountDownになった時
    private void StartCountDown()
    {
        _countDownAnimator.SetTrigger(START_COUNTDOWN);
    }

    /*カウントダウンの数字が変わったタイミングで音を鳴らす
     * Animation側から呼び出す
    */
    public void PlayCountDownSE()
    {
        _audioSource.clip = _countDownSE;
        _audioSource.Play();
    }

    //Animation側で呼び出して通知する
    public void OnCountDownFinishedAnimation()
    {
        countdownCompletedSubject.OnNext(Unit.Default);
    }
}
