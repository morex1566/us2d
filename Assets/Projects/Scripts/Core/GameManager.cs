using UnityEngine;
using DG.Tweening;
using US2D.Network;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class GameManager : MonoBehaviourSingleton<GameManager>
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnBeforeSceneLoaded()
    {
        Init();
        DOTween.Init();
        InputManager.Init();
        SceneManager.Init();
        UIManager.Init();
        NetManager.Init();
    }

    public static void Init()
    {
        GetInstance();
    }
}
