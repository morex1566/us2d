using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SplashSceneManager : MonoBehaviour
{
    [SerializeField] public UnityEvent<Action> OnTitleSceneLoaded = null;

    private void Start()
    {
        StartCoroutine(AsyncLoadTitleSceneImpl());
    }

    private IEnumerator AsyncLoadTitleSceneImpl()
    {
        var loadingOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(SceneManager.Setting.Title, LoadSceneMode.Single);
        {
            loadingOperation.allowSceneActivation = false;
        }

        yield return new WaitUntil(() => loadingOperation.progress >= 0.9f);

        OnTitleSceneLoaded.Invoke(() => 
        {
            loadingOperation.allowSceneActivation = true;
        });

        while (!loadingOperation.isDone)
        {
            yield return null;
        }
    }
}
