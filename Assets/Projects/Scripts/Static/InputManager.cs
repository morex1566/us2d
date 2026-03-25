using Common;
using UnityEngine;

public class InputManager : MonoBehaviourSingleton<InputManager>
{
    private static InputMappingContext inputMappingContext;

    public static InputMappingContext InputMappingContext => inputMappingContext;

    public static void Init()
    {
        GetInstance();
        {
            inputMappingContext = new InputMappingContext();
            inputMappingContext.Enable();
        }
    }
}
