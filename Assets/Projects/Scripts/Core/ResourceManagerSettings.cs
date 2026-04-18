using UnityEngine;

[CreateAssetMenu(fileName = "ResourceManagerSettings", menuName = "Scriptable Objects/Setting/ResourceManagerSettings")]
public class ResourceManagerSettings : ScriptableObject
{
    [Space(10), Header("Loading")]

    [SerializeField] public GameObject loadingSpinnerPf;
}