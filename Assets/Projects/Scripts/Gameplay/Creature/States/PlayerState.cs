using UnityEngine;

public enum PlayerStateType
{
    NONE = 0,
    IDLE = 1,
    MOVE = 2,
    ROLL = 3,
    DEAD = 4,
    JUMP = 5
}


public abstract class PlayerState
{
    protected Player Player { get; private set; }

    public PlayerStateType StateType { get; private set; } = PlayerStateType.NONE;

    protected PlayerState(Player player, PlayerStateType stateType)
    {
        Player = player;
        StateType = stateType;
    }

    public virtual void Enter() {}

    public virtual void Exit() {}

    // 입력을 처리해서 실제로 Player 데이터에 반영
    public virtual void Update(PlayerInputSnapshot inputSnapshot) {}

    public virtual bool CanTransitTo(PlayerStateType nextStateType)
    {
        return true;
    }

    // Player 데이터나 입력을 통해 다른 상태로 넘어가야하는지 검사
    public virtual void Evaluate(PlayerInputSnapshot inputSnapshot) { }
}
