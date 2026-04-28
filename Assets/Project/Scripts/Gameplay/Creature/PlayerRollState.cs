using UnityEngine;

public class PlayerRollState : PlayerState
{
    private readonly Animator animator;

    private float moveBlockTime = 0.275f;

    public PlayerRollState(PlayerController controller) : base(controller, PlayerStateType.ROLL)
    {
        animator = Utls.FindComponent<Animator>(controller.gameObject);
    }

    public override void Update(InputSnapshot inputSnapshot)
    {
        SetMoveDirection(inputSnapshot);
        SetLookDirection(inputSnapshot);
        Move(inputSnapshot);
    }

    protected override void Move(InputSnapshot inputSnapshot)
    {
        if (inputSnapshot.move.IsNearlyZero())
        {
            return; // 구르기 중 입력이 없으면 이동 보정 생략
        }

        if (animator == null)
        {
            return; // Animator가 없으면 구르기 진행률을 알 수 없음
        }

        Vector3 frameVelocity = new Vector3(Controller.CurrMoveDirection.x * Controller.Data.MaxSpeed.x, 0f, Controller.CurrMoveDirection.z * Controller.Data.MaxSpeed.z);

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsName(UnityConstant.Animator.Parameters.AC_Player.Bool.IsRoll) == false)
        {
            return; // 아직 Roll 애니메이션 상태가 아니면 이동 보정 생략
        }

        if (info.normalizedTime <= moveBlockTime)
        {
            Controller.transform.position += frameVelocity * Controller.Data.SpeedAccelAtRollMultiplier * Time.deltaTime;
            return;
        }

        Controller.transform.position += frameVelocity * Controller.Data.SpeedDecelAtRollMultiplier * Time.deltaTime;
    }

    protected override void SetMoveDirection(InputSnapshot inputSnapshot)
    {
        if (inputSnapshot.move.IsNearlyZero())
        {
            return; // 입력이 없으면 구르기 방향 유지
        }

        if (animator == null)
        {
            return; // Animator가 없으면 구르기 방향 잠금 구간을 판단할 수 없음
        }

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        bool isDirectionLocked = info.IsName(UnityConstant.Animator.Parameters.AC_Player.Bool.IsRoll) && info.normalizedTime <= moveBlockTime;
        if (isDirectionLocked)
        {
            return; // 구르기 초반에는 방향 변경을 막아 애니메이션 관성을 유지
        }

        Vector3 input = inputSnapshot.move.normalized;
        Vector3 nextDirection = new Vector3(input.x, 0f, input.y);
        Controller.CurrMoveDirection = Vector3.Lerp(Controller.CurrMoveDirection, nextDirection, Time.deltaTime * 10f);
    }

    public override PlayerStateType? Evaluate(InputSnapshot inputSnapshot)
    {
        if (animator == null)
        {
            return PlayerStateType.IDLE; // Animator가 없으면 구르기를 지속할 기준이 없으므로 안전하게 대기 상태로 복귀
        }

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        bool isRollFinished = info.IsName(UnityConstant.Animator.Parameters.AC_Player.Bool.IsRoll) && info.normalizedTime >= 0.9f;

        if (isRollFinished == false)
        {
            return null; // Roll 애니메이션이 끝나기 전에는 현재 상태 유지
        }

        return inputSnapshot.move.IsNearlyZero() ? PlayerStateType.IDLE : PlayerStateType.MOVE;
    }
}
