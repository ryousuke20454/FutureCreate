using UnityEngine;
using UnityEngine.UI;

public class ChargeEffect : MonoBehaviour
{
    [SerializeField] GameObject playerImage;
    [SerializeField] GameObject stamina;
    [SerializeField] GameObject chargeEffect;
    [SerializeField] float effectDuration = 0.3f; // エフェクト表示を保持する時間

    private float previousStamina;
    private float effectTimer = 0f;

    void Start()
    {
        previousStamina = stamina.GetComponent<Slider>().value;

        if (chargeEffect != null)
        {
            chargeEffect.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        Slider staminaSlider = stamina.GetComponent<Slider>();
        float currentStamina = staminaSlider.value;

        // スタミナが増えているかチェック
        if (currentStamina > previousStamina)
        {
            // タイマーをリセット
            effectTimer = effectDuration;

            // エフェクトを表示
            if (chargeEffect != null && !chargeEffect.activeSelf)
            {
                chargeEffect.SetActive(true);
            }

            // playerImageの位置にエフェクトを配置
            if (playerImage != null)
            {
                chargeEffect.transform.position = playerImage.transform.position;
            }
        }

        // タイマーを減らす
        if (effectTimer > 0)
        {
            effectTimer -= Time.fixedDeltaTime;

            // タイマーが0になったらエフェクトを非表示
            if (effectTimer <= 0 && chargeEffect != null)
            {
                chargeEffect.SetActive(false);
            }
        }

        previousStamina = currentStamina;
    }
}