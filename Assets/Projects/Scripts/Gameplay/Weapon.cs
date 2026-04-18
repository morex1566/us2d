using UnityEngine;

public interface IWeapon
{
    
}

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected WeaponData data = null;

    [SerializeField] protected Animator animator = null;

    [SerializeField] protected SpriteRenderer spriter = null;



    protected bool isLeft = false;



    public bool IsLeft => isLeft;
}
