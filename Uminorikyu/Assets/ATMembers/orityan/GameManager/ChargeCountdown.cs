using KanKikuchi.AudioManager;
using UnityEngine;
using UnityEngine.UI;

public class ChargeCountdown: MonoBehaviour
{
    [SerializeField] GameObject[] players;
    [SerializeField] float countTimeMax;
    [SerializeField] GameObject manager;
    [SerializeField] Slider[] stamina;
    [SerializeField] GameObject fadeEvent;

    float count;
    Text text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<Text>();
        text.text = countTimeMax.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeEvent.GetComponent<FadeEventManager>().isFading)
            return;

        count += Time.deltaTime;

        int now = Mathf.FloorToInt(countTimeMax - count);
        text.text = now.ToString();

        if (now == -1)
        {
            //タイマーとスタミナのUIがONになる
            manager.GetComponent<CanvasManager>().CanvasSwitch(1, true);
            manager.GetComponent<CanvasManager>().CanvasSwitch(2, true);
            //スタミナをコピーする
            stamina[0].GetComponent<StaminaCopy>().Copy();
            stamina[1].GetComponent<StaminaCopy>().Copy();
            //プレイヤーのイベントフラグをfalseにする
            players[0].GetComponent<OriiPlayerMove>().nowEvent = false;
            players[1].GetComponent<OriiPlayerMove>().nowEvent = false;
            //チャージイベントのキャンバスをOFFにする
            manager.GetComponent<CanvasManager>().CanvasSwitch(0, false);
        }
    }
}
