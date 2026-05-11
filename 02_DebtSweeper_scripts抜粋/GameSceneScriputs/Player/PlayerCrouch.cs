using UnityEngine;

public class PlayerCrouch : MonoBehaviour
{
    [SerializeField] private CapsuleCollider _playerCollider = default;
    [SerializeField] private BoxCollider _crouchCollider = default;

    public void CrouchPlayer(bool isCrouchButtonPressed)
    {
        if (isCrouchButtonPressed)
        {
            _playerCollider.enabled = false;
            _crouchCollider.enabled = true;
        }
        else
        {
            _playerCollider.enabled = true;
            _crouchCollider.enabled = false;
        }
    }
}
