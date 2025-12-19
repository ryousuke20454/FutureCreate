using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class BackWave : MonoBehaviour
{
    public RectTransform image; // 動かす画像の参照
    private float move = 1.0f; //この数字の2倍動きます

    private float num = 860.0f;//端から端までの移動量

    bool canmove = true;//波を表示してうごかすかどうか
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (canmove)
        {
            image.position += new Vector3(move, 0, 0);
            num -= move;
        }


        // カウンターが一定値に達したら方向を反転
        if (num <= 0)
        {
            canmove = false;
        }
    }
}
