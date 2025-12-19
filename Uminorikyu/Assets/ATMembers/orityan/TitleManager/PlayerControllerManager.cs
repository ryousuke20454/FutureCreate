using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public enum Round
{ 
    Title,
    Round1,
    Round2,
    Round3,
    Result
}

public struct PlayerInfo
{
    public Gamepad gamepad;
    public int score;
}

public struct RoundInfo
{
    public int weatherNum;
    public Round roundNum;
}

public class PlayerControllerManager : MonoBehaviour
{
    //シングルトン用の変数
    public static PlayerControllerManager controllerManager { get; private set; }
    //プレイヤー二人分のの情報格納用
    public PlayerInfo[] player = new PlayerInfo[2];
    public RoundInfo round;

    void Awake()
    {
        //このスクリプトのシングルトン化及びスタティック化
        if (controllerManager == null)
        {
            controllerManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        player[0].score = 0;
        player[1].score = 0;
        round.roundNum = Round.Title;
        round.weatherNum = 1;
    }

    void Update() 
    {
        //プレイヤー１がゲームパッドを接続していなかったら
        if (player[0].gamepad == null)
        {
            var gamepads = Gamepad.all;

            for (int i = 0; i < gamepads.Count; i++)
            {
                if (player[1].gamepad != gamepads[i])
                {
                    player[0].gamepad = gamepads[i];
                    Debug.Log($"プレイヤー１の{player[0].gamepad.name}が接続されました");
                    break;
                }
            }
        }

        //プレイヤー１がゲームパッドを接続していなかったら
        if (player[1].gamepad == null)
        {
            var gamepads = Gamepad.all;

            for (int i = 0; i < gamepads.Count; i++)
            {
                if (player[0].gamepad != gamepads[i])
                {
                    player[1].gamepad = gamepads[i];
                    Debug.Log($"プレイヤー２の{player[1].gamepad.name}が接続されました");
                    break;
                }
            }
        }
    }

    public void SetScore(int playerNum ,int score)
    {
        player[playerNum].score += score;
    }

    public void RestartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            player[i].score = 0;
        }
    }
}