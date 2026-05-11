using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class TutorialSelectPlayerSystem : MonoBehaviour
{
    //プレイヤーのドラゴンは何番か調べるのに必要
    [SerializeField] private TutorialGameManager _gameManager = default;
    //プレイヤースクリプト　指標をselectDragonと合わせる
    [SerializeField] TutorialPlayerInput[] _playerInputs = default;
    [SerializeField] TutorialPlayerMove[] _playerMoves = default;
    //ドラゴンスクリプト　指標をselectDragonと合わせる
    [SerializeField] AIInput[] _aiInputs = default;
    [SerializeField] AIChangeSpeed[] _aiChangeSpeeds = default;
    [SerializeField] TutorialPlayerMovement[] _playerMovements = default;
    //isPlayerを変更するために必要(ドリフトの処理とかがちょっと違う)
    [SerializeField] TutorialPlayerController[] _playerControllers;
    //入力の受付をプレイヤーだけにするために必要
    [SerializeField] private PlayerInput[] _inputSystems;
    //アイテムのスクリプトのプレイヤーかどうかを判定するブール値を変更するために必要
    [SerializeField] private TutorialPlayerItem[] _playerItems;
    //PlayerWarning　炎のUIを動かすかどうかの判定をつけるため
    [SerializeField] private TutorialPlayerWarning[] _playerWarning;
    //設定するUIのオブジェクト 0指標に1p 1指標に2p
    [SerializeField] private GameObject[] _flameWarnings;
    //PlayerItemのプレイヤー番号の設定

    //1pのロードのマテリアル
    [SerializeField] private Material _onePlayerRoadMaterial = default;
    //2pのロードのマテリアル
    [SerializeField] private Material _twoPlayerRoadMaterial = default;

    //カメラの追従対象を設定するために必要
    [SerializeField] CinemachineVirtualCamera _cinemachineVirtualCamera;
    [SerializeField] CinemachineVirtualCamera _twoPlayerCinemachineVirtualCamera;

    [SerializeField] CinemachineBrain _cinemachineBrain;
    [SerializeField] CinemachineBrain _twoPlayerCinemachineBrain;
    void Awake()
    {
        for (int i = 0; i < _playerInputs.Length; i++)
        {
            if (i == _gameManager._selectDragon)
            {
                _playerInputs[i].enabled = true;
                _cinemachineVirtualCamera.Follow = _playerInputs[i].transform;
                _cinemachineVirtualCamera.LookAt = _playerInputs[i].transform;
                _playerControllers[i].SetIsPlayer(true);
                _playerItems[i]._isPlayer = true;
                _playerItems[i]._playerNumber = 0;
                _playerMoves[i]._playerNumber = 0;
                _playerMoves[i]._roadMaterial = _onePlayerRoadMaterial;
                _playerMoves[i]._visualCamera = _cinemachineBrain;
                _playerWarning[i]._flameWarning = _flameWarnings[0];
                _playerMovements[i]._playerNumber = 0;
            }
            else if (i == _gameManager._twoPlayerSelectDragon)
            {
                _playerInputs[i].enabled = true;
                _twoPlayerCinemachineVirtualCamera.Follow = _playerInputs[i].transform;
                _twoPlayerCinemachineVirtualCamera.LookAt = _playerInputs[i].transform;
                _playerControllers[i].SetIsPlayer(true);
                _playerItems[i]._isPlayer = true;
                _playerItems[i]._playerNumber = 1;
                _playerMoves[i]._playerNumber = 1;
                _playerMoves[i]._roadMaterial = _twoPlayerRoadMaterial;
                _playerMoves[i]._visualCamera = _twoPlayerCinemachineBrain;
                _playerWarning[i]._flameWarning = _flameWarnings[1];
                _playerMovements[i]._playerNumber = 1;
            }
            else
            {
                //_aiInputs[i].enabled = true;
                //_aiChangeSpeeds[i].enabled = true;
                _playerControllers[i].SetIsPlayer(false);
            }
        }

        //プレイヤーの操作入力だけは別
        for (int i = 0; i < _playerControllers.Length; i++)
        {
            if (i == _gameManager._selectDragon)
            {
                _inputSystems[i].enabled = true;
            }
        }
        for (int i = 0; i < _playerControllers.Length; i++)
        {
            if (i == _gameManager._twoPlayerSelectDragon)
            {
                _inputSystems[i].enabled = true;
            }
        }
    }
}
