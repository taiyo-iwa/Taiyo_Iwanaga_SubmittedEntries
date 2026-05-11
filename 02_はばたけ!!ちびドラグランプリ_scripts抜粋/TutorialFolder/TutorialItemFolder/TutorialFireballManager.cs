using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialFireballManager : MonoBehaviour
{
    //ターゲット対象の設定
    [SerializeField] Transform[] _dragons = default;
    //生成するためにFireballのプレファブを入れる
    [SerializeField] GameObject _fireballPrefab = default;
    //Fireballを生成するメソッド　引数は生成する対象のオブジェクトの位置(アイテム使ったドラゴンの位置)を入れる
    public void MakeFireball(Transform fireballMaker)
    {
        GameObject fireball =
            Instantiate(_fireballPrefab,
            fireballMaker.position + fireballMaker.forward + Vector3.up * 0.5f,
            fireballMaker.rotation);
        TutorialFireBall fireballScript = fireball.GetComponent<TutorialFireBall>();
        fireballScript.SetUp(fireballMaker, _dragons);
    }
}
