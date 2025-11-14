using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;

public class StaminaHeal : MonoBehaviour
{
    [SerializeField] float staminaMax;

    //スティック位置の過去情報
    Vector2 stickInputLast = new Vector2(0.0f, 0.0f);
    //スタミナの回復速度
    [SerializeField] float healSpeed;
    //プレイヤーの取得
    [SerializeField]GameObject player;
    [SerializeField] ParticleSystem particle;

    PlayerInputScript input;
    Gamepad gamepad;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //ゲームパッドの取得
        input = GetComponent<PlayerInputScript>();
        gamepad = input.controller;
        if (gamepad != null)
        {
            if (gamepad.name == "XInputControllerWindows")
            {
                healSpeed *= 2.5f;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gamepad != null)
        {
            //最新のレバーの位置の取得
            Vector2 stickInput = input.GetStickValue(gamepad);

            if (stickInput != stickInputLast)
            {
                GetComponent<Slider>().value += healSpeed;
                stickInputLast = stickInput;

                if (GetComponent<Slider>().value >= staminaMax)
                {
                    if (!player.GetComponent<OriiPlayerMove>().nowEvent)
                    {
                        if (particle != null)
                        {
                            particle.Stop();
                        }
                        player.GetComponent<OriiPlayerMove>().barnOut = false;
                        Debug.Log(GetComponent<Slider>().value);
                    }
                    else
                    {
                        GetComponent<Slider>().value = staminaMax;
                    }
                }
            }
        }
    }
}
