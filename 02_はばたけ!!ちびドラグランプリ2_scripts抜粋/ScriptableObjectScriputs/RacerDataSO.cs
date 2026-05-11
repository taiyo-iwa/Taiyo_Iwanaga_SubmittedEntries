using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RacerDataSO", menuName = "Scriptable Objects/RacerDataSO")]
public class RacerDataSO : ScriptableObject
{
    public List<RacerData> Racers = new List<RacerData>();

    public void SetResult(List<RacerData> racers)
    {
        Racers = racers;
    }

    public void Clear()
    {
        Racers.Clear();
    }
}
