using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Vector3 _boxSize = new Vector3(0.5f, 0.1f, 0.5f);
    [SerializeField] private float _castDistance = 0.2f;
    [SerializeField] private Vector3 _castOffset = new Vector3(0, 0.1f, 0);

    public bool IsGrounded { get; private set; }

    void Update()
    {
        CheckGrounded();
    }

    //’n–Ê‚É‚Ó‚ê‚Ä‚¢‚é‚©
    private void CheckGrounded()
    {
        Vector3 origin = transform.position + _castOffset;

        IsGrounded = Physics.BoxCast(
            origin,
            _boxSize * 0.5f,//”¼Œa
            Vector3.down,
            Quaternion.identity,
            _castDistance,
            _groundLayer);
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position + _castOffset;
        Vector3 center = origin + Vector3.down * (_castDistance + _boxSize.y * 0.5f);
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Gizmos.DrawWireCube(center, _boxSize);
    }
#endif
}
