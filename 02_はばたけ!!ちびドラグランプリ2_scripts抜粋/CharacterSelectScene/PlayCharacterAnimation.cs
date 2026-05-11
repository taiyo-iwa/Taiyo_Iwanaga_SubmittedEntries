using UnityEngine;

public class PlayCharacterAnimation : MonoBehaviour
{
    private const string SPEED = "Speed";

    [SerializeField] private Animator _animator = default;
    private void Start()
    {
        _animator.SetFloat(SPEED, 4);
    }
}
