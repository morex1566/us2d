using UnityEngine;
using UnityEngine.SceneManagement;
using US2D.Network;

public class SceneManager : MonoBehaviourSingleton<SceneManager>
{
    [SerializeField] private SceneManagerSettings setting;

    public static SceneManagerSettings Setting => instance.setting;

    public void OnEnable()
    {
        setting = Resources.Load<SceneManagerSettings>(AssetPath.SceneManagerSettings);
    }

    public void OnDisable()
    {
        Resources.UnloadAsset(setting);
    }

    public static void Init()
    {
        GetInstance();
    }

    public static int GetBuildIndex(string sceneName)
    {
        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string nameInPath = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (nameInPath == sceneName)
            {
                return i;
            }
        }

        return -1; // 찾지 못한 경우
    }
}
