using System;
using CameraRaycasting.Interactables;
using CameraRaycasting.Interactables.Base;
using UnityEditor;
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
            _playerInputActions.Menu.Click.performed += Raycast;
            _playerInputActions.Menu.MousePosition.Enable();
        }

        RaycastHit[] rh = new RaycastHit[5];
        private void Raycast(InputAction.CallbackContext ctx)
        {
            for (int i = 0; i < rh.Length; ++i)
            {
                rh[i] = default;
            }
            
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);//_playerInputActions.Menu.MousePosition.ReadValue<Vector2>());
            
            Physics.RaycastNonAlloc(r, rh, 1000f);
            
            for (int i = 0; i < rh.Length; ++i)
            {
                if (rh[i].collider != null && rh[i].collider.TryGetComponent(out Interactable inter))
                {
                    inter.Interact();
                }    
            }
        }
    }
}