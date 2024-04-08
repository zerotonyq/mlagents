using Movement.Fsm.View;
using Movement.Input.Base;
using UnityEngine;

namespace Character
{
    public class CharacterFactory
    {
        public static GameObject CreateCharacter(GameObject prefab, IMovementInputManager movementInputManager = null)
        {
            var character = GameObject.Instantiate(prefab, null);
            if (movementInputManager != null)
                character.GetComponent<FsmMovementView>().Initialize(movementInputManager);
            return character;
        }
    }
}