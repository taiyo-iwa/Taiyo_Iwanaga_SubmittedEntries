using UnityEngine;
using UniRx;
using System;

public class PlayerStatus : MonoBehaviour
{
    public float InputHorizontal { get; private set; }
    public bool IsSouthButton { get; private set; }
    public float RunSpeed { get; private set; }
    public bool CanMove { get; private set; } = false;
    public bool CanReleaseCharge { get; private set; } = false;

    private Subject<Unit> startChargeDashSubject = new Subject<Unit>();

    public IObservable<Unit> OnStartChargeDash
    {
        get { return startChargeDashSubject; }
    }

    public void Initialize(RaceStatus raceStatus)
    {
        raceStatus.OnStateChanged
            .Where(state => state == RaceState.Running)
            .Subscribe(_ => 
            {
                CanMove = true;
                CanReleaseCharge = true;
            })
            .AddTo(this);

        raceStatus.OnStateChanged
            .Where(state => state != RaceState.Running)
            .Subscribe(_ => 
            {
                CanMove = false;
                CanReleaseCharge = false;
            })
            .AddTo(this);

        raceStatus.OnStateChanged
            .Where(state => state == RaceState.Finish)
            .Subscribe(_ =>
            {
                CanMove = false;
                CanReleaseCharge = false;
            })
            .AddTo(this);
    }

    //‘‚«‚İƒ‹[ƒ‹‚ğ‚ ‚ê‚Î‚±‚±‚É‘‚­
    public void UpdateInputHorizontal(float inputHorizontal)
    {
        InputHorizontal = inputHorizontal;
    }

    public void UpdateSouthButton(bool southButtonInput)
    {
        IsSouthButton = southButtonInput;
    }

    public void UpdateRunSpeed(float runSpeed)
    {
        RunSpeed = runSpeed;
    }

    public void NotifyStartChargeDash()
    {
        startChargeDashSubject.OnNext(Unit.Default);
    }
}
