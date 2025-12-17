using KanKikuchi.AudioManager;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChargeCountdown : MonoBehaviour
{
    [SerializeField] GameObject[] players;
    [SerializeField] float countTimeMax = 8f; // 実際の総時間
    [SerializeField] float delayTime = 3f; // ディレイ時間
    [SerializeField] GameObject manager;
    [SerializeField] Slider[] stamina;
    [SerializeField] GameObject fadeEvent;

    float count;
    Text text;
    bool isDelayStarted = false;
    bool isGameStarted = false;
    bool hasStoppedCharge = false; // チャージ停止フラグ
    float displayTimeMax; // 表示用の時間（8 - 3 = 5）
    float delayStartTime; // ディレイ開始時刻

    void Start()
    {
        text = GetComponent<Text>();
        displayTimeMax = countTimeMax - delayTime; // 5秒
        text.text = Mathf.CeilToInt(displayTimeMax).ToString(); // "5"から開始
    }

    void Update()
    {
        if (fadeEvent.GetComponent<FadeEventManager>().isFading)
            return;

        count += Time.deltaTime;

        // 表示用の時間でカウントダウン（5, 4, 3, 2, 1, 0）
        if (count < displayTimeMax)
        {
            int now = Mathf.CeilToInt(displayTimeMax - count); // CeilToIntに変更
            text.text = now.ToString();

            // 0になった瞬間にチャージを停止
            if (now == 0 && !hasStoppedCharge)
            {
                stamina[0].GetComponent<StaminaHeal>().isHealActive = false;
                stamina[1].GetComponent<StaminaHeal>().isHealActive = false;
                hasStoppedCharge = true;
            }
        }
        // 0になったらディレイに入る
        else if (!isDelayStarted)
        {
            delayStartTime = count;
            isDelayStarted = true;
        }

        // ディレイ中（0, 1, 2 をカウントアップ）
        if (isDelayStarted && !isGameStarted)
        {
            float delayElapsed = count - delayStartTime;
            int delayCount = Mathf.FloorToInt(delayElapsed);

            if (delayCount < delayTime)
            {
                text.text = delayCount.ToString(); // "0", "1", "2"
            }

            // 総時間（8秒）経過したらゲーム開始
            if (count >= countTimeMax)
            {
                isGameStarted = true;

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

                // スタミナ回復を再開
                stamina[0].GetComponent<StaminaHeal>().isHealActive = true;
                stamina[1].GetComponent<StaminaHeal>().isHealActive = true;

                enabled = false; // このスクリプトを停止
            }
        }
    }
}