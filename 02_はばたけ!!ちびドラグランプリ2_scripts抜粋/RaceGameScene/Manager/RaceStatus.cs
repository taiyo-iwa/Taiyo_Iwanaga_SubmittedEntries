using UnityEngine;
using UniRx;
using System;

public enum RaceState
{
    Ready,
    Countdown,
    Running,
    Finish,
}

public class RaceStatus : MonoBehaviour
{
    public RaceState CurrentState { get; private set; } = RaceState.Ready;

    private Subject<RaceState> raceStateSubject = new Subject<RaceState>();

    public IObservable<RaceState> OnStateChanged
    {
        get { return raceStateSubject; }
    }

    public void ChangeState(RaceState nextState)
    {
        if(CurrentState == nextState)
        {
            return;
        }

        ExitState(CurrentState);

        CurrentState = nextState;
        
        EnterState(CurrentState);

        raceStateSubject.OnNext(nextState);
    }

    private void EnterState(RaceState state)
    {
        switch (state)
        {
            case RaceState.Ready:
                break;
            case RaceState.Countdown:
                break;
            case RaceState.Running:
                break;
            case RaceState.Finish:
                break;
        }
    }

    private void ExitState(RaceState state)
    {
        switch (state)
        {
            case RaceState.Ready:
                break;
            case RaceState.Countdown:
                break;
            case RaceState.Running:
                break;
            case RaceState.Finish:
                break;
        }
    }
}
