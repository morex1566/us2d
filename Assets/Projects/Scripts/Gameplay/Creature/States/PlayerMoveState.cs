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

        Player.MoveDirection = inputSnapshot.move.normalized;
    }

    private void Move()
    {
        Vector2 frameVelocity = new Vector2
        (
            Player.MoveDirection.x * Player.Data.MaxSpeed.x,
            Player.MoveDirection.y * Player.Data.MaxSpeed.y
        );

        Player.transform.position += (Vector3)frameVelocity * Time.deltaTime;
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

        if (inputSnapshot.move.IsNearlyZero())
        {
            Player.StateMachine.ChangeState(PlayerStateType.IDLE);
            return;
        }
    }
}
