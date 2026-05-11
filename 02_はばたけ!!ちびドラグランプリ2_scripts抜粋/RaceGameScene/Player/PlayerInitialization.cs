using UnityEngine;

public class PlayerInitialization : MonoBehaviour
{
    [SerializeField] private PlayerStatus _playerStatus = default;
    [SerializeField] private ControllerInput _controllerInput = default;
    [SerializeField] private PlayerController _playerController = default;
    [SerializeField] private PlayerMove _playerMove = default;
    [SerializeField] private PlayerChargeDash _playerChargeDash = default;
    [SerializeField] private PlayerEffectController _playerEffectController = default;
    [SerializeField] private PlayerAudioController _playerAudioController = default;
    [SerializeField] private PlayerAnimationController _playerAnimationController = default;
    [SerializeField] private PlayerCameraZoomController _playerCameraZoomController = default;
    [SerializeField] private PlayerStepControl _playerStepControl = default;

    public void AwakePlayer(RaceStatus raceStatus)
    {
        _playerStatus.Initialize(raceStatus);
    }

    public void StartPlayer()
    {
        _playerController.StartPlayerController();
        _playerAudioController.StartPlayerAudioController();
        _playerAnimationController.StartPlayerAnimationController();
        _playerEffectController.StartPlayerEffectController();
    }

    public void UpdatePlayer()
    {
        _playerMove.UpdatePlayerMove();
        _controllerInput.UpdateControllerInput();
        _playerChargeDash.UpdatePlayerChargDash();
        _playerEffectController.UpdatePlayerEffectController();
        _playerAudioController.UpdatePlayerAudioController();
        _playerAnimationController.UpdatePlayerAnimationController();
        _playerCameraZoomController.UpdatePlayerCameraZoomController();
        _playerStepControl.StepSolution();
    }

    public void FixedUpdatePlayer()
    {
        _playerMove.FixedUpdatePlayerMove();
    }
}
