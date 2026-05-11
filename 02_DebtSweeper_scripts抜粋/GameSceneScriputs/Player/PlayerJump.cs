using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private const float JUMPFORCE = 10.0f;
    private const float JUMPGRAVITY = 15.0f;

    Rigidbody _rigidbody = default;
    PlayerGroundCheck _playerGroundCheck = default;

    public void PlayerJumpStart(PlayerGroundCheck playerGroundCheck, Rigidbody rigidbody)
    {
        _playerGroundCheck = playerGroundCheck;
        _rigidbody = rigidbody;
    }
    public void JumpPlayer()
    {
        if (_playerGroundCheck.GroundCheckPlayer())
        {
            Physics.gravity = new Vector3(0.0f, -(JUMPGRAVITY), 0.0f);
            _rigidbody.AddForce(Vector3.up * JUMPFORCE, ForceMode.Impulse);
        }
    }
}
