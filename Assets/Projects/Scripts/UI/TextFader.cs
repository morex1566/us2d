using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class TextFader : MonoBehaviour
{
    public enum ExecutionType
    { 
        Instant,
        Invoke
    }

    [SerializeField] private TextMeshProUGUI text;

    // Fade되는데 걸리는 시간
    [SerializeField] private float duration = 2f;

    [SerializeField, Range(0f, 1f)] private float endAlpha = 0f;

    [SerializeField] private ExecutionType executionType = ExecutionType.Instant;

    public UnityEvent OnFadeCompleted = new();

    private Tweener fading;

    private void Awake()
    {
        if (text == null)
        {
            text = GetComponent<TextMeshProUGUI>();
        }
    }

    private void Start()
    {
        switch (executionType)
        {
            case ExecutionType.Instant:
                StartFade();
                break;

            case ExecutionType.Invoke:
                break;

            default:
                StartFade();
                break;
        }
    }

    public void StartFade(Action onCompleted = null)
    {
        if (text == null)
        {
            return;
        }

        fading = text.DOFade(endAlpha, duration).OnComplete(() => 
        {
            onCompleted?.Invoke();
            OnFadeCompleted?.Invoke();
        });
    }

    public void StopFade()
    {
        text.DOKill();
        // 알파값만 1로 복구
        Color color = text.color;
        color.a = 1f;
        text.color = color;
    }
}
