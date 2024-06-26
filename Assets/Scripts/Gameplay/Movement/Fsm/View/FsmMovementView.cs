﻿using System;
using DefaultNamespace;
using Movement.Fsm.States;
using Movement.Fsm.States.Base;
using Movement.Input.Base;
using UnityEngine;
using Zenject;

namespace Movement.Fsm.View
{
    [RequireComponent(typeof(Rigidbody))]
    public class FsmMovementView : MonoBehaviour
    {
        [SerializeField] private float accelerationRate = 10f;
        
        private FsmBase.Fsm _fsm;

        private IMovementInputManager _movementInputManager;
        
        private bool _initialized = false;
        private Rigidbody _rigidbody;
        public void SetKinematic(bool i) => _rigidbody.isKinematic = i; 
        public void Initialize(IMovementInputManager movementInputManager)
        {
            _movementInputManager = movementInputManager;
            
            _fsm = new FsmBase.Fsm();
            
            _rigidbody = GetComponent<Rigidbody>();
            
            _fsm.AddState(new FsmStateIdle(_fsm, _movementInputManager, _rigidbody, accelerationRate));
            _fsm.AddState(new FsmStateWalk(_fsm, _movementInputManager, _rigidbody, accelerationRate));
            _fsm.AddState(new FsmStateBlocked(_fsm, _movementInputManager, _rigidbody, accelerationRate));
            
            _fsm.SetState<FsmStateIdle>();
            
            _initialized = true;
        }

        private void Update()
        {
            if(!_initialized)
                return;
            
            _fsm.Update();
        }
        
        public FsmBase.Fsm Fsm => _fsm;
    }
}