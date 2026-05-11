using UnityEngine;
using UniRx;
using Unity.Cinemachine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TimeAttackResultManager : MonoBehaviour
{
    private const float FOLLOW_ADMISSIBLE_RATIO = 5.0f;

    [SerializeField] private CinemachineCamera _cinemachineCamera = default;
    [SerializeField] private CinemachineOrbitalFollow _playerFollowCamera = default;
    [SerializeField] private GameObject _resultText = default;
    [SerializeField] private GameObject _nextButton = default;
    [SerializeField] private GameObject _reStartButton = default;
    [SerializeField] private Image _finishLogo = default;
    [SerializeField] private Text _recordTimeText = default;
    [SerializeField] private Vector3 _targetOffset = new Vector3(-25.0f, 60.0f, 20.0f);

    private bool _isCameraFollow = false;
    private float _cameraFollowSpeed = 1.0f;

    public void CameraFollow()
    {
        _isCameraFollow = true;
    }

    public void RecordTimeText(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        int milliSeconds = Mathf.FloorToInt(time * 100 % 100);

        _recordTimeText.text = string.Format("{0:D1}'{1:D2}''{2:D2}", minutes, seconds, milliSeconds);
    }

    public void UpdateResultCameraFollow()
    {
        if(!_isCameraFollow)
        {
            return;
        }

        _playerFollowCamera.TargetOffset = Vector3.Lerp(_playerFollowCamera.TargetOffset, _targetOffset, _cameraFollowSpeed * Time.deltaTime);
        if (Mathf.Abs(_targetOffset.x - _playerFollowCamera.TargetOffset.x) <= FOLLOW_ADMISSIBLE_RATIO)
        {
            if(Mathf.Abs(_targetOffset.y - _playerFollowCamera.TargetOffset.y) <= FOLLOW_ADMISSIBLE_RATIO)
            {
                _isCameraFollow = false;
                _cinemachineCamera.enabled = false;
                _finishLogo.enabled = false;
                _resultText.SetActive(true);
                _nextButton.SetActive(true);
                _reStartButton.SetActive(true);
                EventSystem.current.SetSelectedGameObject(_nextButton.gameObject);
            } 
        }
    }
}
