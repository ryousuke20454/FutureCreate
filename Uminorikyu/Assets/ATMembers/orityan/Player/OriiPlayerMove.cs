using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;


public class OriiPlayerMove : MonoBehaviour
{
    public Gamepad gamepad;
    GameObject player;
    XInputController controller;
    [SerializeField] public float moveSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //別のプレイヤーを検索
        player = GameObject.FindWithTag("Player");

        //別のプレイヤーとコントローラーが被らないようにする
        if (player.GetComponent<OriiPlayerMove>().gamepad !=
            PlayerControllerManager.controllerManager.player1Controller)
        {
            gamepad = PlayerControllerManager.controllerManager.player1Controller;
        }
        else
        {
            gamepad = PlayerControllerManager.controllerManager.player2Controller;
        }

        //デバッグ
        if (gamepad == null)
        {
            Debug.Log("コントローラーが接続されていません");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gamepad != null)
        {
            Vector2 stickInput;

            if (gamepad.name == "DualSenseGamepadHID")
            {
                // 左スティックの入力値を取得
                stickInput = gamepad.leftStick.ReadValue();
            }
            else
            //if (gamepad.name == "XInputControllerWindows")
            {
                // 左スティックの入力値を取得
                stickInput = gamepad.dpad.ReadValue();

                /*
                 * 入力習得一覧
                if (controller.aButton.isPressed)
                {
                    Debug.Log("A Button Pressed");
                }
                if (controller.bButton.isPressed)
                {
                    Debug.Log("B Button Pressed");
                }
                if (controller.xButton.isPressed)
                {
                    Debug.Log("X Button Pressed");
                }
                if (controller.yButton.isPressed)
                {
                    Debug.Log("Y Button Pressed");
                }

                controller.dpad.ReadValue()

                */
            }
            //移動
            gameObject.transform.position =
                new Vector2(gameObject.transform.position.x + stickInput.x * moveSpeed,
                gameObject.transform.position.y + stickInput.y * moveSpeed);
        }
    }
}
