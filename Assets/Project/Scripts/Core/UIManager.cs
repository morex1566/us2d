using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TRPG.Runtime
{
    public class UIManager : MonoBehaviourSingleton<UIManager>
    {
        public UnityEvent<float, float> OnResolutionChanged = new();

        public static void Init()
        {
        }
    }
}
