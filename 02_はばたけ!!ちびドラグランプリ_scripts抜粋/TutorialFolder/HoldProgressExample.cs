using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HoldProgressExample : MonoBehaviour
{
    // 入力を受け取る対象のAction
    [SerializeField] private InputActionReference _hold;

    private InputAction _holdAction;

    private void Awake()
    {
        if (_hold == null) return;

        _holdAction = _hold.action;

        // 入力を受け取るためには必ず有効化する必要がある
        _holdAction.Enable();
    }

    private void Update()
    {
        if (_holdAction == null) return;

        // 長押しの進捗を取得
        var progress = _holdAction.GetTimeoutCompletionPercentage();

        // 進捗をログ出力
        Debug.Log($"Progress : {progress * 100}%");
    }
}
