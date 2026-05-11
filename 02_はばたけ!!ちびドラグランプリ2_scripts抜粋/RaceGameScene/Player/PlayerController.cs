using UnityEngine;
using UniRx;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private ControllerInput _controllerInput = default;
    [SerializeField] private PlayerStatus _playerStatus = default;
    [SerializeField] private PlayerMove _playerMove = default;
    [SerializeField] private PlayerChargeDash _playerChargeDash = default;
    [SerializeField] private PlayerCameraZoomController _playerCameraZoomController = default;

    public void StartPlayerController()
    {
        _controllerInput.OnMove
        .Subscribe(move => MoveInput(move))
        .AddTo(this);

        _controllerInput.OnLook
        .Subscribe(look => LookInput(look))
        .AddTo(this);

        _controllerInput.OnJump
        .Subscribe(isSouthButton => SouthButtonInput(isSouthButton))
        .AddTo(this);
    }

    private void MoveInput(Vector2 moveInput)
    {
        _playerMove.PlayerMoveInput(moveInput);
        //X•ûŒü‚Ì“ü—Í‚ðPlayerState‚É“n‚·
        PassInputHorizontal(moveInput.x);
    }

    public void LookInput(Vector2 lookInput)
    {
        _playerCameraZoomController.PlayerLookInput(lookInput);
    }

    public void SouthButtonInput(bool southButtonInput)
    {
        _playerMove.PlayerSouthButtonInput(southButtonInput);
        _playerChargeDash.PlayerSouthButtonInput(southButtonInput);
        PassInputSouthButton(southButtonInput);
    }

    //PlayerStatus‚ÉX•ûŒü‚Ì“ü—Í‚ðŽó‚¯“n‚·
    private void PassInputHorizontal(float inputHorizontal)
    {
        _playerStatus.UpdateInputHorizontal(inputHorizontal);
    }

    //PlayerStatus‚É‰º•ûŒü‚Ìƒ{ƒ^ƒ“‚Ì“ü—Í‚ðŽó‚¯“n‚·
    private void PassInputSouthButton(bool southButtonInput)
    {
        _playerStatus.UpdateSouthButton(southButtonInput);
    }
}
