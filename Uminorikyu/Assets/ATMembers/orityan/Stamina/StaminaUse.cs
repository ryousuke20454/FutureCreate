using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;

public class StaminaUse : MonoBehaviour
{
    Gamepad gamepad;
    Slider slider;
    GameObject player;
    XInputController controller;

    [SerializeField] float stickPower;
    [SerializeField] float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //ゲームパッドの取得
        gamepad = Gamepad.current;
        controller = Gamepad.current as XInputController;

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
        //プレイヤーの取得
        player = GameObject.FindWithTag("Player");
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
                        player.GetComponent<OriiPlayerMove>().speed = speed;
                    }
                    else
                    {
                        player.GetComponent<OriiPlayerMove>().speed = 0.2f;
                    }
                }
            }
            else
            {
                player.GetComponent<OriiPlayerMove>().speed = 0.2f;
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
                        player.GetComponent<OriiPlayerMove>().speed = speed;
                    }
                    else
                    {
                        player.GetComponent<OriiPlayerMove>().speed = 0.2f;
                    }
                }
            }
            else
            {
                player.GetComponent<OriiPlayerMove>().speed = 0.2f;
            }
        }
    }
}
