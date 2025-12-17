using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;
using Unity.VisualScripting;


public class RandomWeather : MonoBehaviour
{
    //ランダム取得用
    int rnd;

    //自身の取得用
    Image image;

    // 新しいスプライトをアタッチ
    [SerializeField] Sprite sunnySprite;    //晴れ
    [SerializeField] Sprite cloudySprite;   //曇り
    [SerializeField] Sprite rainySprite;    //雨

    void Start()
    {
        rnd = 3;//Random.Range(1, 4); // ※ 1～3の範囲でランダムな整数値が返る
        image = GetComponent<Image>();

        if (PlayerControllerManager.controllerManager != null)
        {
            //晴れ
            if (rnd == 1)
            {
                image.sprite = sunnySprite;
                PlayerControllerManager.controllerManager.round.weatherNum = rnd;
            }
            //曇り
            if (rnd == 2)
            {
                image.sprite = cloudySprite;
                PlayerControllerManager.controllerManager.round.weatherNum = rnd;
            }
            //雨
            if (rnd == 3)
            {
                image.sprite = rainySprite;
                PlayerControllerManager.controllerManager.round.weatherNum = rnd;
            }
        }
    }
}
