using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
[RequireComponent(typeof(SpriteRenderer))]
public class WorldImageVideoRenderer : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private SpriteRenderer spriter;

    [SerializeField] private VideoClip targetClip;
    [SerializeField] private Sprite targetSprite;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        {
            videoPlayer.playOnAwake = true;
            videoPlayer.clip = targetClip;
            videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
            videoPlayer.targetMaterialRenderer = GetComponent<Renderer>();
            videoPlayer.targetMaterialProperty = "_MainTex";
        }

        spriter = GetComponent<SpriteRenderer>();
        {
            targetSprite = spriter.sprite;
        }
    }

    private void Start()
    {
        // 1. Video Player 설정
        videoPlayer.playOnAwake = true;
        videoPlayer.clip = targetClip;

        // 2. 렌더링 방식 설정 (Material Override)
        // 특정 Renderer(예: Quad)의 Material에 직접 영상을 출력합니다.
        videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
        videoPlayer.targetMaterialRenderer = GetComponent<Renderer>();
        videoPlayer.targetMaterialProperty = "_MainTex";

        videoPlayer.Play();
    }

    private void OnDestroy()
    {
        
    }
}