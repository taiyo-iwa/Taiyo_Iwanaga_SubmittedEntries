using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimmickManager : MonoBehaviour
{
    //プレイヤーのドラゴンは何番か調べるのに必要
    [SerializeField] private GameManager _gameManager = default;
    [SerializeField]
    private ParticlManager _particlManager;
    [SerializeField]
    private GameObject[] _dragons;
    [SerializeField]
    private PlayerAnimationController[] _playerAnimationControllers;

    public enum GimmickType
    {
        DashPad,
        JumpPad,
        DamageWall
    }
    public static GimmickManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ActivateGimmick(GimmickType type, GameObject playerObj, float power, float _maxPower, Transform gimmickOrigin)
    {
        var rb = playerObj.GetComponent<Rigidbody>();
        var playerController = playerObj.GetComponent<PlayerController>();
        var playerSoundController = playerObj.GetComponent<PlayerSoundController>();

        //平面上のvelocityの向きを取得（Yを考慮しない）
        //Vector3 moveDir = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        Vector3 forward = playerObj.transform.forward;
        forward.y = 0f;
        forward = forward.normalized;
        switch (type)
        {
            case GimmickType.DashPad:
                if (rb != null)
                    rb.AddForce(forward * power, ForceMode.Impulse);
                if (playerObj == _dragons[_gameManager.SelectDragon])
                    _particlManager.StartHyperDrive(_gameManager.SelectDragon, false);
                if (_gameManager._twoPlayerSelectDragon >= 0 && playerObj == _dragons[_gameManager._twoPlayerSelectDragon])
                    _particlManager.StartHyperDrive(_gameManager._twoPlayerSelectDragon, false);
                if (playerController)
                    playerController.CameraFovUp();
                if (playerSoundController)
                    playerSoundController.DashSound();
                break;

            case GimmickType.JumpPad:
                if (rb != null)
                    rb.AddForce(gimmickOrigin.up * power, ForceMode.Impulse);
                if (playerSoundController)
                    playerSoundController.JumpSound();
                if (playerObj == _dragons[_gameManager.SelectDragon])
                {
                    _playerAnimationControllers[_gameManager.SelectDragon].StopDrift();
                    _playerAnimationControllers[_gameManager.SelectDragon].StartHappy();
                    _particlManager.StartHyperDrive(_gameManager.SelectDragon, false);
                }
                if (_gameManager._twoPlayerSelectDragon >= 0 && playerObj == _dragons[_gameManager._twoPlayerSelectDragon])
                {
                    _playerAnimationControllers[_gameManager._twoPlayerSelectDragon].StopDrift();
                    _playerAnimationControllers[_gameManager._twoPlayerSelectDragon].StartHappy();
                    _particlManager.StartHyperDrive(_gameManager._twoPlayerSelectDragon, false);
                }
                break;

            case GimmickType.DamageWall:
                break;
        }
    }
}
