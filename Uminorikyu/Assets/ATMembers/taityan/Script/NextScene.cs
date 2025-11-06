using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class NextScene:MonoBehaviour
{
    ////自身の取得用
    //Image image;

    //// 新しいスプライトをアタッチ
    //[SerializeField] Sprite Unko;//我らがうんこ大王


    public RectTransform image; // 動かす画像の参照
    private int counter = 0; // カウンター変数
    private float move = 10.0f; // 移動量（速度）
    private float num = 1720;//端から端までの移動量


    private void Start()
    {
        //image = GetComponent<Image>();
        //image.sprite = Unko;
    }

    void Update()
    {

       


        // スペースキーが押されたらシーンを切り替える
        if (Input.GetKeyDown(KeyCode.Return))
        {

            //image = GetComponent<Image>();
            //image.sprite = Unko;


            // 画像を上下に動かす
            image.position += new Vector3(-move, 0, 0);
            num -= move;

            // カウンターが一定値に達したら方向を反転
            if (num == 0)
            {
                SceneManager.LoadScene("SampleGameScene"); // "NextScene"は切り替え先のシーン名
            }

           
        }
    }
}
