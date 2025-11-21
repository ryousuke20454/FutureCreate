using UnityEngine;
using TMPro;
using System.Collections;
using System.Text;
using KanKikuchi.AudioManager;

public class ScoreRoulette : MonoBehaviour
{
    [SerializeField] int playerNum;
    [SerializeField] float digitStopDelay = 0.2f;   // 各桁が止まるまでの間隔
    [SerializeField] float spinSpeed = 0.03f;       // 数字が回るスピード
    [SerializeField] float minSpinTime = 0.5f;      // 最短回転時間
    [SerializeField] float maxSpinTime = 1.2f;      // 最長回転時間
    [SerializeField] GameObject fade;               // フェード取得


    private TMP_Text scoreText;
    private int finalScore;
    private string finalScoreString;
    private bool use; //フラグ

    private int finishedDigits = 0;   // 確定した桁の数
    private bool isAllDigitsFinished = false; // 全桁の処理が終わったか
    private bool playFinishSound = false; // ← 勝者だけ true にする


    void Start()
    {
        use = false;
        scoreText = GetComponent<TMP_Text>();
        finalScore = PlayerControllerManager.controllerManager.player[playerNum].score;
        finalScoreString = finalScore.ToString();

        int score0 = PlayerControllerManager.controllerManager.player[0].score;
        int score1 = PlayerControllerManager.controllerManager.player[1].score;

        // ★ 勝敗判定（同点なら両方 false にする）
        if (score0 > score1 && playerNum == 0)
            playFinishSound = true;
        else if (score1 > score0 && playerNum == 1)
            playFinishSound = true;
        else
            playFinishSound = false;
    }

    private void Update()
    {
        if (!fade.GetComponent<FadeEventManager>().isFading)
        {
            if (!use)
            {
                use = true;
                StartCoroutine(RouletteDigits());
            }
        }
    }

    private IEnumerator RouletteDigits()
    {
        int totalDigits = finalScoreString.Length;
        char[] currentDigits = new char[totalDigits];

        SEManager.Instance.Play(SEPath.DORUM);

        // 一の位から順に桁を動かしていく
        for (int i = 0; i < totalDigits; i++)
        {
            int currentIndex = totalDigits - 1 - i; // 一の位 → 十の位 → ...
            StartCoroutine(RouletteSingleDigit(currentDigits, currentIndex));
            yield return new WaitForSeconds(digitStopDelay);
        }


        
    }

    private IEnumerator RouletteSingleDigit(char[] currentDigits, int index)
    {
        float elapsed = 0f;
        float spinTime = Random.Range(minSpinTime, maxSpinTime);
        char finalDigit = finalScoreString[index];

        while (elapsed < spinTime)
        {
            elapsed += spinSpeed;
            currentDigits[index] = (char)('0' + Random.Range(0, 10));
            UpdateDisplay(currentDigits);
            yield return new WaitForSeconds(spinSpeed);
        }

        // ★ 最終的な桁の決定
        currentDigits[index] = finalDigit;
        UpdateDisplay(currentDigits);

        // ★★★ 桁が止まったことを報告
        finishedDigits++;

        // ★★★ 全桁そろったら、最後に1回だけサウンド
        if (finishedDigits == finalScoreString.Length && !isAllDigitsFinished)
        {
            isAllDigitsFinished = true;
            if (playFinishSound)
            {
                SEManager.Instance.Stop();
                SEManager.Instance.Play(SEPath.JAN); // ← ここで再生！
            }
        }
    }

    private void UpdateDisplay(char[] digits)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < digits.Length; i++)
        {
            sb.Append(digits[i] == '\0' ? ' ' : digits[i]);
        }
        scoreText.text = sb.ToString();
    }
}
