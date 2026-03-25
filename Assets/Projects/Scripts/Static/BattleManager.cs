using Common;
using Static.Pathfinding;
using UnityEngine;
using UnityEngine.InputSystem;

public class BattleManager : MonoBehaviourSingleton<BattleManager>
{
    private void OnEnable()
    {
        InputManager.InputMappingContext.Player.Enable();

        InputManager.InputMappingContext.Player.Move.started += OnMove;
        InputManager.InputMappingContext.Player.Inspect.started += OnFocus;
        InputManager.InputMappingContext.Player.Inspect.started += OnGetObjectInfo;
    }

    private void OnDisable()
    {
        if (InputManager.InputMappingContext != null)
        {
            InputManager.InputMappingContext.Player.Move.started -= OnMove;
            InputManager.InputMappingContext.Player.Inspect.started -= OnFocus;
            InputManager.InputMappingContext.Player.Inspect.started -= OnGetObjectInfo;

            InputManager.InputMappingContext.Player.Disable();
        }
    }

    // context.ReadValue 지점으로 이동
    public void OnMove(InputAction.CallbackContext context)
    {
        
    }

    // context.ReadValue 지점을 확대
    public void OnFocus(InputAction.CallbackContext context)
    {

    }

    public void OnGetObjectInfo(InputAction.CallbackContext context)
    {

    }
}
