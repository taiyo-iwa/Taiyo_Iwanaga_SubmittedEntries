using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlayerHitAction : MonoBehaviour
{
    [SerializeField] private LayerMask _flameLayer;
    [SerializeField] private Vector3 _boxSize = new Vector3(0.5f, 0.1f, 0.5f);
    [SerializeField] private float _castDistance = 0.2f;
    [SerializeField] private Vector3 _castOffset = new Vector3(0, 0.1f, 0);
    //[SerializeField] private PlayerAnimationController _animator;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private TutorialPlayerHeat _shield;
    private bool _isFlameHit = false;
    private TutorialPlayerMovement _playerMovement = default;
    private TutorialSoundController _soundController = default;
    private TutorialPlayerMove _playerMove = default;

    private void Start()
    {
        _playerMove = GetComponent<TutorialPlayerMove>();
        _playerMovement = GetComponent<TutorialPlayerMovement>();
        _soundController = GetComponent<TutorialSoundController>();
    }
    void Update()
    {
        CheckFlamed();
    }

    #region フレアの接触判定
    //フレアにふれているか
    private void CheckFlamed()
    {
        Vector3 origin = transform.position + _castOffset;
        _isFlameHit = Physics.BoxCast(
            origin,
            _boxSize * 0.5f,//半径
            Vector3.back,
            Quaternion.Euler(0f, 66.319f, 0f),
            _castDistance,
            _flameLayer);
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position + _castOffset;
        Vector3 center = origin + Vector3.back * (_castDistance + _boxSize.y * 0.5f);
        Gizmos.color = _isFlameHit ? Color.green : Color.red;
        Gizmos.DrawWireCube(center, _boxSize);
    }
#endif
    #endregion


    /// <summary>
    /// フレアに当たった時の処理
    /// </summary>
    public void FlameHit()
    {
        //シールドを展開していない時かつ空中にいない時
        if (!_shield.isShield && !_playerMove.CheckAir())
        {
            _playerMovement.FireballDamage();
            _soundController.FireballHitSound();
            //_rb.velocity = Vector3.zero;
        }
    }
}
