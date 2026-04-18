using TMPro;
using UnityEngine;

public class WeaponIndicator : MonoBehaviour
{
    [SerializeField] private RangeWeaponController weaponController;

    [SerializeField] private TextMeshProUGUI ammoText;



    private void OnEnable()
    {
        weaponController.OnAmmoChanged += UpdateAmmoText;
    }

    private void OnDisable()
    {
        weaponController.OnAmmoChanged -= UpdateAmmoText;
    }

    private void UpdateAmmoText(int currentAmmo, int maxAmmo)
    {
        ammoText.text = $"{currentAmmo}/{maxAmmo}";
    }
}