using DefaultNamespace.Storm.Fsm.States;
using DefaultNamespace.Storm.Moving;
using Gameplay.Fsm.States;
using UnityEngine;

namespace Gameplay.Fsm
{
    public class FsmStateStorm : FsmStateGameplay
    {
        private StormMovingManager _stormMovingManager;
        public FsmStateStorm(FsmBase.Fsm fsm, Timer.Timer timer, float stateTime, StormMovingManager stormMovingManager) : base(fsm, timer, stateTime)
        {
            _stormMovingManager = stormMovingManager;
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("storm");
            _timer.EndTimer += Fsm.SetState<FsmStateWaitStorm>;
            _stormMovingManager.StartMoving();
        }

        public override void Exit()
        {
            base.Exit();
            _timer.EndTimer -= Fsm.SetState<FsmStateWaitStorm>;
            _stormMovingManager.StopMoving();
        }

        public override void Update()
        {
            base.Update();
        }
    }
}