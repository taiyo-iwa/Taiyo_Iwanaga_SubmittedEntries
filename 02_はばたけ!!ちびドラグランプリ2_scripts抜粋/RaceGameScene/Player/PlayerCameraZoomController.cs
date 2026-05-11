using UnityEngine;
using Unity.Cinemachine;

public class PlayerCameraZoomController : MonoBehaviour
{
    private const float CAMERA_FOLLOW_SPEED = 2.0f;

    private int _cameraSetIndex = 1;
    private bool _isPressedRock = false;
    [SerializeField] private PlayerStatus _playerStatus = default;
    [SerializeField] CinemachineOrbitalFollow _playerFollowCamera = default;
    [SerializeField] float[] cameraYPostionList = new float[] { 3.0f, 9.0f, 18.0f, 36.0f};
    [SerializeField] float[] cameraRadiusList = new float[] { 15.5f, 12.5f, 8.0f, 0.0f};

    public void UpdatePlayerCameraZoomController()
    {
        if (!_playerStatus.CanMove)
        {
            return;
        }
        CameraController();
    }

    public void PlayerLookInput(Vector2 lookInput)
    {
        if (!_playerStatus.CanMove)
        {
            return;
        }

        //“ü—ÍƒŠƒZƒbƒg
        if (lookInput.y == 0.0)
        {
            _isPressedRock = false;
            return;
        }

        if(lookInput.y > 0.0)
        {
            if (_isPressedRock)
            {
                return;
            }

            _cameraSetIndex--;
            _isPressedRock = true;
            if (_cameraSetIndex < 0)
            {
                _cameraSetIndex = 0;
            }
        }
        else if(lookInput.y < 0.0)
        {
            if (_isPressedRock)
            {
                return;
            }

            _cameraSetIndex++;
            _isPressedRock = true;
            if (_cameraSetIndex >= cameraYPostionList.Length)
            {
                _cameraSetIndex = cameraYPostionList.Length - 1;
            }
        }
    }

    private void CameraController()
    {
        _playerFollowCamera.TargetOffset.y = Mathf.Lerp(_playerFollowCamera.TargetOffset.y, cameraYPostionList[_cameraSetIndex], CAMERA_FOLLOW_SPEED * Time.deltaTime);
        _playerFollowCamera.Radius = Mathf.Lerp(_playerFollowCamera.Radius, cameraRadiusList[_cameraSetIndex], CAMERA_FOLLOW_SPEED * Time.deltaTime);
    }
}
