using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class RaceFinishView : MonoBehaviour
{
    private const string FINISH = "Finish";

    [SerializeField] private RaceStatus _raceStatus = default;
    [SerializeField] private Image _finishLogo = default;
    [SerializeField] private Animator _finishLogoAnimator = default;
    [SerializeField] private AudioSource _audioSource = default;
    [SerializeField] private AudioClip _finishSE = default;

    public void Start()
    {
        _finishLogo.enabled = false;

        _raceStatus.OnStateChanged
        .Where(state => state == RaceState.Finish)
        .Subscribe(_ => { RaceFinish(); })
        .AddTo(this);
    }

    public void RaceFinish()
    {
        _finishLogo.enabled = true;
        _audioSource.Stop();
        _audioSource.PlayOneShot(_finishSE);
        _finishLogoAnimator.SetBool(FINISH, true);
    }
}
 