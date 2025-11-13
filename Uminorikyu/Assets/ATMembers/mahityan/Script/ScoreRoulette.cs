using UnityEngine;
using TMPro;
using System.Collections;
using System.Text;

public class ScoreRoulette : MonoBehaviour
{
    [SerializeField] int playerNum;
    [SerializeField] float digitStopDelay = 0.2f;   // 各桁が止まるまでの間隔
    [SerializeField] float spinSpeed = 0.03f;       // 数字が回るスピード
    [SerializeField] float minSpinTime = 0.5f;      // 最短回転時間
    [SerializeField] float maxSpinTime = 1.2f;      // 最長回転時間

    private TMP_Text scoreText;
    private int finalScore;
    private string finalScoreString;

    void Start()
    {
        scoreText = GetComponent<TMP_Text>();
        finalScore = PlayerControllerManager.controllerManager.player[playerNum].score;
        finalScoreString = finalScore.ToString();
        StartCoroutine(RouletteDigits());
    }

    private IEnumerator RouletteDigits()
    {
        int totalDigits = finalScoreString.Length;
        char[] currentDigits = new char[totalDigits];

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
        float spinTime = Random.Range(minSpinTime, maxSpinTime); // 桁ごとに変化をつける
        char finalDigit = finalScoreString[index];

        while (elapsed < spinTime)
        {
            elapsed += spinSpeed;
            // ランダムな数字を表示
            currentDigits[index] = (char)('0' + Random.Range(0, 10));
            UpdateDisplay(currentDigits);
            yield return new WaitForSeconds(spinSpeed);
        }

        // 最終的な桁を確定
        currentDigits[index] = finalDigit;
        UpdateDisplay(currentDigits);
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
