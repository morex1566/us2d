using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerView))]
[DisallowMultipleComponent]
public class Player : Creature
{
    public PlayerStateMachine StateMachine { get; private set; } = null;

    public Vector3 MoveDirection { get; set; } = Vector3.right;

    public Vector2 LookDirection { get; set; } = Vector2.right;



    public new PlayerData Data => base.Data as PlayerData;



    protected override void OnValidate()
    {
        base.OnValidate();

        Init();
    }

    protected override void Awake()
    {
        base.Awake();

        Init();
    }

    protected override void Init()
    {
        base.Init();

        StateMachine = new(this);
    }
}
