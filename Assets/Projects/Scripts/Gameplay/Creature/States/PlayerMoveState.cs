using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(Player player) : base(player, PlayerStateType.MOVE) {}

    public override void Update(PlayerInputSnapshot inputSnapshot)
    {
        SetMoveDirection(inputSnapshot);
        SetLookDirection(inputSnapshot);

        Move();
    }

    private void SetMoveDirection(PlayerInputSnapshot inputSnapshot)
    {
        if (inputSnapshot.move.IsNearlyZero()) return;

        Vector3 input = inputSnapshot.move.normalized;

        Player.MoveDirection = new Vector3(input.x, 0, input.y);
    }

    private void Move()
    {
        Vector3 frameVelocity = new Vector3
        (
            Player.MoveDirection.x * Player.Data.MaxSpeed.x,
            0,
            Player.MoveDirection.z * Player.Data.MaxSpeed.z
        );

        Player.transform.position += frameVelocity * Time.deltaTime;
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

        if (inputSnapshot.move.IsNearlyZero())
        {
            Player.StateMachine.ChangeState(PlayerStateType.IDLE);
            return;
        }
    }
}
