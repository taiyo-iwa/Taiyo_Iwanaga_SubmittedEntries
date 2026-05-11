using System.Collections.Generic;
using UnityEngine;

public class RaceUIManager : MonoBehaviour
{
    public static RaceUIManager Instance;

    [SerializeField] private List<RaceRankingView> _views;

    private void Start()
    {
        Instance = this;
    }

    public void UpdateRanking(List<RaceProgressTracker> ranking)
    {
        for (int i = 0; i < ranking.Count; i++)
        {
            _views[i].SetData(
                i + 1,
                ranking[i].RacerId,
                ranking[i].LapCount);
        }
    }
}
