using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("추적 대상")]
    public Transform target;

    [Header("설정")]
    [Range(0, 10)] public float lerpThreshold = 0.1f;



    void Update()
    {
        if (target == null) return;

        Vector2 destination = Vector2.Lerp(transform.position, target.position, lerpThreshold * Time.deltaTime);

        transform.position = new Vector3(destination.x, destination.y, -10);
    }
}