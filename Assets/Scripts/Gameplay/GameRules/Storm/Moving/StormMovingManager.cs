using Movement.Fsm.View;
using UnityEngine;

namespace DefaultNamespace.Storm.Moving
{
    public class StormMovingManager
    {
        private readonly FsmMovementView _movementView;
        
        public StormMovingManager(FsmMovementView movementVIew)
        {
            _movementView = movementVIew;
        }

        public static StormMovingManager Construct()
        {
            var fsmMovement = new GameObject("MovementStorm").AddComponent<FsmMovementView>();
            return new StormMovingManager(fsmMovement);
        }
    }
}