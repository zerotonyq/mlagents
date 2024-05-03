using FsmBase;

namespace DefaultNamespace.Storm.Fsm.States
{
    public abstract class FsmStateGameplay : FsmState
    {
        protected Timer.Timer _timer;
        private float _stateTime;
        protected FsmStateGameplay(FsmBase.Fsm fsm, Timer.Timer timer, float stateTime) : base(fsm)
        {
            _timer = timer;
            _stateTime = stateTime;
        }

        public override void Enter()
        {
            _timer.StopTimer();
            _timer.ResetCurrentTime();
            _timer.SetTime(_stateTime);
            _timer.StartTimer();
            
        }

        public override void Exit()
        {
            
        }
    }
}