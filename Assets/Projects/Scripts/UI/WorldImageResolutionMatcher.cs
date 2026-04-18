using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class WorldImageResolutionMatcher : MonoBehaviour
{
    private Vector3 initialScale;

    private Sprite sprite;

    private void Awake()
    {
        initialScale = transform.localScale;
        sprite = GetComponent<SpriteRenderer>().sprite;
    }

    private void OnEnable()
    {
        UIManager.GetInstance().OnResolutionChanged.AddListener(UpdateScale);

    }

    private void OnDisable()
    {
        UIManager.GetInstance().OnResolutionChanged.RemoveListener(UpdateScale);
    }

    private void Start()
    {
        // 초기 해상도에 맞춰 1회 실행
        UpdateScale(Screen.width, Screen.height);

        Debug.Log(sprite.texture.texelSize);
    }

    private void UpdateScale(float width, float height)
    {
        Vector2 referenceResolution = UIManager.GetInstance().GetReferenceResolution();
        float scaleFactor = height / referenceResolution.y;

        transform.localScale = initialScale * scaleFactor;
    }
}