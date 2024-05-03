using Movement.Input.Base;
using UnityEngine;

namespace DefaultNamespace.Movement.Input
{
    public class ConstantMovementInputManager : IMovementInputManager
    {
        private Vector2 _constantDirection;
        public ConstantMovementInputManager(Vector2 constantDirection) => _constantDirection = constantDirection; 
        public Vector2 ReadMoveDirection()
        {
            return _constantDirection;
        }
    }
}