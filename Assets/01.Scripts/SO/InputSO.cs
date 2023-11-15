using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
[CreateAssetMenu(menuName = "SO/Input")]
public class InputSO : ScriptableObject,PlayerControls.INormalActions, PlayerControls.IUXInputActions
{
    private PlayerControls _playerControls;
    
    public event Action OnDashKeyPress;
    public event Action OnDefendKeyPress;
    public event Action<Vector2> OnJumpKeyPress;
    public event Action<Vector2> OnMovementKeyPress;
    public event Action<Vector2> OnMouseAim;

    public event Action OnMenuIndexUpPress = null;
    public event Action OnMenuIndexDownPress = null;
    public event Action OnBackPress = null;
    public event Action OnEnterPress = null;
    public event Action OnRefreshPress = null;
    
    private Vector2 _inputValue = Vector2.zero;
    public Vector2 CurrentInputValue => _inputValue;
    private Vector2 _mousePos = Vector2.zero;
    public Vector2 CurrentMousePos => _mousePos;
    
    public void OnEnable()
    {
        if (_playerControls == null)
        {
            _playerControls = new PlayerControls();
            _playerControls.Normal.SetCallbacks(this);
            _playerControls.UXInput.SetCallbacks(this);
        }
        _playerControls.Normal.Enable();
        _playerControls.UXInput.Enable();
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

    public void OnDefend(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnDefendKeyPress?.Invoke();
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        _mousePos = context.ReadValue<Vector2>();
        OnMouseAim?.Invoke(_mousePos);
    }

    public void OnMenuIndexUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnMenuIndexUpPress?.Invoke();
        }
    }

    public void OnMenuIndexDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnMenuIndexDownPress?.Invoke();
        }
    }

    public void OnBack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnBackPress?.Invoke();
        }
    }

    public void OnEnter(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnEnterPress?.Invoke();
        }
    }

    public void OnRefresh(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnRefreshPress?.Invoke();
        }
    }
}
