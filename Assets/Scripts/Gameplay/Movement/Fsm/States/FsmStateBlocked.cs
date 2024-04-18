using Movement.Fsm.States.Base;
using Movement.Input.Base;
using UnityEngine;

namespace Movement.Fsm.States
{
    public class FsmStateBlocked : FsmStateMovement
    {
        public FsmStateBlocked(FsmBase.Fsm fsm, 
            IMovementInputManager movementInputManager, 
            Rigidbody targetRigidbody, 
            float accelerationRate) : 
            base(fsm, movementInputManager, targetRigidbody, accelerationRate)
        {
        }

        public override void Enter()
        {
            Debug.Log("blocked state [ENTER]");
        }

        public override void Exit()
        {
            Debug.Log("blocked state [EXIT]");
            Fsm.SetState<FsmStateIdle>();
        }

        public override void Update()
        {
            
        }
    }
}