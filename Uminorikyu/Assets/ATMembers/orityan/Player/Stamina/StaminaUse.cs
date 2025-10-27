using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;

public class StaminaUse : MonoBehaviour
{
    Slider slider;
    [SerializeField] GameObject player;
    Gamepad gamepad;
    Gamepad controller;

    [SerializeField] float stickPower;
    [SerializeField] float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //ゲームパッドの取得
        if(player.GetComponent<OriiPlayerMove>().gamepad.name == "DualSenseGamepadHID")
        {
            gamepad = player.GetComponent<OriiPlayerMove>().gamepad;
        }
        else
        {
            controller = player.GetComponent<OriiPlayerMove>().gamepad;
        }


        if (controller != null)
        {
            Debug.Log("XInput Controller detected!");
        }

        if (gamepad != null)
        {
            Debug.Log("ゲームパッドが接続されています");
        }

        //スタミナについてるスライダーコンポーネントの取得
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        //ゲームパッドが入っていれば
        if (gamepad != null)
        {
            // 左スティックの入力値を取得
            Vector2 stickInput = gamepad.leftStick.ReadValue();

            //コントローラーの×ボタンを押してるときに
            if (gamepad.buttonSouth.IsPressed())
            {
                //左のレバーが少しでも倒されているとき
                if (stickInput.x > stickPower || stickInput.x < -stickPower &&
                    stickInput.y > stickPower || stickInput.y < -stickPower)
                {
                    //スタミナがあれば
                    if (slider.value > 0.0f)
                    {
                        slider.value -= 0.1f;
                        player.GetComponent<OriiPlayerMove>().moveSpeed = speed;
                    }
                    else
                    {
                        player.GetComponent<OriiPlayerMove>().moveSpeed = 0.2f;
                    }
                }
            }
            else
            {
                player.GetComponent<OriiPlayerMove>().moveSpeed = 0.2f;
            }
        }

        if (controller != null)
        {
            // 左スティックの入力値を取得
            Vector2 stickInput = controller.dpad.ReadValue();

            //コントローラーの×ボタンを押してるときに
            if (controller.bButton.IsPressed())
            {
                //左のレバーが少しでも倒されているとき
                if (stickInput.x > stickPower || stickInput.x < -stickPower &&
                    stickInput.y > stickPower || stickInput.y < -stickPower)
                {
                    //スタミナがあれば
                    if (slider.value > 0.0f)
                    {
                        slider.value -= 0.1f;
                        player.GetComponent<OriiPlayerMove>().moveSpeed = speed;
                    }
                    else
                    {
                        player.GetComponent<OriiPlayerMove>().moveSpeed = 0.2f;
                    }
                }
            }
            else
            {
                player.GetComponent<OriiPlayerMove>().moveSpeed = 0.2f;
            }
        }
    }
}
