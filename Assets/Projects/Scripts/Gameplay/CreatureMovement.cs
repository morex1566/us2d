using System;
using UnityEngine;

[Serializable]
public class CreatureMovement
{
    private Creature owner = null;

    private Vector2 currMoveDirection = Vector2.zero;

    private Vector2 prevMoveDirection = Vector2.right;



    public Creature Owner => owner;

    public Vector2 CurrentMoveDirection => currMoveDirection;

    public Vector2 PreviousMoveDirection => prevMoveDirection;



    public void Init(Creature owner)
    {
        this.owner = owner;
    }

    public void SetMoveDirection(Vector2 moveDirection)
    {
        Vector2 normalizedDirection = moveDirection.IsNearlyZero() ? Vector2.zero : moveDirection.normalized;

        if (normalizedDirection.IsNotNearlyZero())
        {
            prevMoveDirection = currMoveDirection;
        }

        currMoveDirection = normalizedDirection;
    }

    public void Move(Transform transform)
    {
        Vector2 frameVelocity = new Vector2
        (
            currMoveDirection.x * owner.Data.MaxSpeed.x,
            currMoveDirection.y * owner.Data.MaxSpeed.y
        );

        transform.position += (Vector3)frameVelocity * Time.deltaTime;
    }
}
