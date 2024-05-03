using FsmBase;
using Movement.Fsm.States.Base;
using Movement.Input.Base;
using UnityEngine;

namespace Movement.Fsm.States
{
    public class FsmStateWalk : FsmStateMovement
    {

        public FsmStateWalk(FsmBase.Fsm fsm,
            IMovementInputManager movementInputManager,
            Rigidbody rb,
            float accelerationRate) :
            base(fsm, movementInputManager, rb, accelerationRate)
        {
        }

        public override void Enter()
        {
            //.Log("walk state [ENTER]");
        }

        public override void Exit()
        {
            //Debug.Log("walk state [EXIT]");
        }

        public override void Update()
        {
            var currentDirection = MovementInputManager.ReadMoveDirection(); 
            if(currentDirection.magnitude < 0.05f)
                Fsm.SetState<FsmStateIdle>();

            Move(currentDirection);
        }

        private void Move(Vector2 direction)
        {
            if (_rigidbody.isKinematic)
            {
                _rigidbody.transform.position += 
                    new Vector3(direction.x * _accelerationRate, 0, direction.y * _accelerationRate) * Time.deltaTime;
            }
            else
            {   
                _rigidbody.velocity = new Vector3(
                direction.x * _accelerationRate, 
                _rigidbody.velocity.y,
                direction.y * _accelerationRate);
                
            }
        }
    }
}