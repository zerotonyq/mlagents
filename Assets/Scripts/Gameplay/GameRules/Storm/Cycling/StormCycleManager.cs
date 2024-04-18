using DefaultNamespace.Storm.Moving;
using Zenject;

namespace DefaultNamespace.Storm
{
    public class StormCycleManager
    {
        private readonly Timer.Timer _cycleTimer;
        private readonly StormMovingManager _stormMovingManager;
        
        [Inject]
        public StormCycleManager(Timer.Timer cycleTimer)
        {
            _stormMovingManager = StormMovingManager.Construct(); 
            
            _cycleTimer = cycleTimer;
            
            _cycleTimer.SetLooped(true);
        }
        
        
    }
}