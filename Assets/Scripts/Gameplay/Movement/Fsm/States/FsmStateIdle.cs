using FsmBase;
using Movement.Fsm.States.Base;
using Movement.Input.Base;
using UnityEngine;

namespace Movement.Fsm.States
{
    public class FsmStateIdle : FsmStateMovement
    {
        public FsmStateIdle(FsmBase.Fsm fsm, 
            IMovementInputManager movementInputManager, 
            Rigidbody rb,
            float accelerationRate) : 
            base(fsm, movementInputManager, rb, accelerationRate) { }
        
        public override void Enter()
        {
            //Debug.Log("idle state [ENTER]");
        }

        public override void Exit()
        {
            //Debug.Log("idle state [EXIT]");
        }

        public override void Update()
        {
            if(MovementInputManager.ReadMoveDirection().magnitude > 0.05f)
                Fsm.SetState<FsmStateWalk>();
        }
    }
}