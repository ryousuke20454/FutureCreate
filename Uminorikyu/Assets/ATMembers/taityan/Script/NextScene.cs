using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class NextScene:MonoBehaviour
{
    public RectTransform image; // 動かす画像の参照
    private int counter = 0; // カウンター変数
    private float move =10.0f; //この数字の2倍動きます

    private float num = 1600.0f;//端から端までの移動量

    bool canmove = false;//波を表示してうごかすかどうか


    private void Start()
    {

    }

    void  FixedUpdate()
    {

        // スペースキーが押されたらシーンを切り替える
        if (Input.GetKeyDown(KeyCode.Return))
        {

            canmove = true;

        }

        if (canmove)
        {
            image.position += new Vector3(-move, 0, 0);
            num -= move;
        }


        // カウンターが一定値に達したら方向を反転
        if (num <= 0)
        {
            canmove = false;

            SceneManager.LoadScene("SampleGameScene"); // "NextScene"は切り替え先のシーン名
        }
    }
}
