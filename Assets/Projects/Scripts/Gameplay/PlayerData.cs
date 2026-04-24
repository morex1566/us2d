using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : CreatureData
{
    [field: SerializeField] public float RollSpCost { get; set; } = 25f;

    [field: SerializeField] public float SpeedAccelAtRollMultiplier { get; set; } = 3f;

    [field: SerializeField] public float SpeedDecelAtRollMultiplier { get; set; } = 0.3f;

    [field: Header("Jump")]
    [field: SerializeField] public float JumpVelocity { get; set; } = 6.5f;

    [field: SerializeField] public float JumpGravity { get; set; } = -18f;

    [field: SerializeField] public float AirControlMultiplier { get; set; } = 0.85f;
}
