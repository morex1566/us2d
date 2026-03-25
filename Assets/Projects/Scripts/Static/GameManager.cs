using NetCore;
using UnityEngine;
using Common;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class GameManager : MonoBehaviourSingleton<GameManager>
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnAfterSceneLoaded()
    {
        NetManager.Init();
        InputManager.Init();
    }
}