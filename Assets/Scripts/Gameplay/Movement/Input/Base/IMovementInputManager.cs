using UnityEngine;

namespace Movement.Input.Base
{
    public interface IMovementInputManager
    {
        Vector2 ReadMoveDirection();
    }
}