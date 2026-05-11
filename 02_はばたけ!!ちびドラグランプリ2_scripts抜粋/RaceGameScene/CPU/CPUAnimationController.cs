using UnityEngine;

public class CPUAnimationController : MonoBehaviour
{
    //アニメーション名
    private const string RUN_SPEED = "Speed";
    private const string INPUT_HORIZONTAL = "Horizontal";

    [SerializeField] CPUStatus _cpuStatus = default;
    [SerializeField] Transform _cpuTransform = default;

    private Animator _cpuAnimator = default;

    public void StartCpuAnimationController()
    {
        _cpuAnimator = _cpuTransform.GetComponentInChildren<Animator>();
    }

    public void UpdateCPUAnimationController()
    {
        //_playerAnimator.SetFloat(INPUT_HORIZONTAL, _playerStatus.InputHorizontal);
        _cpuAnimator.SetFloat(RUN_SPEED, _cpuStatus.RunSpeed);
    }
}
