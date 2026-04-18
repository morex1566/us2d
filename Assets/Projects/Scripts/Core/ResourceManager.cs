using UnityEngine;
using US2D.Network;

public class ResourceManager : MonoBehaviourSingleton<ResourceManager>
{
    [SerializeField] private ResourceManagerSettings setting;

    public static ResourceManagerSettings Setting => GetInstance().setting;

    public void OnEnable()
    {
        setting = Resources.Load<ResourceManagerSettings>(AssetPath.ResourceManagerSettings);
    }

    public void OnDisable()
    {
        Resources.UnloadAsset(setting);
    }
}
