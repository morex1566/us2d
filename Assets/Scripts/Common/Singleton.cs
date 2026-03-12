
using UnityEditorInternal;
using UnityEngine;

namespace NetCommon
{
    /// <summary>
    /// Singleton base class
    /// </summary>
    public class Singleton<T> where T : class, new()
    {
        public static T GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T();
            }

            return _instance;
        }

        protected static T _instance = null;
    }

    public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T GetInstance()
        {
            if (_instanceObj == null && _instance == null)
            {
                _instanceObj = new(nameof(T));
                {
                    _instance = _instanceObj.AddComponent<T>();
                }
            }

            return _instance;
        }

        protected static GameObject _instanceObj = null;

        protected static T _instance = null;
    }
}
