using Net.Protocol;
using UnityEngine;
using UnityEngine.InputSystem;

public static class PlayerAnimationStatus
{
    public static readonly int IsMoving = Animator.StringToHash("IsMoving");
}

public partial class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Animator _animator = null;

    [SerializeField]
    private PlayerData _data = null;

    private bool _isMoving = false;

    private Vector2 _inputDirection = Vector2.zero;

    private Vector2 _prevInputDirection = Vector2.zero;

    private Vector2 _currVelocity = Vector2.zero;

    private void Update()
    {
        UpdateMovement();
        UpdateAnimation();
    }

    /// <summary>
    /// Vector2 단위를 사용한 가속도 기반 이동 로직
    /// </summary>
    public void UpdateMovement()
    {
        // 목표 속도 계산 (Vector2)
        Vector2 targetSpeed = new Vector2
        (
            _inputDirection.x * _data.MaxSpeed.x,
            _inputDirection.y * _data.MaxSpeed.y
        );

        // 현재 적용할 가속/감속 수치 결정 (Vector2)
        Vector2 accel = new Vector2
        (
            (_inputDirection.x != 0) ? _data.MaxAccel.x : _data.MaxDecel.x,
            (_inputDirection.y != 0) ? _data.MaxAccel.y : _data.MaxDecel.y
        );

        // 가속도
        _currVelocity.x = Mathf.MoveTowards(_currVelocity.x, targetSpeed.x, accel.x * Time.deltaTime);
        _currVelocity.y = Mathf.MoveTowards(_currVelocity.y, targetSpeed.y, accel.y * Time.deltaTime);

        // 이동 상태 판정
        _isMoving = _currVelocity.sqrMagnitude > 0.001f;

        // 최종 위치 업데이트 + 서버 동기화
        transform.position += (Vector3)_currVelocity * Time.deltaTime;
        // Move(Net.Protocol.Transform.CreateTransform());
    }

    [NetSendMessage(PacketTypeId.C2S_Transform)]
    public void Move(Net.Protocol.Transform c2sPayload)
    {

    }

    private void OnMoveInput(InputValue value)
    {
        _inputDirection = value.Get<Vector2>();

        if (_inputDirection.sqrMagnitude > 0)
        {
            _prevInputDirection = _inputDirection;
        }
    }

    public void UpdateAnimation()
    {
        _animator.SetBool(PlayerAnimationStatus.IsMoving, _isMoving);
    }
}