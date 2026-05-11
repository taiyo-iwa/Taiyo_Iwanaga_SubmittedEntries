using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeat : MonoBehaviour
{
    [SerializeField ]private int _duration = 5;
    [SerializeField] FireballManager _fireballManager;
    [SerializeField] PlayerSoundController _soundController;
    private GameObject activeShield;
    private PlayerAnimationController _animatorController = default;
    public bool isShield { get; private set; } = false;

    private void Start()
    {
        _animatorController = this.GetComponent<PlayerAnimationController>();
    }

    public void ActivateHeat(Transform playerTransForm, GameObject shieldPrefab, PlayerItem.ItemUseType itemType)
    {
        switch (itemType)
        {
            case PlayerItem.ItemUseType.Attack:
                _animatorController.FireballShoot();
                break;

            case PlayerItem.ItemUseType.Defense:
                ActivateShield(shieldPrefab);
                _soundController.PlayShieldSound();
                break;
        }
    }

    private void ActivateShield(GameObject shieldPrefab)
    {
        if (activeShield != null)
        {
            Destroy(activeShield); // すでにあるなら消す
        }

        // プレイヤーの位置にシールドを生成（親をプレイヤーにする）
        activeShield = Instantiate(shieldPrefab, transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity, transform);

        isShield = true;

        // 一定時間後にシールド解除
        StartCoroutine(ShieldDuration(shieldPrefab));
    }

    private IEnumerator ShieldDuration(GameObject shieldPrefab)
    {
        yield return new WaitForSeconds(_duration);

        if (activeShield != null)
        {
            Destroy(activeShield);
            activeShield = null;
            isShield = false;
            Debug.Log("シールド終了");
        }
    }

    public void ShootFireball()
    {
        _fireballManager.MakeFireball(transform);
        _soundController.PlayFireballSound();
    }
}
