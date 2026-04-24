using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player player) : base(player, PlayerStateType.IDLE) {}

    public override void Update(PlayerInputSnapshot inputSnapshot)
    {
        SetMoveDirection(inputSnapshot);
        SetLookDirection(inputSnapshot);
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

    public override void Evaluate(PlayerInputSnapshot inputSnapshot)
    {
        if (Player.CurrentHp <= 0f)
        {
            Player.StateMachine.ChangeState(PlayerStateType.DEAD);
            return;
        }

        if (inputSnapshot.rollPressed)
        {
            Player.StateMachine.ChangeState(PlayerStateType.ROLL);
            return;
        }

        PlayerController controller = Utls.FindComponent<PlayerController>(Player.gameObject);
        if (inputSnapshot.jumpPressed && controller != null && controller.IsGrounded)
        {
            Player.StateMachine.ChangeState(PlayerStateType.JUMP);
            return;
        }

        if (inputSnapshot.move.IsNotNearlyZero())
        {
            Player.StateMachine.ChangeState(PlayerStateType.MOVE);
            return;
        }
    }
}
