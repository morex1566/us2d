using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class TextBlinker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text; // 깜빡일 대상 UI

    [SerializeField] private float duration = 2f; // 한 번 깜빡이는 시간

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        StartBlink();
    }

    public void StartBlink()
    {
        if (text == null)
        {
            return;
        }

        // 1. 투명도를 0으로 변경
        // 2. 무한 반복(-1) 및 왕복(Yoyo) 설정
        // 3. 부드러운 전환을 위해 Ease 설정 (선택 사항)
        text.DOFade(0.02f, duration)
                   .SetLoops(-1, LoopType.Yoyo)
                   .SetEase(Ease.InOutSine);
    }

    // 깜빡임을 멈추고 싶을 때 호출
    public void StopBlinking()
    {
        text.DOKill(); // 해당 트윈 종료
        text.color = Color.white; // 불투명 상태로 복구
    }
}
