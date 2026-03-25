using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    public Vector2 MaxSpeed;

    public Vector2 MaxAccel;

    public Vector2 MaxDecel;
}
