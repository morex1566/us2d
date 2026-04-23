using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("External Dependencies")]
    public Transform target;

    [Header("Setup")]
    [Range(0, 20)] public float lerpThreshold = 0.1f;



    private void OnValidate()
    {
        UpdateMovement();
    }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        target = GameObject.FindGameObjectWithTag(UnityConstant.Tags.Player).transform;
    }

    private void Update()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        Vector2 destination = Vector2.Lerp(transform.position, target.position, lerpThreshold * Time.deltaTime);
        transform.position = new Vector3(destination.x, destination.y, -10);
    }
}