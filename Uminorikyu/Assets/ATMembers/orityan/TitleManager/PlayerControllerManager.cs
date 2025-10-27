using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerManager : MonoBehaviour
{
    public static PlayerControllerManager controllerManager { get; private set; }

    public Gamepad player1Controller;
    public Gamepad player2Controller;


    void Start()
    {
        if (controllerManager == null)
        {
            controllerManager = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("通ってるぜ");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update() 
    {
        //プレイヤー１がゲームパッドを接続していなかったら
        if (player1Controller == null)
        {
            var gamepads = Gamepad.all;

            for (int i = 0; i < gamepads.Count; i++)
            {
                if (player2Controller != gamepads[i])
                {
                    player1Controller = gamepads[i];
                    Debug.Log($"プレイヤー１の{player1Controller.name}が接続されました");
                    break;
                }
            }
        }

        //プレイヤー１がゲームパッドを接続していなかったら
        if (player2Controller == null)
        {
            var gamepads = Gamepad.all;

            for (int i = 0; i < gamepads.Count; i++)
            {
                if (player1Controller != gamepads[i])
                {
                    player2Controller = gamepads[i];
                    Debug.Log($"プレイヤー２の{player2Controller.name}が接続されました");
                    break;
                }
            }
        }
    }
}