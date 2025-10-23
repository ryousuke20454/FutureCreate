using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;
using Unity.VisualScripting;


public class unko : MonoBehaviour
{

    int rnd;

    // Imageコンポーネントをアタッチ
    //晴れ想定
    public Image targetImage;

    // 新しいスプライトをアタッチ
    //曇り想定
    public Sprite newSprite;

    // 新しいスプライトをアタッチ
    //雨想定
    public Sprite newSprite2;

    void Start()
    {
        rnd = Random.Range(1, 4); // ※ 1～3の範囲でランダムな整数値が返る

        if (rnd == 2)
        {
            targetImage.sprite = newSprite;
        }
        if (rnd == 3)
        {
            targetImage.sprite = newSprite2;
        }
    }

   


    


    // ボタンやイベントで呼び出すメソッド


}
