using UnityEngine;

public class PlayerJumpState : PlayerState
{
    private PlayerController controller;

    private float verticalVelocity;
    private float jumpStartY;

    public PlayerJumpState(Player player) : base(player, PlayerStateType.JUMP)
    {
        controller = Utls.FindComponent<PlayerController>(player.gameObject);
    }

    public override void Enter()
    {
        if (controller != null)
        {
            jumpStartY = controller.GroundedY;
        }
        else
        {
            jumpStartY = Player.transform.position.y;
        }

        verticalVelocity = Player.Data.JumpVelocity;
    }

    public override void Update(PlayerInputSnapshot inputSnapshot)
    {
        SetMoveDirection(inputSnapshot);
        SetLookDirection(inputSnapshot);

        MoveHorizontal();
        ApplyGravityAndMoveVertical();
    }

    private void SetMoveDirection(PlayerInputSnapshot inputSnapshot)
    {
        if (inputSnapshot.move.IsNearlyZero()) return;

        Vector3 input = inputSnapshot.move.normalized;
        Player.MoveDirection = new Vector3(input.x, 0, input.y);
    }

    private void SetLookDirection(PlayerInputSnapshot inputSnapshot)
    {
        if (inputSnapshot.look.IsNearlyZero()) return;

        Player.LookDirection = (Utls.GetMouseWorldPosition() - Player.transform.position).normalized;
    }

    private void MoveHorizontal()
    {
        Vector3 frameVelocity = new Vector3
        (
            Player.MoveDirection.x * Player.Data.MaxSpeed.x,
            0,
            Player.MoveDirection.z * Player.Data.MaxSpeed.z
        );

        float airControl = Player.Data.AirControlMultiplier;
        Player.transform.position += frameVelocity * airControl * Time.deltaTime;
    }

    private void ApplyGravityAndMoveVertical()
    {
        verticalVelocity += Player.Data.JumpGravity * Time.deltaTime;

        Vector3 pos = Player.transform.position;
        pos.y += verticalVelocity * Time.deltaTime;
        Player.transform.position = pos;
    }

    public override void Evaluate(PlayerInputSnapshot inputSnapshot)
    {
        if (Player.CurrentHp <= 0f)
        {
            Player.StateMachine.ChangeState(PlayerStateType.DEAD);
            return;
        }

        bool isGrounded = controller != null && controller.IsGrounded;
        if (isGrounded && verticalVelocity <= 0f)
        {
            Vector3 pos = Player.transform.position;
            pos.y = jumpStartY;
            Player.transform.position = pos;

            Player.StateMachine.ChangeState(inputSnapshot.move.IsNearlyZero() ? PlayerStateType.IDLE : PlayerStateType.MOVE);
            return;
        }
    }

    public override bool CanTransitTo(PlayerStateType nextStateType)
    {
        if (nextStateType == PlayerStateType.ROLL) return false;
        return true;
    }
}
