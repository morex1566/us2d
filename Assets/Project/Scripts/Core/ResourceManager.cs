using UnityEngine;

namespace TRPG.Runtime
{
    public class ResourceManager : MonoBehaviourSingleton<ResourceManager>
    {
        private static ResourceManagerSettingsData data;

        public static void Init()
        {
            GetInstance();
            {
                data = Resources.Load<ResourceManagerSettingsData>("SO_ResourceManagerSettings");
            }
        }
    }
}
