
using Net.Protocol;
using UnityEngine;

public abstract class PlayerState : IState<PlayerStateType>
{
    protected PlayerController Controller { get; private set; }

    public PlayerStateType StateType { get; private set; } = PlayerStateType.NONE;

    protected PlayerState(PlayerController controller, PlayerStateType stateType)
    {
        Controller = controller;
        StateType = stateType;
    }

    public virtual void Enter() {}

    public virtual void Exit() {}

    public virtual void Update(InputSnapshot inputSnapshot) {}

    public virtual PlayerStateType? Evaluate(InputSnapshot inputSnapshot)
    {
        return null;
    }

    public virtual bool CanTransitTo(PlayerStateType nextStateType)
    {
        return true;
    }

    protected virtual void SetMoveDirection(InputSnapshot inputSnapshot)
    {
        if (inputSnapshot.move.IsNearlyZero())
        {
            return; // 이동 입력이 없으면 현재 이동 방향 유지
        }

        Vector3 input = inputSnapshot.move.normalized;
        Controller.CurrMoveDirection = new Vector3(input.x, 0f, input.y);
    }

    protected virtual void SetLookDirection(InputSnapshot inputSnapshot)
    {
        Vector3 lookDirection = Utls.GetMouseWorldPosition() - Controller.transform.position;
        lookDirection.z = 0f;
        Controller.CurrLookDirection = lookDirection.normalized;
    }

    protected virtual void Move(InputSnapshot inputSnapshot)
    {
        if (inputSnapshot.move.IsNearlyZero())
        {
            return; // 이동 입력이 없으면 위치 이동 생략
        }

        Vector3 frameVelocity = new Vector3(Controller.CurrMoveDirection.x * Controller.Data.MaxSpeed.x, 0f, Controller.CurrMoveDirection.z * Controller.Data.MaxSpeed.z);

        Controller.transform.position += frameVelocity * Time.deltaTime;
    }

    protected virtual void Rotate() {}
}
