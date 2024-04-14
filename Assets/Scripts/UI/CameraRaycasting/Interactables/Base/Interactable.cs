using System;
using DG.Tweening;
using UnityEngine;

namespace CameraRaycasting.Interactables.Base
{
    public interface Interactable
    {
        public Action Triggered { get; set; }

        public virtual void Interact()
        {
        }
    }
}