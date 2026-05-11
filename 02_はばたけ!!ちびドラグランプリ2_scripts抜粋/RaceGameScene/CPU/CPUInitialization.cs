using UnityEngine;

public class CPUInitialization : MonoBehaviour
{
    [SerializeField] private RaceStatus _raceStatus = default;
    [SerializeField] private CPUStatus _cpuStatus = default;
    [SerializeField] private CPUController _cpuController = default;
    [SerializeField] private CPUMove _cpuMove = default;
    [SerializeField] private CPUChargeDash _cpuChargeDash = default;
    [SerializeField] private CPUEffectController _cpuEffectController = default;
    [SerializeField] private CPUAudioController _cpuAudioController = default;
    [SerializeField] private CPUStepControl _stepControl = default;
    [SerializeField] private CPUAnimationController _cpuAnimation = default;
    


    public void Awake()
    {
        _cpuStatus.Initialize(_raceStatus);
    }

    public void Start()
    {
        _cpuController.CPUControllerStart();
        _cpuMove.StartCPUMove();
        _cpuEffectController.StartCPUEffectController();
        _cpuAudioController.StartCPUAudioController();
        _cpuAnimation.StartCpuAnimationController();
    }

    public void Update()
    {
        _cpuController.CPUControllerUpdate();
        _cpuMove.UpdateCPUMove();
        _cpuChargeDash.UpdateCPUChargDash();
        _cpuEffectController.UpdateCPUEffectController();
        _cpuAudioController.UpdateCPUAudioController();
        _cpuAnimation.UpdateCPUAnimationController();
        _stepControl.StepSolution();
    }

    public void FixedUpdate()
    {
        _cpuMove.FixedUpdateCPUMove();
    }
}
