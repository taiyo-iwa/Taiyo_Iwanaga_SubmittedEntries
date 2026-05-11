using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GetHoldExample : MonoBehaviour
{
    // 入力を受け取る対象のAction
    [SerializeField] private InputActionReference _hold;
    //シーン先の名前
    private string _charcterSelectScaneName = "CharacterSelectionScene";

    private void Awake()
    {
        if (_hold == null) return;

        // performedコールバックのみを受け取る
        // 長押し判定になったらこのコールバックが呼ばれる
        _hold.action.performed += OnHold;

        // 入力を受け取るためには必ず有効化する必要がある
        _hold.action.Enable();
    }

    // 長押しされたときに呼ばれるメソッド
    private void OnHold(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene(_charcterSelectScaneName);
    }
}
