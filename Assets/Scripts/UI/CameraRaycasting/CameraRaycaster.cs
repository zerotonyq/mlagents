using System;
using CameraRaycasting.Interactables;
using CameraRaycasting.Interactables.Base;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace CameraRaycasting
{
    public class CameraRaycaster
    {
        private PlayerInputActions _playerInputActions;
        [Inject]
        public CameraRaycaster(PlayerInputActions playerInputActions)
        {
            _playerInputActions = playerInputActions;
            _playerInputActions.Menu.Enable();
            //_playerInputActions.Menu.Click += Raycast;
            _playerInputActions.Menu.MousePosition.Enable();
        }
        
        
        public void Raycast(InputAction.CallbackContext ctx)
        {
            RaycastHit rh;
            Camera.main.ScreenPointToRay(_playerInputActions.Menu.MousePosition.ReadValue<Vector2>());
                //Ray r = new Ray()
            //Physics.RaycastNonAlloc()
        }
    }
}