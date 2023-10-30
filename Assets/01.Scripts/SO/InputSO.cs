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
    public event Action OnJumpKeyPress;
    public event Action<Vector2> OnMovementKeyPress; 
    public void OnEnable()
    {
        if (_playerControls == null)
        {
            _playerControls = new PlayerControls();
            _playerControls.Normal.SetCallbacks(this);
        }
        _playerControls.Normal.Enable();
    }


    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnJumpKeyPress?.Invoke();
        }
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        Vector2 inputValue = context.ReadValue<Vector2>();
        OnMovementKeyPress?.Invoke(inputValue);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnDashKeyPress?.Invoke();
        }
    }
}
