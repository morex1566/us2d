using UnityEngine;
using UnityEngine.InputSystem;

public static class PlayerAnimationStatus
{
    public static readonly int IsMoving = Animator.StringToHash("IsMoving");
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator animator = null;

    [SerializeField] private SpriteRenderer spriter = null;

    [SerializeField] private PlayerData data = null;

    private bool isMoving = false;

    private float currentSpeed = 0;

    Vector2 inputDirection = Vector2.zero;

    Vector2 prevInputDirection = Vector2.zero;



    private void Awake()
    {
        currentSpeed = data.speed;
    }

    private void Start()
    {

    }

    private void Update()
    {
        UpdateMovement();
        UpdateAnimation();
    }

    public void OnMove(InputValue value)
    {
        inputDirection = value.Get<Vector2>();
        isMoving = inputDirection.magnitude > 0;

        if (isMoving)
        {
            prevInputDirection = inputDirection;
        }
    }

    public void UpdateMovement()
    {
        if (!isMoving) { return; }

        Vector3 movement = new Vector3(inputDirection.x, inputDirection.y, 0);
        transform.position += movement * currentSpeed * Time.deltaTime;
    }

    public void UpdateAnimation()
    {
        SetFlip();

        animator.SetBool(PlayerAnimationStatus.IsMoving, isMoving);
    }

    private void SetFlip()
    {
        if (prevInputDirection == Vector2.left)
        {
            spriter.flipX = true;
        }

        if (prevInputDirection == Vector2.right)
        {
            spriter.flipX = false;
        }
    }
}
