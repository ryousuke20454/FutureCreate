using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;


public class OriiPlayerMove : MonoBehaviour
{
    Gamepad gamepad;
    XInputController controller;
    [SerializeField] public float speed;
    Rigidbody2D rb;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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



        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gamepad != null)
        {
            // 左スティックの入力値を取得
            Vector2 stickInput = gamepad.leftStick.ReadValue();

            //移動
            gameObject.transform.position =
                new Vector2(gameObject.transform.position.x + stickInput.x * speed,
                gameObject.transform.position.y + stickInput.y * speed);
        }
        
        if(controller != null)
        {
            // 左スティックの入力値を取得
            Vector2 stickInput = controller.dpad.ReadValue();

            //移動
            gameObject.transform.position =
                new Vector2(gameObject.transform.position.x + stickInput.x * speed,
                gameObject.transform.position.y + stickInput.y * speed);

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
    }
}
