using System;
using UnityEngine;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
[DisallowMultipleComponent]
public class PlayerView : MonoBehaviour
{
    [SerializeField, ReadOnly] private Player player = null;

    [SerializeField] private Animator animator = null;

    [SerializeField] private SpriteRenderer spriter = null;

    [SerializeField] private float lookDeadZone = 0.1f;


    private void OnValidate()
    {
        Init();
    }

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        player = GetComponent<Player>();
        animator = Utls.FindComponent<Animator>(gameObject);
        spriter = Utls.FindComponent<SpriteRenderer>(gameObject);
    }

    private void Update()
    {
        UpdateFlip();
        UpdateAnimationParameters();
    }

    public void UpdateFlip()
    {
        // 마우스가 위쪽에 있을 때 캐릭터가 좌우로 계속 덜덜 떨림 방지
        if (Mathf.Abs(player.LookDirection.x) < lookDeadZone)
        {
            return;
        }

        spriter.flipX = player.LookDirection.x < 0;
    }

    public void UpdateAnimationParameters()
    {
        animator.SetBool
        (
            UnityConstant.Animator.Parameters.AC_Player.Bool.IsIdle,
            player.StateMachine.CurrentState.StateType == PlayerStateType.IDLE
        );

        animator.SetBool
        (
            UnityConstant.Animator.Parameters.AC_Player.Bool.IsMoving,
            player.StateMachine.CurrentState.StateType == PlayerStateType.MOVE
        );

        animator.SetBool
        (
            UnityConstant.Animator.Parameters.AC_Player.Bool.IsRoll,
            player.StateMachine.CurrentState.StateType == PlayerStateType.ROLL
        );

        animator.SetBool
        (
            "IsJump",
            player.StateMachine.CurrentState.StateType == PlayerStateType.JUMP
        );
    }
}
