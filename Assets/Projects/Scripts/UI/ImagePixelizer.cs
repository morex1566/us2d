using UnityEngine;
using UnityEngine.UI;

public class ImagePixelizer : MonoBehaviour
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

    
    private Image targetImage;
    private Material pixelMaterial;
    private static readonly int PixelScaleId = Shader.PropertyToID("_PixelScale");

    private void Awake()
    {
        targetImage = GetComponent<Image>();
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
        targetImage.material = pixelMaterial;
        UpdatePixelation();
    }

    private void OnValidate()
    {
        // For Editor preview
        if (targetImage == null) targetImage = GetComponent<Image>();
        if (targetImage != null && pixelMaterial == null && targetImage.material != null && targetImage.material.shader.name == "UI/PixelizeShader")
        {
            pixelMaterial = targetImage.material;
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