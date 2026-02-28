using TMPro;
using UnityEngine;

public class ChattingManager : MonoBehaviour
{
    public TextMeshPro serverBuffer;
    public TextMeshPro clientBuffer;

    private void Awake()
    {
        serverBuffer = GetComponent<TextMeshPro>();
        clientBuffer = GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        
    }
}
