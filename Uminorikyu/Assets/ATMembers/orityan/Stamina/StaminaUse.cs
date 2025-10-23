using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StaminaUse : MonoBehaviour
{
    Gamepad gamepad;
    Slider slider;
    GameObject player;

    [SerializeField] float stickPower;
    [SerializeField] float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //ゲームパッドの取得
        gamepad = Gamepad.current;
        if (gamepad != null)
        {
            Debug.Log("コントローラがありません");
        }

        //スタミナについてるスライダーコンポーネントの取得
        slider = GetComponent<Slider>();
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (gamepad != null)
        {

            // 左スティックの入力値を取得
            Vector2 stickInput = gamepad.leftStick.ReadValue();

            //PS5の×ボタンを押してるときに
            if (gamepad.buttonSouth.IsPressed())
            {
                //左のレバーが少しでも倒されているとき
                if (stickInput.x > stickPower || stickInput.x < -stickPower &&
                    stickInput.y > stickPower || stickInput.y < -stickPower)
                {
                    //スタミナがあれば
                    if (slider.value > 0.0f)
                    {
                        slider.value -= speed;
                    }
                }
            }
        }
    }
}
