using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMove _playerMove;
    private PlayerJump _playerJump;
    private PlayerObjectGrab _playerObjectGrab;
    private PlayerCrouch _playerCrouch;

    public void PlayerControllerStart(PlayerMove playerMove, PlayerJump playerJump, PlayerObjectGrab playerObjectGrab, PlayerCrouch playerCrouch)
    {
        _playerMove = playerMove;
        _playerJump = playerJump;
        _playerObjectGrab = playerObjectGrab;
        _playerCrouch = playerCrouch;
    }
    public void UpdateMoveInput(float horizontal, float vertical)
    {
        _playerMove.MoveInput(horizontal, vertical);
    }
    public void UpdateMouseInput(float mouseHorizontal, float mouseVertical)
    {
        _playerMove.MouseInput(mouseHorizontal, mouseVertical);
    }

    public void JumpController()
    {
        _playerJump.JumpPlayer();
    }

    public void DashController(bool isDashButtonPressed)
    {
        _playerMove.DashInput(isDashButtonPressed);
    }

    public void CrouchController(bool isCrouchButtonPressed)
    {
        _playerMove.CrouchInput(isCrouchButtonPressed);
        _playerCrouch.CrouchPlayer(isCrouchButtonPressed);
    }

    public void GrabController(bool isMouseClick)
    {
        if (isMouseClick)
        {
            _playerObjectGrab.TryGrab();
        }
        else
        {
            _playerObjectGrab.Release();
        }
    }

    public void GrabDistanceController(float mouseScroll)
    {
        _playerObjectGrab.ChangeGrabDistance(mouseScroll);
    }
}
