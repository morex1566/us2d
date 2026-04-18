using UnityEngine;

[CreateAssetMenu(fileName = "RangeWeaponData", menuName = "Scriptable Objects/RangeWeaponData")]
public class RangeWeaponData : WeaponData
{
    [Header("Projectile")]
    [SerializeField] private Projectile projectilePf = null;

    public Projectile ProjectilePrefab => projectilePf;



    [Header("Audio")]
    public AudioClip Fire;

    public AudioClip Cocking;

    public AudioClip ExtractMag;



    [Header("Setup")]
    public int MaxAmmo;

    public FireMode Mode;

    public float ReloadTime;
}
