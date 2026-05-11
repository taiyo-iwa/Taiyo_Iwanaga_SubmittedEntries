using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RaceResultSO", menuName = "Scriptable Objects/RaceResultSO")]
public class RaceResultSO : ScriptableObject
{
    public List<RaceResultData> Results = new List<RaceResultData>();

    public void SetResult(List<RaceResultData> results)
    {
        Results = results;
    }

    public void Clear()
    {
        Results.Clear();
    }
}
