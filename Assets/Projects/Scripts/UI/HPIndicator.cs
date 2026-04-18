using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class HPIndicator : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Slider hpBar;
    [SerializeField] private TextMeshProUGUI hpText;



    private void OnEnable()
    {
        playerController.OnColliderTriggered += UpdateHpIndicator;
    }

    private void OnDisable()
    {
        playerController.OnColliderTriggered -= UpdateHpIndicator;
    }

    private void UpdateHpIndicator(float currentHp, float maxHp)
    {
        hpBar.value = currentHp / maxHp;
        hpText.text = $"{currentHp} / {maxHp}";
    }
}