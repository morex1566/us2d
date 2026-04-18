using UnityEngine;
using UnityEngine.InputSystem;

public class WorldImageGyroscoper : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float sensitivity = 0.01f; // 월드 단위이므로 낮게 설정 권장
    [SerializeField] private float smoothMoveTime = 0.2f;
    [SerializeField] private Vector2 moveLimit = new Vector2(2f, 2f); // 3D 미터 단위

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 currentMoveVelocity;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        UpdateMovement();
    }

    private void Init()
    {
        // 월드 공간에서의 시작 위치 저장
        startPosition = transform.localPosition;
        targetPosition = transform.localPosition;
    }

    private void UpdateMovement()
    {
        if (Mouse.current == null) return;

        // 1. 입력 처리 (Mouse Delta)
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // 2. 타겟 위치 계산 (Z축은 유지하고 X, Y만 조작)
        targetPosition.x += mouseDelta.x * sensitivity;
        targetPosition.y += mouseDelta.y * sensitivity;

        // 3. 이동 범위 제한 (Clamping)
        targetPosition.x = Mathf.Clamp(targetPosition.x, startPosition.x - moveLimit.x, startPosition.x + moveLimit.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, startPosition.y - moveLimit.y, startPosition.y + moveLimit.y);

        // 4. 부드러운 이동 (SmoothDamp)
        transform.localPosition = Vector3.SmoothDamp(
            transform.localPosition,
            targetPosition,
            ref currentMoveVelocity,
            smoothMoveTime
        );
    }

    // 에디터에서 범위를 시각적으로 확인하기 위함
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = Application.isPlaying ? startPosition : transform.localPosition;
        Gizmos.DrawWireCube(center, new Vector3(moveLimit.x * 2, moveLimit.y * 2, 0.1f));
    }
}