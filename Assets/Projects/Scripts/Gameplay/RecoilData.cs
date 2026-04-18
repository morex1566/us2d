using UnityEngine;

[CreateAssetMenu(fileName = "RecoilData", menuName = "Scriptable Objects/RecoilData")]
public class RecoilData : ScriptableObject
{
    [Header("Kick")]
    public float kickbackDistance = 0.1f;
    public float kickbackStrength = 20f;

    [Header("Rotation")]
    public float recoilAngle = 5f;
    public float rotationStrength = 20f;

    [Header("Recovery")]
    public float kickbackRecoveryStrength = 12f;
    public float recoilRecoveryStrength = 10f;

    [Header("Clamp")]
    public float maxKickbackDistance = 0.25f;
    public float maxRecoilAngle = 15f;
}