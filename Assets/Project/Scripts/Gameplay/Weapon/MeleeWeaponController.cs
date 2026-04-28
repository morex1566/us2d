using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public partial class MeleeWeaponController : WeaponController
{
    [Header("External")]
    [SerializeField, ReadOnly] private PlayerController playerController = null;

    [SerializeField, ReadOnly] private PlayerView playerView = null;

    [SerializeField, ReadOnly] private MeleeWeaponView meleeWeaponView = null;

    [SerializeField] private Transform pivotTransform = null;

    [SerializeField] private Transform socketTransform = null;



    public MeleeWeaponStateType CurrState { get; private set; } = MeleeWeaponStateType.LeftReady;

    [Header("Swing")]
    [SerializeField, Min(0f)] private float swingDuration = 0.16f;

    // x는 정규화된 진행도, y는 목표 회전까지의 진행 비율
    [SerializeField] private AnimationCurve swingAnimationCurve = new AnimationCurve
    (
        new Keyframe(0f, 0f, 0f, 0f),
        new Keyframe(0.35f, 0.12f),
        new Keyframe(0.65f, 0.88f),
        new Keyframe(1f, 1f, 0f, 0f)
    );

    [SerializeField] private float socketTransformSwingAngle = 420f;

    [SerializeField] private float pivotTransformSwingAngle = 170f;

    // 현재 재생 중인 스윙 코루틴
    private Coroutine swingCoroutine = null;

    // 이번 스윙에서 최종적으로 더할 socket 회전량
    private float currentSocketSwingAngle = 0f;

    // 이번 스윙에서 최종적으로 더할 pivot 회전량
    private float currentPivotSwingAngle = 0f;



    protected override void OnValidate()
    {
        Init();
    }

    protected override void Awake()
    {
        Init();
    }

    protected override void Init()
    {
        meleeWeaponView ??= GetComponent<MeleeWeaponView>();

        // 무기가 플레이어에게 부착되었는지?
        if (Application.isPlaying && transform.IsChildOf(GameObject.FindGameObjectWithTag(UnityConstant.Tags.Player).transform))
        {
            playerController = Utls.FindComponentByTag<PlayerController>(UnityConstant.Tags.Player);
            playerView = Utls.FindComponentByTag<PlayerView>(UnityConstant.Tags.Player);

            pivotTransform = playerController.SpringArm.PivotTransform;
            socketTransform = playerController.SpringArm.SocketTransform;
        }
    }

    private void OnEnable()
    {
        playerController.OnAttackTriggered += OnAttack;
    }

    private void OnDisable()
    {
        playerController.OnAttackTriggered -= OnAttack;
    }

    private void OnDrawGizmos()
    {
        if (playerController)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(playerController.SpringArm.PivotTransform.position, transform.position);
        }
    }
}


public partial class MeleeWeaponController
{
    public void OnAttack(PlayerController controller, InputSnapshot input)
    {
        // 스윙 시작 시점의 ready 상태를 코루틴에 넘겨 종료 상태를 결정
        StartSwing(playerView.Spriter.flipX, CurrState);
    }

    private void StartSwing(bool playerStartFlipX, MeleeWeaponStateType startState)
    {
        swingCoroutine = StartCoroutine(CoSwing(playerStartFlipX, startState));
        CurrState = MeleeWeaponStateType.Swing;
    }

    private IEnumerator CoSwing(bool playerStartFlipX, MeleeWeaponStateType startState)
    {
        float prevElapsedTime = 0f;
        float currElapsedTime = 0f;
        float duration = Mathf.Max(swingDuration, 0.01f);
        float prevAnimCurve = swingAnimationCurve.Evaluate(0f);
        float currAnimCurve = swingAnimationCurve.Evaluate(duration);

        // 무기 공격 시작
        CurrState = MeleeWeaponStateType.Swing;

        // 무기 공격중
        while (currElapsedTime < duration)
        {
            currElapsedTime = Mathf.Min(prevElapsedTime + Time.deltaTime, duration);
            currAnimCurve = swingAnimationCurve.Evaluate(Mathf.Clamp01(currElapsedTime / duration));

            // 누적 회전이므로 이전 프레임 대비 변화량만 더함
            float deltaAnimCurve = currAnimCurve - prevAnimCurve;

            ApplySwingRotation(playerStartFlipX, startState, deltaAnimCurve);

            prevElapsedTime = currElapsedTime;
            prevAnimCurve = currAnimCurve;

            yield return new WaitForEndOfFrame();
        }

        // 무기 공격 끝
        swingCoroutine = null;
        CurrState = startState == MeleeWeaponStateType.LeftReady ? MeleeWeaponStateType.RightReady : MeleeWeaponStateType.LeftReady;
    }

    private void ApplySwingRotation(bool playerStartFlipX, MeleeWeaponStateType startState, float deltaAnimCurve)
    {
        float pivotDeltaAngle = pivotTransformSwingAngle * deltaAnimCurve;
        float socketDeltaAngle = socketTransformSwingAngle * deltaAnimCurve;

        // 내려치기 상태
        if (startState == MeleeWeaponStateType.LeftReady)
        {
            pivotTransform.Rotate(0f, 0f, pivotDeltaAngle * -1);
            socketTransform.Rotate(0f, 0f, socketDeltaAngle * -1);

            return;
        }

        // 올려치기 상태
        if (startState == MeleeWeaponStateType.RightReady)
        {
            pivotTransform.Rotate(0f, 0f, pivotDeltaAngle);
            socketTransform.Rotate(0f, 0f, socketDeltaAngle);

            return;
        }
    }
}
