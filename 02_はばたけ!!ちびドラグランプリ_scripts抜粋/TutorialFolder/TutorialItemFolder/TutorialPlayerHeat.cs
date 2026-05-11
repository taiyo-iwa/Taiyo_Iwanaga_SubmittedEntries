using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlayerHeat : MonoBehaviour
{
    private const int _duration = 5;
    [SerializeField] TutorialFireballManager _fireballManager;
    [SerializeField] TutorialSoundController _soundController;
    private GameObject activeShield;
    public bool isShield { get; private set; } = false;

    //アイテムを持っている時にアイテムを使ったか
    public bool _isFlameItemUse { get; set; } = false;
    public bool _isShieldItemUse { get; set; } = false;

    public void ActivateHeat(Transform playerTransForm, GameObject shieldPrefab, TutorialPlayerItem.ItemUseType itemType)
    {
        switch (itemType)
        {
            case TutorialPlayerItem.ItemUseType.Attack:
                _fireballManager.MakeFireball(playerTransForm);
                _soundController.PlayFireballSound();
                _isFlameItemUse = true;
                break;

            case TutorialPlayerItem.ItemUseType.Defense:
                ActivateShield(shieldPrefab);
                _soundController.PlayShieldSound();
                _isShieldItemUse = true;
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
}
