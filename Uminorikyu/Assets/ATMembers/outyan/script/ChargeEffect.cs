using UnityEngine;
using UnityEngine.UI;

public class ChargeEffect : MonoBehaviour
{
    [SerializeField] GameObject playerImage;
    [SerializeField] GameObject stamina;
    [SerializeField] GameObject chargeEffect;

    private float previousStamina;

    void Start()
    {
        // 初期値を設定
        previousStamina = stamina.GetComponent<Slider>().value;

        // エフェクトを最初は非表示に
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
        else
        {
            // スタミナが増えていない場合はエフェクトを非表示
            if (chargeEffect != null && chargeEffect.activeSelf)
            {
                chargeEffect.SetActive(false);
            }
        }

        // 現在の値を保存
        previousStamina = currentStamina;
    }
}