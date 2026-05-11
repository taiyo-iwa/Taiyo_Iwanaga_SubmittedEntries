using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    //アニメーション名
    private const string RUN_SPEED = "Speed";
    private const string INPUT_HORIZONTAL = "Horizontal";

    [SerializeField] PlayerStatus _playerStatus = default;
    [SerializeField] Transform _playerTransform = default;

    private Animator _playerAnimator = default;

    public void StartPlayerAnimationController()
    {
        _playerAnimator = _playerTransform.GetComponentInChildren<Animator>();
    }

    public void UpdatePlayerAnimationController()
    {
        //_playerAnimator.SetFloat(INPUT_HORIZONTAL, _playerStatus.InputHorizontal);
        _playerAnimator.SetFloat(RUN_SPEED, _playerStatus.RunSpeed);
    }
}
