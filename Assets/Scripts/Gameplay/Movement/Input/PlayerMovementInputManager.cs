using System;
using System.Collections;
using System.Collections.Generic;
using Movement.Input.Base;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementInputManager : IMovementInputManager
{
    private PlayerInputActions _playerInputActions;

    public PlayerMovementInputManager()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
    }

    public Vector2 ReadMoveDirection() => _playerInputActions.OnFeet.Walk.ReadValue<Vector2>();
}
