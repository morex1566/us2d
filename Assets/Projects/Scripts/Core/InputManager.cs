using UnityEngine;
using US2D.Network;

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
