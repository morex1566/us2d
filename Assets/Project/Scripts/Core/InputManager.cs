using UnityEngine;

namespace TRPG.Runtime
{
    public struct InputSnapshot
    {
        // Delta
        public Vector2 move;
        public Vector2 look;

        // Trigger
        public bool attackPressed;
        public bool rollPressed;
        public bool reloadPressed;

        public bool IsEmpty => Equals(default(InputSnapshot));

        public InputSnapshot Consume()
        {
            InputSnapshot value = this;
            this = default;
            move = value.move;
            look = value.look;

            return value;
        }
    }

    public class InputManager : MonoBehaviourSingleton<InputManager>
    {
        public static InputMappingContext InputMappingContext;

        public static void Init()
        {
            GetInstance();
            {
                InputMappingContext = new InputMappingContext();
                InputMappingContext.Enable();
            }
        }
    }
}
