using DefaultNamespace.Storm.Fsm.States;
using DefaultNamespace.StormFlower;
using UnityEngine;

namespace Gameplay.Fsm.States
{
    public class FsmStateWaitStorm : FsmStateGameplay
    {
        private StormFlowerGraphManager _stormFlowerGraphManager;
        public FsmStateWaitStorm(FsmBase.Fsm fsm, Timer.Timer timer, float stateTime,
            StormFlowerGraphManager stormFlowerGraphManager) : base(fsm, timer, stateTime)
        {
            _stormFlowerGraphManager = stormFlowerGraphManager;
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("wait storm");
            _timer.EndTimer += Fsm.SetState<FsmStateBeforeStorm>;
            _stormFlowerGraphManager.DeactivateFlowers();
        }

        public override void Exit()
        {
            base.Exit();
            _timer.EndTimer -= Fsm.SetState<FsmStateBeforeStorm>;
        }

        public override void Update()
        {
            base.Update();
        }
    }
}