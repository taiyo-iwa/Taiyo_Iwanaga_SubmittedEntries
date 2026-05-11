using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    [SerializeField] private PlayerStateMachine _playerStateMachine = default;
    [SerializeField] private AudioSource _audioSource = default;
    [SerializeField] private AudioClip _walkClip = default;

    public void PlayerAudioControllerUpdate()
    {
        PlayerState currentState = _playerStateMachine.PlayerState;

        switch (currentState)
        {
            case PlayerState.Idle:
                _audioSource.Stop();
                break;
            case PlayerState.Walk:
                _audioSource.clip = _walkClip;
                _audioSource.Play();
                break;
            case PlayerState.Run:
                break;
            case PlayerState.Crouch:
                break;
            case PlayerState.Jump:
                break;
            case PlayerState.Stun:
                break;
            case PlayerState.Death:
                break;
        }
    }
}
