using DefaultNamespace.Storm.Fsm.States;
using DefaultNamespace.StormFlower;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Gameplay.Fsm.States
{
    public class FsmStateBeforeStorm : FsmStateGameplay
    {
        private StormFlowerGraphManager _stormFlowerGraphManager;
        public FsmStateBeforeStorm(FsmBase.Fsm fsm, Timer.Timer timer, float stateTime,
            StormFlowerGraphManager stormFlowerGraphManager) : base(fsm, timer, stateTime)
        {
            _stormFlowerGraphManager = stormFlowerGraphManager;
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("before storm");
            _timer.EndTimer += Fsm.SetState<FsmStateStorm>;
            //TODO: flowers count to settings for level
            var count = UnityEngine.Random.Range(5, 11);
            _stormFlowerGraphManager.CreateFlowers(count);
        }

        public override void Exit()
        {
            base.Exit();
            _timer.EndTimer -= Fsm.SetState<FsmStateStorm>;
        }

        public override void Update()
        {
            base.Update();
        }
    }
}