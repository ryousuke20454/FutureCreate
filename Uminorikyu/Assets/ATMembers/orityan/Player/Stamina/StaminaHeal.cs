using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class StaminaHeal : MonoBehaviour
{
    Gamepad gamepad;
    XInputController controller;
    //スティック位置の過去情報
    Vector2 stickInputLast = new Vector2(0.0f, 0.0f);

    [SerializeField] float healSpeed;

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
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gamepad != null)
        {
            //最新のレバーの位置の取得
            Vector2 stickInput = gamepad.leftStick.ReadValue();

            if (stickInput.x != 0.0f && 
                stickInput.y != 0.0f &&
                stickInput != stickInputLast)
            {
                GetComponent<Slider>().value += healSpeed;
                stickInputLast = stickInput;
            }
        }
        if (controller != null)
        {
            //最新のレバーの位置の取得
            Vector2 stickInput = controller.dpad.ReadValue();

            if (stickInput != stickInputLast)
            {
                GetComponent<Slider>().value += healSpeed * 1.8f;
                stickInputLast = stickInput;
            }
        }
    }
}
