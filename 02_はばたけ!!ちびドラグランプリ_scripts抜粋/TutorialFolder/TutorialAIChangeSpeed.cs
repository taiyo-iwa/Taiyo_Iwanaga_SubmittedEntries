using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAIChangeSpeed : MonoBehaviour
{
    //プレイヤーのドラゴンは何番か調べるのに必要
    [SerializeField] private TutorialGameManager _gameManager = default;
    [SerializeField]
    private int aiIndex = 0;
    [SerializeField]
    private pathRanking pathRank;
    [SerializeField]
    private PlayerMovement _playerMovement;

    private float standardForwardForce = 20;
    private float standardMaxSpeed = 12;
    private float standardRotateSpeed = 1;

    private float changeForwardForce = 0.05f;
    private float changeMaxSpeed = 0.03f;
    private float changeRotateSpeed = 0.005f;

    private float changeTime = 0;

    private int changeStandard = 0;

    void Update()
    {
        int diff = pathRank.GetCurrentPoints(_gameManager._selectDragon) - pathRank.GetCurrentPoints(aiIndex);
        _playerMovement._runForce = Mathf.Clamp(standardForwardForce + diff * changeForwardForce, 5, 30);
        _playerMovement._maxRunSpeed = Mathf.Clamp(standardMaxSpeed + diff * changeMaxSpeed, 3, 18);
        _playerMovement._rotateSpeed = Mathf.Clamp(standardRotateSpeed + diff * changeRotateSpeed, 0.4f, 1.2f);

        if (diff > 5)
        {
            changeTime = Mathf.Max(changeTime + Time.deltaTime, 0);
            if (changeTime > 5 && changeStandard < 5 && pathRank.GetCurrentPoints(aiIndex) < 3000)
            {
                changeTime = 0;

                standardForwardForce += changeForwardForce * 10;
                standardMaxSpeed += changeMaxSpeed * 10;
                standardRotateSpeed += changeRotateSpeed * 10;

                changeStandard++;
            }
        }
        else if (diff < -5)
        {
            changeTime = Mathf.Min(changeTime - Time.deltaTime, 0);
            if (changeTime < -5 && changeStandard > -8)
            {
                changeTime = 0;

                standardForwardForce -= changeForwardForce * 10;
                standardMaxSpeed -= changeMaxSpeed * 10;
                standardRotateSpeed -= changeRotateSpeed * 10;

                changeStandard--;
            }
        }
    }
}
