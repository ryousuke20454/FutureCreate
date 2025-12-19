using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class PlayerInputScript : MonoBehaviour
{
    //インスペクターでプレイヤー番号を入力する
    [SerializeField] public int PlayerNum;
    //コントローター情報
    public Gamepad controller;
    public string joystick;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //コントローラー情報を取得する
        if (PlayerControllerManager.controllerManager != null)
            controller = PlayerControllerManager.controllerManager.player[PlayerNum].gamepad;

        //デバッグ
        if (controller == null)
        {
            Debug.Log("コントローラーが接続されていません");
        }
        else
        {
            string[] joysticksNames = Input.GetJoystickNames();
            string[] joysticks = new string[2];

            for (int i = 0; i < joysticksNames.Length; i++)
            {
                if (!string.IsNullOrEmpty(joysticksNames[i].Trim()))
                {
                    if (joysticks[0] == null)
                    {
                        joysticks[0] = joysticksNames[i];
                        continue;
                    }

                    if (joysticks[1] == null)
                    {
                        joysticks[1] = joysticksNames[i];
                        break;                    
                    }
                }
            }

            joystick = joysticks[PlayerNum];
        }
    }

    public Vector2 GetStickValue(Gamepad gamepad)
    {
        Vector2 value = new Vector2(0.0f, 0.0f);

        if (gamepad != null)
        {
            //デュアルセンスだったら
            if (gamepad.name == "DualSenseGamepadHID")
            {
                // 左スティックの入力値を取得
                value = gamepad.leftStick.ReadValue();
            }
            //// アケコンだったら
            //else if (joystick == "Controller (HORI Fighting Stick mini)")
            //{
            //    // 左スティックの入力値を取得
            //    value = gamepad.dpad.ReadValue();
            //}
            //else if (joystick == "Controller (HORI Fighting Stick mini for PC)")
            //{
            //    // 左スティックの入力値を取得
            //    value = gamepad.leftStick.ReadValue();
            //}
            else if (gamepad.name == "XInputControllerWindows")
            {
                value = gamepad.dpad.ReadValue();
            }
            else if (gamepad.name == "XInputControllerWindows1")
            {
                value = gamepad.leftStick.ReadValue();
            }
        }

        return value;
    }

    public bool GetTriangleButton(Gamepad gamepad)
    {
        bool button = false;
        if (gamepad != null)
        {
            button = gamepad.buttonNorth.IsPressed();
        }
        return button;
    }

    public bool GetSquareButton(Gamepad gamepad)
    {
        bool button = false;
        if (gamepad != null)
        {
            button = gamepad.buttonWest.IsPressed();
        }
        return button;
    }

    public bool GetCircleButton(Gamepad gamepad)
    {
        bool button = false;
        if (gamepad != null)
        {
            button = gamepad.buttonEast.IsPressed();
        }
        return button;
    }

    public bool GetCrossButton(Gamepad gamepad)
    {
        bool button = false;
        if (gamepad != null)
        {
            button = gamepad.buttonSouth.IsPressed();
        }
        return button;
    }
}
