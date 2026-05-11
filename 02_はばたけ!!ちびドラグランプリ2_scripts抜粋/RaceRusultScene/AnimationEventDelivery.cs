using UnityEngine;
using UniRx;
using System;

public class AnimationEventDelivery : MonoBehaviour
{
    private Subject<Unit> finishPerfomanceSubject = new Subject<Unit>();

    public IObservable<Unit> OnFinishPerformance
    {
        get { return finishPerfomanceSubject; }
    }

    //プレイヤーのリザルトアニメーションが終了したら
    public void FinishchactorAnimation()
    {
        finishPerfomanceSubject.OnNext(Unit.Default);
    }
}
