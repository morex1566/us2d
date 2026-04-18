using UnityEngine;
using UnityEngine.Pool;

public enum FireMode
{
    Auto,
    SemiAuto,
    Burst
}

public class RangeWeapon : Weapon
{
    private ObjectPool<Projectile> projectilePool;

    private int maxAmmo;

    private int currAmmo;



    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        projectilePool = new ObjectPool<Projectile>
        (
            OnCreateProjectile,
            OnGetProjectile,
            OnReleaseProjectile,
            OnDestroyProjectile,
            false,
            maxAmmo,
            Mathf.Max(maxAmmo, 1)
        );

        maxAmmo = (data as RangeWeaponData).MaxAmmo;
        currAmmo = maxAmmo;
    }

    public bool TryFire()
    {
        if (currAmmo <= 0)
        {
            return false;
        }

        currAmmo--;
        projectilePool?.Get();
        return true;
    }

    public bool TryReload()
    {
        if (currAmmo >= maxAmmo)
        {
            return false;
        }

        currAmmo = maxAmmo;
        return true;
    }

    private Projectile OnCreateProjectile()
    {
        Projectile projectile = Instantiate((data as RangeWeaponData).ProjectilePrefab, transform.position, transform.rotation, transform);
        projectile.gameObject.SetActive(false);
        return projectile;
    }

    private void OnGetProjectile(Projectile projectile)
    {
        projectile.transform.SetParent(transform, false);
        projectile.transform.SetPositionAndRotation(transform.position, transform.rotation);
        projectile.gameObject.SetActive(true);
    }

    private void OnReleaseProjectile(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
        projectile.transform.SetParent(transform, false);
    }

    private void OnDestroyProjectile(Projectile projectile)
    {
        Destroy(projectile.gameObject);
    }
}
