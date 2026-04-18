using Net.Protocol;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using US2D.Network.Logic;

[RequireComponent(typeof(Player))]
public partial class PlayerController : MonoBehaviour, InputMappingContext.IPlayerActions
{
    [Header("Player")]
    [SerializeField] private Player player = null;

    private CreatureView view = new CreatureView();

    private CreatureMovement movement = new CreatureMovement();



    /// <summary>
    /// float1 : 현재 체력, float2: 최대 체력
    /// </summary>
    public Action<float, float> OnColliderTriggered;

    public event Action OnFireTriggered;

    public event Action OnMoveTriggered;



    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        view.Init(player);
        movement.Init(player);
    }

    private void OnEnable()
    {
        InputManager.InputMappingContext.Player.Move.performed += OnMove;
        InputManager.InputMappingContext.Player.Move.canceled += OnMove;
        InputManager.InputMappingContext.Player.Attack.performed += OnFire;
        InputManager.InputMappingContext.Player.Look.performed += OnLook;
    }

    private void OnDisable()
    {
        InputManager.InputMappingContext.Player.Move.performed -= OnMove;
        InputManager.InputMappingContext.Player.Move.canceled -= OnMove;
        InputManager.InputMappingContext.Player.Attack.performed -= OnFire;
        InputManager.InputMappingContext.Player.Look.performed -= OnLook;
    }

    private void Update()
    {
        movement.Move(transform);
    }

    private void LateUpdate()
    {
        view.SetAnimationParameters(player.IsMoving, player.IsGroggy, player.IsRolling);
    }

    public void OnMove(InputAction.CallbackContext value)
    {
        movement.SetMoveDirection(value.ReadValue<Vector2>());

        OnMoveTriggered?.Invoke();
    }

    public void OnFire(InputAction.CallbackContext value)
    {
        OnFireTriggered?.Invoke();
    }

    public void OnLook(InputAction.CallbackContext context) 
    {
        view.UpdateLookDirection(transform, context.ReadValue<Vector2>());
        view.UpdateFlip();
    }


    public void OnInspect(InputAction.CallbackContext context) { }
    public void OnAttack(InputAction.CallbackContext context) { }
    public void OnInteract(InputAction.CallbackContext context) { }
    public void OnCrouch(InputAction.CallbackContext context) { }
    public void OnJump(InputAction.CallbackContext context) { }
    public void OnPrevious(InputAction.CallbackContext context) { }
    public void OnNext(InputAction.CallbackContext context) { }
    public void OnSprint(InputAction.CallbackContext context) { }

    public void OnTriggerEnter2D(Collider2D collision)
    {

    }
}

/// <summary>
/// 네트워크 메서드
/// </summary>
public partial class PlayerController
{
    [NetSendMessage(PacketTypeId.Transform)]
    public void OnSendMove(Net.Protocol.Transform payload) { }
}