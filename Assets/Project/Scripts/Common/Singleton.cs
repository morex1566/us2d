using UnityEngine;

namespace TRPG.Runtime
{
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
        protected static bool isQuitting = false;

        public static T GetInstance()
        {
            if (isQuitting)
            {
                return null;
            }

            // 존재하지 않을 때만 새로 생성
            instance = FindAnyObjectByType<T>();
            if (instance == null)
            {
                // 존재하지 않을 때만 새로 생성
                instanceObj = new GameObject($"[{typeof(T).Name}]");
                instance = instanceObj.AddComponent<T>();
            }
            else
            {
                instanceObj = instance.gameObject;
                instanceObj.name = $"[{typeof(T).Name}]";
            }

            GameObject managerRoot = GetManagerRoot();
            if (instanceObj != managerRoot)
            {
                instanceObj.transform.SetParent(managerRoot.transform);
            }

            return instance;
        }

        protected virtual void OnApplicationQuit()
        {
            isQuitting = true;
        }

        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
                instanceObj = null;
            }
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

        protected static GameObject instanceObj = null;

        protected static T instance = null;

        private const string ManagerRootName = "[Manager]";

        private static GameObject managerRoot = null;

        private static GameObject GetManagerRoot()
        {
            if (managerRoot == null)
            {
                managerRoot = GameObject.Find(ManagerRootName);
            }

            if (managerRoot == null)
            {
                managerRoot = new GameObject(ManagerRootName);
            }

            DontDestroyOnLoad(managerRoot);
            return managerRoot;
        }
    }
}
