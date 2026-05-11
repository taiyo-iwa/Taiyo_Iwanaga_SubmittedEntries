using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerInput : MonoBehaviour
{
    private PlayerInputHandler _playerInputHandler;
    private PlayerMove _playerMove;
    private Vector2 _moveInputValue;
    private float _accelInputValue;
    private float _soarInputValue;
    private float _driftInputValue;
    private float _dashInputValue;
    private float _itemInputValue;
    private float _itemTwoInputValue;

    void Start()
    {
        _playerInputHandler = GetComponent<PlayerInputHandler>();
        _playerMove = GetComponent<PlayerMove>();
    }

    void Update()
    {
        _playerInputHandler.UpdateInput(
            horizontal: _moveInputValue.x,
            vertical: _moveInputValue.y,
            leftTrigger: _itemTwoInputValue,
            rightTrigger: _dashInputValue,
            accel: _accelInputValue,
            fly: _soarInputValue,
            drift: _driftInputValue,
            item: _itemInputValue
            );
    }

    private void FixedUpdate()
    {
        _playerMove.PlayFixedUpdate();
    }

    // メソッド名は何でもOK
    // publicにする必要がある
    /// <summary>
    /// InputActionからの入力値を受け取るメソッド
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInputValue = context.ReadValue<Vector2>();
    }

    public void OnAccel(InputAction.CallbackContext context)
    {
        _accelInputValue = context.ReadValue<float>();
    }
    public void OnSoar(InputAction.CallbackContext context)
    {
        _soarInputValue = context.ReadValue<float>();
    }
    public void OnDrift(InputAction.CallbackContext context)
    {
        _driftInputValue = context.ReadValue<float>();
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        _dashInputValue = context.ReadValue<float>();
    }
    public void OnItem(InputAction.CallbackContext context)
    {
        _itemInputValue = context.ReadValue<float>();
    }
    public void OnItemTwo(InputAction.CallbackContext context)
    {
        _itemTwoInputValue = context.ReadValue<float>();
    }
}
