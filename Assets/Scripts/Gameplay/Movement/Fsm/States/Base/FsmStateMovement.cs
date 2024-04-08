using FsmBase;
using Movement.Input.Base;
using UnityEngine;

namespace Movement.Fsm.States.Base
{
    public abstract class FsmStateMovement : FsmState
    {
        protected readonly IMovementInputManager MovementInputManager;

        protected readonly Rigidbody _rigidbody;
        protected float _accelerationRate = 0f;
        public FsmStateMovement(FsmBase.Fsm fsm, 
            IMovementInputManager movementInputManager, 
            Rigidbody targetRigidbody, 
            float accelerationRate) : base(fsm)
        {
            MovementInputManager = movementInputManager;
            _rigidbody = targetRigidbody;
            _accelerationRate = accelerationRate;
        }
    }
}