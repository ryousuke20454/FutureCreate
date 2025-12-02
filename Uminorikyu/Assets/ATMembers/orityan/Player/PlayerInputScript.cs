using UnityEngine;
using UnityEngine.InputSystem;

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
            string[] joysticks = Input.GetJoystickNames();
            for (int i = 0; i < joysticks.Length; i++)
            {
                if (joysticks[i] != "")
                {
                    joystick = joysticks[i + PlayerNum] ;
                    break;
                }
            }

            Debug.Log(joystick);
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
            //アケコンだったら
            else if (joystick == "Controller (HORI Fighting Stick mini)")
            {
                // 左スティックの入力値を取得
                value = gamepad.dpad.ReadValue();
            }
            else if (joystick == "Controller (HORI Fighting Stick mini for PC)")
            {
                // 左スティックの入力値を取得
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
