using System.Collections.Generic;
using UnityEngine;

public class CharacterModelManager : MonoBehaviour
{
    [SerializeField] private RacerDataSO _racerDataSO = default;
    [SerializeField] private List<GameObject> characterList = new List<GameObject>();
    [SerializeField] private List<GameObject> characterModelList = new List<GameObject>();
    [SerializeField] private List<RaceProgressTracker> raceProgressTrackerList = new List<RaceProgressTracker>();

    List<int> racerIdList = new List<int>();

    public void AwakeCharacterModelManager()
    {
        foreach (RacerData racerData in _racerDataSO.Racers)
        {
            racerIdList.Add(racerData.SelectRacerId);
        }

        CharacterModelChange();
    }

    private void CharacterModelChange()
    {
        List<GameObject> tmpModelList = characterModelList;
        List<RaceProgressTracker> tmpProgressTrackers = raceProgressTrackerList;

        //プレイヤーのプレハブを生成
        GameObject playerPrefab = Instantiate(tmpModelList[racerIdList[0]]);
        // 生成したオブジェクトを親の子にする
        playerPrefab.transform.SetParent(characterList[0].transform, false);
        // 位置を初期化
        playerPrefab.transform.localRotation = Quaternion.Euler(0, 0, 0);
        //RaceProgressTrackerのRaceIdを決める
        raceProgressTrackerList[0].RacerId = racerIdList[0];


        //選んだキャラクターの要素をModelListの最初の要素で埋める
        tmpModelList[racerIdList[0]] = tmpModelList[0];
        //tmpProgressTrackers[racerIdList[0]] = tmpProgressTrackers[0];

        //CPUのモデルを割り当てる
        for (int i = 1; i < characterList.Count; i++)
        {
            GameObject cpuPrefab = Instantiate(tmpModelList[i]);
            cpuPrefab.transform.SetParent(characterList[i].transform, false);
            cpuPrefab.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        for(int i = 1; i < raceProgressTrackerList.Count; i++)
        {
            if(i != racerIdList[0])
            {
                raceProgressTrackerList[i].RacerId = i;
            }
        }
    }
}
