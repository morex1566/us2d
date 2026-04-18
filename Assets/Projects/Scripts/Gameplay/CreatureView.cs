using System;
using UnityEngine;

[Serializable]
public class CreatureView
{
    private Creature owner = null;

    private float lookDeadZone = 0.1f;

    private Vector2 currLookDirection = Vector2.right;

    private Vector2 prevLookDirection = Vector2.right;



    public Creature Owner => owner;

    public Vector2 CurrLookDirection => currLookDirection;

    public Vector2 PreviousLookDirection => prevLookDirection;




    public void Init(Creature owner)
    {
        this.owner = owner;
    }

    public void UpdateLookDirection(Transform creatureTransform, Vector2 input)
    {
        if (input.IsNearlyZero())
        {
            return;
        }

        prevLookDirection = currLookDirection;
        currLookDirection = (Utls.GetMouseWorldPosition() - creatureTransform.position).normalized;
    }

    // 마우스 입력 시 콜, 크로스헤어가 플레이어 기준 좌/우 위치에 따라 플립
    public void UpdateFlip()
    {
        owner.Spriter.flipX = currLookDirection.x < lookDeadZone * -1f ? true : currLookDirection.x > lookDeadZone ? false : owner.Spriter.flipX;
    }

    // 매 업데이트 마다
    public void SetAnimationParameters(bool isMoving, bool isGroggy, bool isRoll)
    {
        owner.Animator.SetBool(UnityConstant.Animator.Parameters.AC_Player.Bool.IsMoving, isMoving);
        owner.Animator.SetBool(UnityConstant.Animator.Parameters.AC_Player.Bool.IsGroggy, isGroggy);
        owner.Animator.SetBool(UnityConstant.Animator.Parameters.AC_Player.Bool.IsRoll, isRoll);
    }
}
