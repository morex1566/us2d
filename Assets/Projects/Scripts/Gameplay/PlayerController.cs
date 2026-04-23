using Net.Protocol;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using US2D.Network.Logic;



public struct PlayerInputSnapshot
{
    // Delta
    public Vector2 move;
    public Vector2 look;

    // Trigger
    public bool firePressed;
    public bool rollPressed;
    public bool reloadPressed;

    public bool IsEmpty => Equals(default(PlayerInputSnapshot));

    public PlayerInputSnapshot Consume()
    {
        PlayerInputSnapshot value = this;
        this = default;
        move = value.move;
        look = value.look;

        return value;
    }
}



[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerView))]
[DisallowMultipleComponent]
public partial class PlayerController : MonoBehaviour, InputMappingContext.IPlayerActions
{

    [SerializeField, ReadOnly] private Player player = null;

    private PlayerInputSnapshot inputSnapshot = new PlayerInputSnapshot();




    /// <summary>
    /// float1 : 현재 체력, float2: 최대 체력
    /// </summary>
    public Action<float, float> OnColliderTriggered;

    public event Action OnFireTriggered;

    public event Action OnReloadTriggered;

    public event Action OnMoveTriggered;

    public event Action OnRollTriggered;

    public event Action<UnityEngine.Transform, Vector2> OnLookTriggered;

    private void OnValidate()
    {
        Init();
    }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        InputManager.InputMappingContext.Player.Move.performed += OnMove;
        InputManager.InputMappingContext.Player.Move.canceled += OnMove;
        InputManager.InputMappingContext.Player.Look.performed += OnLook;
        InputManager.InputMappingContext.Player.Look.canceled += OnLook;
        InputManager.InputMappingContext.Player.Fire.performed += OnFire;
        InputManager.InputMappingContext.Player.Roll.performed += OnRoll;
        InputManager.InputMappingContext.Player.Reload.performed += OnReload;
    }

    private void OnDisable()
    {
        InputManager.InputMappingContext.Player.Move.performed -= OnMove;
        InputManager.InputMappingContext.Player.Move.canceled -= OnMove;
        InputManager.InputMappingContext.Player.Look.performed -= OnLook;
        InputManager.InputMappingContext.Player.Look.canceled -= OnLook;
        InputManager.InputMappingContext.Player.Fire.performed -= OnFire;
        InputManager.InputMappingContext.Player.Roll.performed -= OnRoll;
        InputManager.InputMappingContext.Player.Reload.performed -= OnReload;
    }

    private void Update()
    {
        player.StateMachine.Update(inputSnapshot.Consume());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // MoveDirection
        Gizmos.DrawLine(player.transform.position, player.transform.position + player.MoveDirection);
    }
}

/// <summary>
/// 입력, 트리거 처리
/// </summary>
public partial class PlayerController
{
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        inputSnapshot.move = input;

        OnMoveTriggered?.Invoke();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        inputSnapshot.look = input;

        OnLookTriggered?.Invoke(transform, player.LookDirection);
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        inputSnapshot.firePressed = true;

        OnFireTriggered?.Invoke();
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        inputSnapshot.rollPressed = true;

        OnRollTriggered?.Invoke();
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        inputSnapshot.reloadPressed = true;

        OnReloadTriggered?.Invoke();
    }

    public void OnInspect(InputAction.CallbackContext context) {}

    public void OnInteract(InputAction.CallbackContext context) {}

    public void OnPrevious(InputAction.CallbackContext context) {}

    public void OnNext(InputAction.CallbackContext context) {}

    public void OnTriggerEnter2D(Collider2D collision)
    {
        OnColliderTriggered?.Invoke(player.CurrentHp, player.Data.MaxHp);
    }
}

/// <summary>
/// 네트워크 메서드
/// </summary>
public partial class PlayerController
{
    [NetSendMessage(PacketTypeId.Transform)]
    public void OnSendMove(Net.Protocol.Transform payload) {}
}
