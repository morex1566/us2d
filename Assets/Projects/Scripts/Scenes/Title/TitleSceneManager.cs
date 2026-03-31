using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    [Space(10), Header("Canvas")]

    [SerializeField] private Canvas overlayCanvas;
    [SerializeField] private Canvas cameraCanvas;


    [Space(10), Header("Init Phase Fadeout")]

    [SerializeField] private GameObject fadeInScreenPf;
    private GameObject fadeInScreenObj;
    public UnityEvent OnFadeInScreenCompleted = new();


    [Space(10), Header("Press To Continue")]

    [SerializeField] private GameObject guideMessagePf;
    [SerializeField] private GameObject backgroundObj;
    [SerializeField] private GameObject titleNameObj;
    [SerializeField] private GameObject fadeOutScreenPf;

    private GameObject fadeOutScreenObj;
    private GameObject guideMessageObj;
    public UnityEvent OnGuideMessagePressed = new();


    [Space(10), Header("Auth")]

    [SerializeField] public UnityEvent OnGoogleAuthPressed = new();
    [SerializeField] public UnityEvent OnGuestAuthPressed = new();


    [Space(10), Header("Menu")]

    [SerializeField] public UnityEvent OnStartPressed = new();
    [SerializeField] public UnityEvent OnOptionPressed = new();
    [SerializeField] public UnityEvent OnCreditsPressed = new();
    [SerializeField] public UnityEvent OnExitPressed = new();


    public void InvokeFadeInScreenCompleted() => OnFadeInScreenCompleted?.Invoke();
    public void InvokePressToContinue() => OnGuideMessagePressed?.Invoke();
    public void InvokeGoogleAuth() => OnGoogleAuthPressed?.Invoke();
    public void InvokeGuestAuth() => OnGuestAuthPressed?.Invoke();
    public void InvokeStart() => OnStartPressed?.Invoke();
    public void InvokeOption() => OnOptionPressed?.Invoke();
    public void InvokeCredits() => OnCreditsPressed?.Invoke();
    public void InvokeExit() => OnExitPressed?.Invoke();


    private void OnEnable()
    {
        OnFadeInScreenCompleted.AddListener(OnDestroyFadeInScreen);
        OnFadeInScreenCompleted.AddListener(OnInstantiateGuideMessage);

        OnGuideMessagePressed.AddListener(OnCheckIntegrity);
        OnGuideMessagePressed.AddListener(OnDecreasePixelScaleGradually);
        OnGuideMessagePressed.AddListener(OnInstantiateFadeOutScreen);
    }

    private void OnDisable()
    {
        OnFadeInScreenCompleted.RemoveAllListeners();

        OnGuideMessagePressed.RemoveAllListeners();
    }

    private void OnDestroyFadeInScreen()
    {
        if (fadeInScreenObj)
        {
            Destroy(fadeInScreenObj);
        }
    }

    // PRESS SCREEN TO CONTINUE
    private void OnInstantiateGuideMessage()
    {
        guideMessageObj = Instantiate(guideMessagePf, overlayCanvas.transform, false);
        {
            Button button = guideMessageObj.GetComponentInChildren<Button>();
            button.onClick.AddListener(InvokePressToContinue);
        }
    }

    // 화면이 점점 픽셀화 되어감
    private void OnDecreasePixelScaleGradually()
    {
        var backgroundPixelizer = backgroundObj.GetComponentInChildren<ImagePixelizer>();
        if (backgroundPixelizer != null)
        {
            backgroundPixelizer.PixelScale = 512f;
            DOTween.To(() => backgroundPixelizer.PixelScale, x => backgroundPixelizer.PixelScale = x, 32f, 1.4f).SetEase(Ease.OutQuad);
        }

        var titleNamePixelizer = titleNameObj.GetComponentInChildren<ImagePixelizer>();
        if (titleNamePixelizer != null)
        {
            titleNamePixelizer.PixelScale = 512f;
            DOTween.To(() => titleNamePixelizer.PixelScale, x => titleNamePixelizer.PixelScale = x, 32f, 1.4f).SetEase(Ease.OutQuad);
        }

        var guideMessagePixelizer = guideMessageObj.GetComponentInChildren<TextPixelizer>();
        if (guideMessagePixelizer != null)
        {
            guideMessagePixelizer.PixelScale = 512f;
            DOTween.To(() => guideMessagePixelizer.PixelScale, x => guideMessagePixelizer.PixelScale = x, 32f, 1.4f).SetEase(Ease.OutQuad);
        }
    }

    private void OnInstantiateFadeOutScreen()
    {
        fadeOutScreenObj = Instantiate(fadeOutScreenPf, overlayCanvas.transform, false);
    }

    // 서버의 Manifest와 비교
    private void OnCheckIntegrity()
    {
        if (guideMessageObj)
        {
            Destroy(guideMessageObj);
        }

        // 랜더링 : 로딩 UI
        GameObject loadingUI = Instantiate(ResourceManager.Setting.loadingSpinnerPf, overlayCanvas.transform, false);
        loadingUI.GetComponentInChildren<TextMeshProUGUI>().text = "Checking Integrity";

        StartCoroutine(OnCheckIntegrityImpl(loadingUI));
    }

    // 서버의 Manifest와 비교
    private IEnumerator OnCheckIntegrityImpl(GameObject loadingUI)
    {
        yield return StartCoroutine(CheckVersionIntegrityInternal(loadingUI));

        yield return StartCoroutine(CheckResourceIntegrityInternal(loadingUI));

        Destroy(loadingUI);
    }

    // 서버의 Manifest와 비교
    private IEnumerator CheckVersionIntegrityInternal(GameObject loadingUI)
    {
        yield return new WaitForSeconds(0.5f);
    }

    // 서버의 Manifest와 비교
    private IEnumerator CheckResourceIntegrityInternal(GameObject loadingUI)
    {
        yield return new WaitForSeconds(0.5f);
    }
}
