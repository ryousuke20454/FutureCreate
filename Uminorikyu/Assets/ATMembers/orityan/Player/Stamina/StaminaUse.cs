using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;

public class StaminaUse : MonoBehaviour
{
    //自分自身のコンポーネント取得用
    Slider slider;

    //================================================
    //  ダッシュ判定の設定項目
    //================================================
    [SerializeField] float stickPower;
    [SerializeField] float dashSpeed;
    //================================================
    //  内部変数
    //================================================
    GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //プレイヤーの取得
        player = GetComponent<StaminaState>().player;
        //スタミナについてるスライダーコンポーネントの取得
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        //ダッシュがTrueだったら
        if (player.GetComponent<OriiPlayerMove>().dash)
        {
            //スタミナがあれば
            if (slider.value > 0.0f)
            {
                slider.value -= 0.1f;
                player.GetComponent<OriiPlayerMove>().moveSpeed = dashSpeed;
            }
            else
            {
                player.GetComponent<OriiPlayerMove>().moveSpeed = 0.2f;
                GetComponent<StaminaState>().player.GetComponent<OriiPlayerMove>().barnOut = true;
            }
        }
        else
        {
            player.GetComponent<OriiPlayerMove>().moveSpeed = 0.2f;
        }

    }
}
