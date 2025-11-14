using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ResultWinnerEffect : MonoBehaviour
{
    [SerializeField] private RectTransform player1Image;
    [SerializeField] private RectTransform player2Image;
    [SerializeField] private TMP_Text winText;
    [SerializeField] private ParticleSystem winParticlePrefab; // ← 勝利パーティクルプレハブ

    private int player1Score;
    private int player2Score;
    
    void Start()
    {
        player1Score = PlayerControllerManager.controllerManager.player[0].score;
        player2Score = PlayerControllerManager.controllerManager.player[1].score;
        StartCoroutine(ShowResult());
    }

    IEnumerator ShowResult()
    {
        yield return new WaitForSeconds(1.5f);

        RectTransform winner = (player1Score >= player2Score) ? player1Image : player2Image;
        string winnerName = (player1Score >= player2Score) ? "P1 WIN!" : "P2 WIN!";

        Vector3 startPos = winner.anchoredPosition;
        Vector3 targetPos = Vector3.zero;
        Vector3 startScale = winner.localScale;
        Vector3 targetScale = startScale * 1.5f;

        float duration = 1.2f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, time / duration);

            winner.anchoredPosition = Vector3.Lerp(startPos, targetPos, t);
            winner.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }

        
        // 少し待ってからWINテキスト表示
        yield return new WaitForSeconds(0.5f);

        if (winParticlePrefab != null)
        {
            // Canvasの位置をワールド座標に変換してパーティクルを出す
            Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, winner.position);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f)); // z=10 はカメラ距離

            ParticleSystem ps = Instantiate(winParticlePrefab, worldPos, Quaternion.identity);
            ps.Play();
        }



        winText.text = winnerName;
        winText.gameObject.SetActive(true);
    }
}
