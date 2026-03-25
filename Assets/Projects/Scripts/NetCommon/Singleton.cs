using UnityEngine;

namespace Common
{
    /// <summary>
    /// Singleton base class
    /// </summary>
    public class Singleton<T> where T : class, new()
    {
        public static T GetInstance()
        {
            if (instance == null)
            {
                instance = new T();
            }

            return instance;
        }

        protected static T instance = null;
    }

    public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T GetInstance()
        {
            // 존재하지 않을 때만 새로 생성
            instance = FindAnyObjectByType<T>();
            if (instance == null)
            {
                // 존재하지 않을 때만 새로 생성
                instanceObj = new GameObject(typeof(T).Name);
                instance = instanceObj.AddComponent<T>();
            }
            else
            {
                instanceObj = instance.gameObject;
            }

            return instance;
        }

        public static void Destroy()
        {
            if (instanceObj != null)
            {
                Destroy(instanceObj);

                instanceObj = null;
                instance = null;
            }
        }

        public void Start()
        {
            DontDestroyOnLoad(instanceObj);
        }

        protected static GameObject instanceObj = null;

        protected static T instance = null;
    }
}
