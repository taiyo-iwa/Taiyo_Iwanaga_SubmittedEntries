using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RacerNameSO", menuName = "Scriptable Objects/RacerNameSO")]
public class RacerNameSO : ScriptableObject
{
    public List<string> RacerNameList;

    //public void SetResult(List<RacerNameData> racers)
    //{
    //    RacerNameList = racers;
    //}

    //public void Clear()
    //{
    //    RacerNameList.Clear();
    //}
}
