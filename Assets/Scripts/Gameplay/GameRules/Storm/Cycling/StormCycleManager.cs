using DefaultNamespace.Storm.Moving;
using DefaultNamespace.StormFlower;
using Gameplay.Fsm;
using Gameplay.Fsm.States;
using UnityEngine;
using Zenject;

namespace DefaultNamespace.Storm
{
    public class StormCycleManager
    {
        private readonly Timer.Timer _cycleTimer;
        private readonly StormMovingManager _stormMovingManager;
        private StormFlowerGraphManager _stormFlowerGraphManager;
        
        [Inject]
        public StormCycleManager(Timer.Timer cycleTimer,
            StormFlowerGraphManager stormFlowerGraphManager,
            StormMovingManager stormMovingManager)
        {
            _stormMovingManager = stormMovingManager;
            _stormFlowerGraphManager = stormFlowerGraphManager;
            
            _cycleTimer = cycleTimer;
            _cycleTimer.SetLooped(false);

            CreateFsm();
        }

        private void CreateFsm()
        {
            
            var fsm = new FsmBase.Fsm();
            
            fsm.AddState(new FsmStateWaitStorm(fsm, _cycleTimer, 5f, _stormFlowerGraphManager));
            fsm.AddState(new FsmStateBeforeStorm(fsm, _cycleTimer, 20f, _stormFlowerGraphManager));
            fsm.AddState(new FsmStateStorm(fsm, _cycleTimer, 5f, _stormMovingManager));
            
            fsm.SetState<FsmStateWaitStorm>();
        }
        
    }
}