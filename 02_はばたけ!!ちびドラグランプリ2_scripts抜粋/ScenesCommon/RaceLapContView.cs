using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class RaceLapContView : MonoBehaviour
{
    [SerializeField] private RaceRule _raceRule = default;
    [SerializeField] private RaceProgressTracker _raceProgressTracker = default;
    [SerializeField] private Text _lapCount = default;

    private void Start()
    {
        //Å‰‚ÉLap”‚ð•\Ž¦‚³‚¹‚é‚½‚ß
        LapCountView(_raceProgressTracker.LapCount);

        _raceProgressTracker.OnLapCompleted
        .Subscribe(lap =>
        {
            LapCountView(lap);
        })
        .AddTo(this);
    }

    private void LapCountView(int lapCount)
    {
        if(lapCount > _raceRule.MaxLap)
        {
            return;
        }
        _lapCount.text = lapCount.ToString() + " / " + _raceRule.MaxLap.ToString();
    }
}
