using UnityEngine;
using TMPro;

public class TextPixelizer : MonoBehaviour
{
    [SerializeField, Range(1, 512)] private float pixelScale = 100f;

    public float PixelScale
    {
        get => pixelScale;
        set
        {
            pixelScale = Mathf.Clamp(value, 1, 512);
            UpdatePixelation();
        }
    }

    private TextMeshProUGUI targetText;
    private Material pixelMaterial;
    private static readonly int PixelScaleId = Shader.PropertyToID("_PixelScale");

    private void Awake()
    {
        targetText = GetComponent<TextMeshProUGUI>();
        InitMaterial();
    }

    private void InitMaterial()
    {
        Shader shader = Shader.Find("UI/PixelizeShader");
        if (shader == null)
        {
            Debug.LogError("PixelizeShader not found!");
            return;
        }

        // Create an instance of the material to avoid modifying the global one
        pixelMaterial = new Material(shader);
        pixelMaterial.hideFlags = HideFlags.HideAndDontSave;
        
        // Link the material to the TMP component
        targetText.fontSharedMaterial = pixelMaterial;
        UpdatePixelation();
    }

    private void OnValidate()
    {
        // For Editor preview
        if (targetText == null) targetText = GetComponent<TextMeshProUGUI>();
        if (targetText != null && pixelMaterial == null && targetText.fontSharedMaterial != null && targetText.fontSharedMaterial.shader.name == "UI/PixelizeShader")
        {
            pixelMaterial = targetText.fontSharedMaterial;
        }
        UpdatePixelation();
    }

    private void Update()
    {
        UpdatePixelation();
    }

    private void UpdatePixelation()
    {
        if (pixelMaterial != null)
        {
            pixelMaterial.SetFloat(PixelScaleId, pixelScale);
        }
    }
    
    private void OnDestroy()
    {
        if (pixelMaterial != null)
        {
            if (Application.isPlaying)
                Destroy(pixelMaterial);
            else
                DestroyImmediate(pixelMaterial);
        }
    }
}
