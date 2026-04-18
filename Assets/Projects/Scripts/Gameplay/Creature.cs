using System;
using UnityEngine;

public abstract class Creature : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] protected CreatureData data = null;

    [SerializeField] protected Animator animator = null;

    [SerializeField] protected SpriteRenderer spriter = null;




    protected float currentHp = 0f;

    protected float currentSp = 0f;

    protected bool isDead = false;

    protected bool isMoving = false;

    protected bool isLeft = false;



    public event Action<float, float> OnHpChanged;

    public CreatureData Data => data;

    public Animator Animator => animator;

    public SpriteRenderer Spriter => spriter;

    public float CurrentHp => currentHp;

    public float CurrentSp => currentSp;

    public bool IsDead => isDead;

    public bool IsMoving => isMoving;

    public bool IsLeft => isLeft;


    protected virtual void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        currentHp = data.MaxHp;
        currentSp = data.MaxSp;
    }
}
