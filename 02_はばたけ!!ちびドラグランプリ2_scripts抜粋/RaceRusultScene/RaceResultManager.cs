using UnityEngine;
using System.Collections.Generic;

public class RaceResultManager : MonoBehaviour
{
    [SerializeField] private RaceResultSO _raceResultSO = default;
    [SerializeField] private RacerDataSO _raceData = default;
    [SerializeField] private RaceResultPerformance _raceResultPerformance = default;

    List<int> racerIdList = new List<int>();
    List<int> finishRankList = new List<int>();
    List<float> finishTimeList = new List<float>();

    private void Start()
    {
        foreach (RaceResultData raceResult in _raceResultSO.Results)
        {
            racerIdList.Add(raceResult.RaceId);
            finishRankList.Add(raceResult.FinalRank);
            finishTimeList.Add(raceResult.FinishTime);
        }

        _raceResultPerformance.ResultPerformance(racerIdList, _raceData.Racers[0].SelectRacerId);

        foreach(RaceResultData raceResult in _raceResultSO.Results)
        {
            if(raceResult.RaceId == _raceData.Racers[0].SelectRacerId)
            {
                _raceResultPerformance.ChangeRankUI(raceResult.FinalRank);
            }
        }

        _raceResultSO.Clear();
    }
}
