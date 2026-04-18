using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class RangeWeaponController : MonoBehaviour
{

    [SerializeField] private Transform weaponSocket;

    [SerializeField] private Transform weaponRotationPivot;

    [SerializeField] private Weapon weapon;

    [SerializeField] private PlayerController playerController;

    [SerializeField] private RangeWeaponMovement movement = new RangeWeaponMovement();

    [SerializeField] private RangeWeaponView view = new RangeWeaponView();




    private Vector2 currLookDirection = Vector2.right;

    private Vector2 prevLookDirection = Vector2.right;



       
    public Action<int, int> OnAmmoChanged;




    private void OnValidate()
    {

    }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {

    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    private void Update()
    {

    }

    private void OnDrawGizmos()
    {
        if (weaponRotationPivot == null || weaponSocket == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(weaponRotationPivot.position, weaponSocket.position);
    }
}
