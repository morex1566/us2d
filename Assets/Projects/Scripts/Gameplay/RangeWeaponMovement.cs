using UnityEngine;

[System.Serializable]
public class RangeWeaponMovement
{
    [Header("Kick")]
    public float kickbackDistance = 0.1f;
    public float kickbackSpeed = 20f;

    [Header("Rotation")]
    public float recoilAngle = 5f;
    public float rotationSpeed = 20f;

    [Header("Recovery")]
    public float positionReturnSpeed = 12f;
    public float rotationReturnSpeed = 10f;

    [Header("Clamp")]
    public float maxKickbackDistance = 0.25f;
    public float maxRecoilAngle = 15f;

    // 사격 시 콜, 무기가 튕겨 올라감
    private void ApplyRecoil()
    {
    }
    
    // 사격 시 콜, 무기가 뒤로 밀림
    private void ApplyKickback()
    {
    }
}