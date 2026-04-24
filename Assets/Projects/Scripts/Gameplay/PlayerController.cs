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
    public bool jumpPressed;

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

    [Header("Ground Check")]
    [SerializeField] private UnityEngine.Transform groundCheck = null;
    [SerializeField] private float groundCheckRadius = 0.08f;
    [SerializeField] private float groundCheckExtraDistance = 0.02f;
    [SerializeField] private LayerMask groundLayerMask = Physics2D.AllLayers;

    public bool IsGrounded { get; private set; } = true;
    public float GroundedY { get; private set; } = 0f;

    private InputAction jumpAction = null;
    private readonly Collider2D[] groundHits = new Collider2D[8];




    /// <summary>
    /// float1 : 현재 체력, float2: 최대 체력
    /// </summary>
    public Action<float, float> OnColliderTriggered;

    public event Action OnFireTriggered;

    public event Action OnReloadTriggered;

    public event Action OnMoveTriggered;

    public event Action OnRollTriggered;

    public event Action OnJumpTriggered;

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
        if (InputManager.InputMappingContext != null)
        {
            InputManager.InputMappingContext.Player.Move.performed += OnMove;
            InputManager.InputMappingContext.Player.Move.canceled += OnMove;
            InputManager.InputMappingContext.Player.Look.performed += OnLook;
            InputManager.InputMappingContext.Player.Look.canceled += OnLook;
            InputManager.InputMappingContext.Player.Fire.performed += OnFire;
            InputManager.InputMappingContext.Player.Roll.performed += OnRoll;
            InputManager.InputMappingContext.Player.Reload.performed += OnReload;

            jumpAction = InputManager.InputMappingContext.asset.FindAction("Player/Jump", false);
            if (jumpAction != null)
            {
                jumpAction.performed += OnJump;
            }
        }
    }

    private void OnDisable()
    {
        if (InputManager.InputMappingContext != null)
        {
            InputManager.InputMappingContext.Player.Move.performed -= OnMove;
            InputManager.InputMappingContext.Player.Move.canceled -= OnMove;
            InputManager.InputMappingContext.Player.Look.performed -= OnLook;
            InputManager.InputMappingContext.Player.Look.canceled -= OnLook;
            InputManager.InputMappingContext.Player.Fire.performed -= OnFire;
            InputManager.InputMappingContext.Player.Roll.performed -= OnRoll;
            InputManager.InputMappingContext.Player.Reload.performed -= OnReload;
        }

        if (jumpAction != null)
        {
            jumpAction.performed -= OnJump;
            jumpAction = null;
        }
    }

    private void Update()
    {
        UpdateGrounded();
        player.StateMachine.Update(inputSnapshot.Consume());
    }

    private void UpdateGrounded()
    {
        Vector2 checkPos = GetGroundCheckPosition();

        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = false;
        filter.useLayerMask = true;
        filter.layerMask = GetGroundCheckLayerMask();

        int count = Physics2D.OverlapCircle(checkPos, groundCheckRadius, filter, groundHits);

        IsGrounded = count > 0;
        if (IsGrounded)
        {
            GroundedY = transform.position.y;
        }
    }

    private Vector2 GetGroundCheckPosition()
    {
        if (groundCheck != null)
        {
            return groundCheck.position;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            return new Vector2(col.bounds.center.x, col.bounds.min.y - groundCheckExtraDistance);
        }

        return (Vector2)transform.position + Vector2.down * 0.25f;
    }

    private LayerMask GetGroundCheckLayerMask()
    {
        int mask = groundLayerMask.value;
        mask &= ~(1 << gameObject.layer);
        return mask;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (player != null)
        {
            // MoveDirection
            Gizmos.DrawLine(player.transform.position, player.transform.position + player.MoveDirection);
        }

        Vector3 groundPos = GetGroundCheckPosition();
        Gizmos.color = IsGrounded ? Color.cyan : Color.red;
        Gizmos.DrawWireSphere(groundPos, groundCheckRadius);
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

    public void OnJump(InputAction.CallbackContext context)
    {
        inputSnapshot.jumpPressed = true;

        OnJumpTriggered?.Invoke();
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
