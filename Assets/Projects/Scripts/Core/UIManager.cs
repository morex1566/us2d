using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using US2D.Network;

public class UIManager : MonoBehaviourSingleton<UIManager>
{
    public UnityEvent<float, float> OnResolutionChanged = new();

    private ReferenceResolutionCanvas refResolutionCanvas;

    public static void Init()
    {
        GetInstance().CreateReferenceResolutionCanvas();
    }

    public Vector2 GetReferenceResolution()
    {
        return refResolutionCanvas.canvasScaler.referenceResolution;
    }

    /// <summary>
    /// 해상도 변경을 감지할 전용 객체 생성
    /// </summary>
    private void CreateReferenceResolutionCanvas()
    {
        // 이미 트래커가 있다면 생략
        if (refResolutionCanvas != null) return;

        // 전용 트래커 오브젝트 생성
        GameObject referenceCanvasInstanceObj = new GameObject("[ResolutionReference]");

        // DontDestroyOnLoad 관리
        referenceCanvasInstanceObj.transform.SetParent(this.transform);

        // 필수 컴포넌트 추가
        var canvas = referenceCanvasInstanceObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;      
        
        var canvasScalar = referenceCanvasInstanceObj.AddComponent<CanvasScaler>();
        {
            canvasScalar.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScalar.referenceResolution = new Vector2(float.Parse(UnityConstant.JsonConfig.Resolution_Width), float.Parse(UnityConstant.JsonConfig.Resolution_Height));
        }

        // 전용 리스너 추가
        refResolutionCanvas = referenceCanvasInstanceObj.AddComponent<ReferenceResolutionCanvas>();
    }
}
