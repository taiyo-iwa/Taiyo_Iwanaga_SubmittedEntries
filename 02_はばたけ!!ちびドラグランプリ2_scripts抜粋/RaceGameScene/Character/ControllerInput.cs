using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;
using System;

public class ControllerInput : MonoBehaviour
{
    [SerializeField] private InputActionReference _moveAction = default;
    [SerializeField] private InputActionReference _lookAction = default;
    [SerializeField] private InputActionReference _jumpAction = default;
    

    private Subject<Vector2> _moveStream = new Subject<Vector2>();
    private Subject<Vector2> _lookStream = new Subject<Vector2>();
    private Subject<bool> _jumpStream = new Subject<bool>();

    public IObservable<Vector2> OnMove
    {
        get { return _moveStream; }
    }

    public IObservable<Vector2> OnLook
    {
        get { return _lookStream; }
    }

    public IObservable<bool> OnJump
    {
        get { return _jumpStream; }
    }

    private void OnEnable()
    {
        _moveAction.action.Enable();
        _lookAction.action.Enable();
        _jumpAction.action.Enable();
        _jumpAction.action.started += _ => _jumpStream.OnNext(true);
        _jumpAction.action.canceled += _ => _jumpStream.OnNext(false);
    }

    private void OnDisable()
    {
        _moveAction.action.Disable();
        _lookAction.action.Disable();
        _jumpAction.action.Disable();
    }

    public void UpdateControllerInput()
    {
        Vector2 move = _moveAction.action.ReadValue<Vector2>();
        _moveStream.OnNext(move);

        Vector2 look = _lookAction.action.ReadValue<Vector2>();
        _lookStream.OnNext(look);
    }
}
