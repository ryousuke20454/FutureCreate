using UnityEngine;
using UnityEngine.UI;

public class PlayerAndStaminaInfo : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject stamina;

    [SerializeField] private float maxStamina = 100f;
    private float currentStamina;

    private Slider staminaSlider;

    private void Awake()
    {
        if (stamina != null)
            staminaSlider = stamina.GetComponent<Slider>();

        currentStamina = maxStamina;

        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }
    }

    private void UpdateSlider()
    {
        if (staminaSlider != null)
            staminaSlider.value = currentStamina;
    }

    public void TakeDamage(float amount)
    {
        // 既にバーンアウト中なら処理しない
        if (player.GetComponent<OriiPlayerMove>().barnOut)
            return;

        currentStamina -= amount;
        if (currentStamina < 0) currentStamina = 0;

        UpdateSlider();

        if (currentStamina <= 0)
        {
            GetExplosionTrash();
        }
    }


    public void GetExplosionTrash()
    {
        stamina.GetComponent<Slider>().value = 0;
        player.GetComponent<OriiPlayerMove>().barnOut = true;
    }
}
