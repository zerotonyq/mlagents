using System;
using CameraRaycasting.Interactables.Base;
using DG.Tweening;
using UnityEngine;

namespace CameraRaycasting.Interactables
{
    public class Button3D : MonoBehaviour, Interactable
    {
        public Action Triggered { get; set; }

        public Sequence currentSequence;

        public void Interact()
        {
            if (currentSequence.IsActive())
                return;

            Triggered += () => Debug.Log("button pressed");
            var startPosition = transform.position;
            var endPosition = startPosition - 0.1f * transform.up;
            currentSequence = DOTween.Sequence()
                .Append(transform.DOMoveY((transform.position - 0.1f * transform.up).y, 0.1f))
                .AppendCallback(() => Triggered?.Invoke())
                .Append(transform.DOMoveY(transform.position.y, 0.1f));
        }
    }
}