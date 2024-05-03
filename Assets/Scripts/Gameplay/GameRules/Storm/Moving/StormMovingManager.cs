using DefaultNamespace.Movement.Input;
using Movement.Fsm.States;
using Movement.Fsm.View;
using UnityEngine;
using Zenject;

namespace DefaultNamespace.Storm.Moving
{
    public class StormMovingManager : ITickable
    {
        private FsmMovementView _movementView;
        
        [Inject]
        public void Initialize()
        {
            _movementView = new GameObject("MovementStorm").AddComponent<FsmMovementView>();
            _movementView.Initialize(new ConstantMovementInputManager(new Vector2(0,1)));
            _movementView.SetKinematic(true);
            _movementView.Fsm.SetState<FsmStateBlocked>();
        }

        public void StartMoving()
        {
            Debug.Log("start moving storm");
            _movementView.Fsm.SetState<FsmStateWalk>();
        }

        public void StopMoving()
        {
            _movementView.Fsm.SetState<FsmStateBlocked>();
        }

        public void Tick()
        {
            _movementView.Fsm.Update();
        }
    }
}