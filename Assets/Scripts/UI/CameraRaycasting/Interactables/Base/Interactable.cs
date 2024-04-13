using System;
using UnityEngine;

namespace CameraRaycasting.Interactables.Base
{
    public abstract class Interactable : MonoBehaviour
    {
        public Action Triggered { get; set; }
        public void Interact() => Triggered?.Invoke();
    }
}