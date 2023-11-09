using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
[CreateAssetMenu(menuName = "SO/Input")]
public class InputSO : ScriptableObject,PlayerControls.INormalActions
{
    private PlayerControls _playerControls;
    public event Action OnDashKeyPress;
    public event Action<Vector2> OnJumpKeyPress;
    public event Action<Vector2> OnMovementKeyPress;
    public event Action<Vector2> OnMouseAim;

    Vector2 _inputValue = Vector2.zero;
    public Vector2 CurrentInputValue => _inputValue;
    public void OnEnable()
    {
        if (_playerControls == null)
        {
            _playerControls = new PlayerControls();
            _playerControls.Normal.SetCallbacks(this);
        }
        _playerControls.Normal.Enable();
    }

    public void SetInput(bool enable)
    {
        var inputType = _playerControls.Normal;
        if (enable) inputType.Enable();
        else inputType.Disable();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnJumpKeyPress?.Invoke(_inputValue);
        }
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        _inputValue = context.ReadValue<Vector2>();
        OnMovementKeyPress?.Invoke(_inputValue);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnDashKeyPress?.Invoke();
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        Vector2 mouseValue = context.ReadValue<Vector2>();
        OnMouseAim?.Invoke(mouseValue);
    }
}
