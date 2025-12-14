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

    public void TakeDamage(float amount)
    {
        // 既にバーンアウト中なら処理しない
        if (player.GetComponent<OriiPlayerMove>().barnOut)
            return;

        staminaSlider.value -= amount;
        if (staminaSlider.value < 0) staminaSlider.value = 0;

        if (staminaSlider.value <= 0)
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
