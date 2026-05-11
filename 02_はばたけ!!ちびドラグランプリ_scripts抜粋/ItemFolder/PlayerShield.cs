using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    public bool isShield { get; private set; } = false;

    private GameObject activeShield;
    private GameObject activeFlame;

    public void ActivateShield(GameObject shieldPrefab, GameObject flamePrefab, float duration)
    {
        if (activeShield != null || activeFlame != null)
        {
            Destroy(activeShield); // すでにあるなら消す
            Destroy(activeFlame);
        }

        // プレイヤーの位置にシールドを生成（親をプレイヤーにする）
        activeShield = Instantiate(shieldPrefab, transform.position + new Vector3(0f,0.5f,0f), Quaternion.identity, transform);
        activeFlame = Instantiate(flamePrefab, transform.position, Quaternion.Euler(-90f,0f,0f), transform);

        isShield = true;

        // 一定時間後にシールド解除
        StartCoroutine(ShieldDuration(duration));
    }

    private IEnumerator ShieldDuration(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (activeShield != null || activeFlame != null)
        {
            Destroy(activeShield);
            Destroy(activeFlame);
            activeShield = null;
            activeFlame = null;
            isShield = false;
            Debug.Log("シールド終了");
        }
    }
}
